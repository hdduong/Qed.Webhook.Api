using System;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using NServiceBus.Transport.SQLServer;
using Qed.Document.Worker.Interfaces;
using Qed.Document.Worker.Ioc;
using Qed.Webhook.Api.Repository.Interfaces;
using Qed.Webhook.Api.Shared.Bus.Document;
using Qed.Webhook.Api.Shared.Bus.Document.Events;
using Qed.Webhook.Api.Shared.Cache;
using Qed.Webhook.Api.Shared.Configurations;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Api.Shared.Helpers;
using Qed.Webhook.Service.Interfaces;
using Qed.Webhook.Service.Ioc;
using Qed.Webhook.Service.Models.Requests.Document;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Qed.Document.Worker
{
    public class ProgramService : ServiceBase
    {
        private static IEndpointInstance _endpointInstance;
        private static IContainer _container { get; set; }
        private static IConfiguration _configuration { get; set; }
        private static IServiceProvider _serviceProvider { get; set; }
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        static async Task Main(string[] args)
        {
            try
            {
                _serviceProvider = AddAutofacContainer();
                await SetupCache(_serviceProvider).ConfigureAwait(false);

                var endpointConfig = SetupBus();
                _endpointInstance = await Endpoint.Start(endpointConfig).ConfigureAwait(false);

                using (var service = new ProgramService())
                {
                    if (ServiceHelper.IsService())
                    {
                        Run(service);
                        return;
                    }

                    Console.Title = "Document Worker";
                    service.OnStart(args);

                    Console.WriteLine("Document Worker started. Press any key to exit ...");
                    Console.ReadKey();
                    _logger.Info("Document Worker started. Press any key to exit ...");
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
            AsyncOnStart(args).GetAwaiter().GetResult();
        }

        private async Task AsyncOnStart(string[] args)
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                // when passing option paramter means in debug mode
                var parameters = new Option();
                Parser.Default.ParseArguments<Option>(args).WithParsed(a => parameters = a);  
                if (!string.IsNullOrEmpty(parameters.LoanGuid))
                {
                    var documentService = _serviceProvider.GetService<IDocumentService>();
                    
                    var result = await documentService.ProcessDocumentAsync(new ProcessDocumentRequest
                    {
                        LoanGuid = Guid.Parse(parameters.LoanGuid)
                    }).ConfigureAwait(false);
                    // there is no update db back for encompass event with ad-hoc testing based on loanId
                }                    
            }
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
            containerBuilder.RegisterWebhookServices(_configuration);
            containerBuilder.RegisterDocumentWorker(_configuration, _endpointInstance);

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
            var encompassSdkConfigService = serviceProvider.GetService<IEncompassSdkConfiguration>();
            var encompassDataService = serviceProvider.GetService<IEncompassWebhookDictonaryDataService>();
            var encompassSdkConfig = await encompassSdkConfigService.GetEncompassSdkConfigAsync().ConfigureAwait(false);
            var eventStatuses = await encompassDataService.GetEncompassEventStatusAsync().ConfigureAwait(false);
            var documentStatuses = await encompassDataService.GetEncompassDocumentStatusAsync().ConfigureAwait(false);

            localCache.Add(ConstantString.EncompassSdkCacheKey, encompassSdkConfig);          
            localCache.Add(ConstantString.EventStatusCacheKey, eventStatuses);
            localCache.Add(ConstantString.DocumentStatusKey, documentStatuses);
        }

        private static EndpointConfiguration SetupBus()
        {
            var endpointConfiguration = new EndpointConfiguration(ConstantString.DocumentWorkerEndpoint);
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
                transport.UseSchemaForEndpoint(ConstantString.DocumentWorkerEndpoint, ConstantString.BusSchema);
                transport.CreateMessageBodyComputedColumn();

                var routing = transport.Routing();
                routing.RouteToEndpoint(typeof(DocumentWorkerEventResponse), ConstantString.SagaEndpoint);
                routing.RegisterPublisher(typeof(DocumentWorkerEvent).Assembly, ConstantString.SagaEndpoint);

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
