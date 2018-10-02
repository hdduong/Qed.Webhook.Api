using System;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Qed.Webhook.Api.Shared.Configurations;
using Qed.Webhook.RedisCache.Api.Interfaces;
using LogLevel = NLog.LogLevel;

namespace Qed.Webhook.RedisCache.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog(NLogConfiguration.GetNLogFileNamePlatformDeployment()).GetCurrentClassLogger();
            try
            {
                var webhost = BuildWebHost(args);
                logger.Info("Start loading documents into cache...");
                //var bootstrapCache = await BootstrapRedisCache(webhost.Services).ConfigureAwait(false);
                logger.Info("Latest documents loaded into cache");
                webhost.Run();
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex);
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
                })
                .UseNLog()  // NLog: setup NLog for Dependency injection
                .ConfigureServices(services => services.AddAutofac())
                .Build();

        public static async Task<bool> BootstrapRedisCache(IServiceProvider serviceProvider)
        {
            var redisCache = serviceProvider.GetService<IRedisCacheService>();
            var loadedSuccessBit = await redisCache.BootstrapDocumentAsync().ConfigureAwait(false);
            return loadedSuccessBit;
        }
    }
}
