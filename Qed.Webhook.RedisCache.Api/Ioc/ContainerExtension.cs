using Autofac;
using Microsoft.Extensions.Configuration;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Api.Shared.Loggings;
using Qed.Webhook.RedisCache.Api.Configurations;
using Qed.Webhook.RedisCache.Api.Interfaces;
using Qed.Webhook.RedisCache.Api.Services;


namespace Qed.Webhook.RedisCache.Api.Ioc
{
    public static class ContainerExtension
    {
        public static void RegisteRedisCacheApi(this ContainerBuilder builder, IConfiguration configuration)
        {
            var redisCacheServerAddressUri = configuration[ConstantString.RedisServerAddressConfig];
            if (string.IsNullOrEmpty(redisCacheServerAddressUri)) throw new ApiException(string.Format(ConstantString.EmptyConfiguration, ConstantString.RedisServerAddressConfig));

            var numberOfWorker = int.Parse(configuration[ConstantString.NumberOfWorkerConfig]);
            if (string.IsNullOrEmpty(numberOfWorker.ToString())) throw new ApiException(string.Format(ConstantString.EmptyConfiguration, ConstantString.NumberOfWorkerConfig));

            builder.Register(ctx => new RedisCacheConfiguration(numberOfWorker, redisCacheServerAddressUri))
                .As<IRedisCacheConfiguration>()
                .SingleInstance();
           
            builder.RegisterType<RedisCacheHelper>().As<IRedisCacheHelper>().InstancePerLifetimeScope();
            builder.RegisterType<RedisCacheService>().As<IRedisCacheService>().InstancePerLifetimeScope();

        }
    }
}
