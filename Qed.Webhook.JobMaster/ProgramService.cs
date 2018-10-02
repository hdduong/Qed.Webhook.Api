using System;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using NServiceBus.Transport.SQLServer;
using Qed.JobPicker.Worker.Ioc;
using Qed.Webhook.Api.Repository.Entities;
using Qed.Webhook.Api.Repository.Interfaces;
using Qed.Webhook.Api.Repository.Ioc;
using Qed.Webhook.Api.Shared.Bus.Document;
using Qed.Webhook.Api.Shared.Bus.Document.Events;
using Qed.Webhook.Api.Shared.Cache;
using Qed.Webhook.Api.Shared.Configurations;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Api.Shared.Helpers;
using Qed.Webhook.JobMaster.Interfaces;
using Qed.Webhook.Service.Interfaces;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Qed.Webhook.JobMaster
{
    public class ProgramService : ServiceBase
    {
        private static IEndpointInstance _endpointInstance;

        private static IContainer _container { get; set; }
        private static IConfiguration _configuration { get; set; }
        private static Logger _logger;

        static async Task Main(string[] args)
        {
            _logger = LogManager.GetCurrentClassLogger();

        
            try
            {
                var serviceProvider = AddAutofacContainer();
                var endpointConfig = SetupBus();
                _endpointInstance = await Endpoint.Start(endpointConfig).ConfigureAwait(false);

                await SetupCache(serviceProvider).ConfigureAwait(false);

                using (var service = new ProgramService())
                {
                    if (ServiceHelper.IsService())
                    {
                        Run(service);
                        return;
                    }

                    Console.Title = "JobPicker Worker";
                    service.OnStart(null);

                    Console.WriteLine("JobPicker Worker started. Press any key to exit ...");
                    Console.ReadKey();
                    service.OnStop();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }

        }

        protected override void OnStart(string[] args)
        {
            _logger.Info("OnStart");
            AsyncOnStart().GetAwaiter().GetResult();
        }

        private async Task AsyncOnStart()
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var jobPickupService = scope.Resolve<IJobMasterService>();
                var pickedEvent = await jobPickupService.GetProcessingEventAsync().ConfigureAwait(false);

                if (IsValidEventToProcess(pickedEvent))
                {
                    // transform encompass event to in-house events and then send to bus
                    var documentCmd = await jobPickupService.GetDocumentDownloadCmdAsync(pickedEvent.Id).ConfigureAwait(false);
                    await _endpointInstance.Send(documentCmd).ConfigureAwait(false);
                }
                
            }
        }

        private bool IsValidEventToProcess(WebhookEventQueueEntity eventQueue)
        {
            return eventQueue.Id > 0;
        }

        protected override void OnStop()
        {
            _logger.Info("OnStop");
            _endpointInstance?.Stop().GetAwaiter().GetResult();
        }

        public static IServiceProvider AddAutofacContainer()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<ILoggerFactory, LoggerFactory>();
            serviceCollection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            serviceCollection.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace));
     
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);

            _configuration = Startup.CreateConfiguration();
            containerBuilder.RegisterWebhookRepositories(_configuration);
            containerBuilder.RegisterJobPickerWorker(_configuration, _endpointInstance);

            _container = containerBuilder.Build();

            var serviceProvider = new AutofacServiceProvider(_container);
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            LogManager.LoadConfiguration(NLogConfiguration.GetNLogFileNamePlatformDeployment());

            serviceCollection.AddLogging();

            return serviceProvider;
        }


        public static async Task SetupCache(IServiceProvider serviceProvider)
        {
            var localCache = serviceProvider.GetService<ILocalCache>();         
            var encompassDataService = serviceProvider.GetService<IEncompassWebhookDictonaryDataService>();
            var eventSources = await encompassDataService.GetEncompassEventSourceAsync().ConfigureAwait(false);
            var eventStatuses = await encompassDataService.GetEncompassEventStatusAsync().ConfigureAwait(false);
            var eventTypes = await encompassDataService.GetEncompassEventTypeAsync().ConfigureAwait(false);
            var resourceTypes = await encompassDataService.GetEncompassResourceTypeAsync().ConfigureAwait(false);
            localCache.Add(ConstantString.EventSourceCacheKey, eventSources);
            localCache.Add(ConstantString.EventStatusCacheKey, eventStatuses);
            localCache.Add(ConstantString.EventTypeCacheKey, eventTypes);
            localCache.Add(ConstantString.ResourceTypeCacheKey, resourceTypes);
        }

        private static EndpointConfiguration SetupBus()
        {
            var endpointConfiguration = new EndpointConfiguration(ConstantString.JobMasterEndpoint);
            endpointConfiguration.UseContainer<AutofacBuilder>(
                customizations =>
                {
                    customizations.ExistingLifetimeScope(_container);
                });

            using (var scope = _container.BeginLifetimeScope())
            {
                var repoConfig = scope.Resolve<IRepositoryConfiguration>();
                var stagingConnStr = repoConfig.GetStagingConnectionString();

                endpointConfiguration.SendFailedMessagesTo(ConstantString.ErrorTableName);
                endpointConfiguration.AuditProcessedMessagesTo(ConstantString.AuditTableName);
                endpointConfiguration.EnableInstallers();

                var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
                transport.ConnectionString(stagingConnStr);
                transport.DefaultSchema(ConstantString.BusSchema);
                transport.UseSchemaForQueue(ConstantString.ErrorTableName, ConstantString.BusSchema);
                transport.UseSchemaForQueue(ConstantString.AuditTableName, ConstantString.BusSchema);
                transport.UseSchemaForEndpoint(ConstantString.JobMasterEndpoint, ConstantString.BusSchema);
                transport.CreateMessageBodyComputedColumn();

                var routing = transport.Routing();
                routing.RouteToEndpoint(typeof(DocumentSagaStartCmd), ConstantString.SagaEndpoint);
                routing.RouteToEndpoint(typeof(DocumentMainMessageResponse), ConstantString.SagaEndpoint);

                var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
                var dialect = persistence.SqlDialect<SqlDialect.MsSqlServer>();
                dialect.Schema(ConstantString.BusSchema);
                persistence.ConnectionBuilder(() => new SqlConnection(stagingConnStr));

                var subscriptions = persistence.SubscriptionSettings();
                subscriptions.CacheFor(TimeSpan.FromMinutes(1));

                endpointConfiguration.EnableOutbox();
            }

            return endpointConfiguration;
        }

    }
}
