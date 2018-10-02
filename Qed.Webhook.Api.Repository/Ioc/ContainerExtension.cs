using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Qed.Webhook.Api.Repository.Interfaces;
using Qed.Webhook.Api.Repository.Repositories;
using Qed.Webhook.Api.Shared.Constants;

namespace Qed.Webhook.Api.Repository.Ioc
{
    public static class ContainerExtension
    {
        public static void RegisterWebhookRepositories(this ContainerBuilder builder, IConfiguration configuration)
        {
            var stagingConnectionString = configuration[ConstantString.StagingKeyConfig];
            if (string.IsNullOrEmpty(stagingConnectionString)) throw new Exception(string.Format(ConstantString.EmptyConfiguration, ConstantString.StagingKeyConfig));

            var magnusConnectionString = configuration[ConstantString.MagnusKeyConfig];
            if (string.IsNullOrEmpty(magnusConnectionString)) throw new Exception(string.Format(ConstantString.EmptyConfiguration, ConstantString.MagnusKeyConfig));

            builder.Register(ctx => new RepositoryConfiguration(stagingConnectionString, magnusConnectionString))
                .As<IRepositoryConfiguration>()
                .SingleInstance();

            builder.RegisterType<DbConnectionFactory>().As<IDbConnectionFactory>().InstancePerLifetimeScope();
            builder.RegisterType<EncompassSdkRepository>().As<IEncompassSdkRepository>().InstancePerLifetimeScope();
            builder.RegisterType<EncompassEventRepository>().As<IEncompassEventRepository>().InstancePerLifetimeScope();
            builder.RegisterType<EncompassWebhookDictionaryDataRepository>().As<IEncompassWebhookDictionaryDataRepository>().InstancePerLifetimeScope();
            builder.RegisterType<RedisCacheRepository>().As<IRedisCacheRepository>().InstancePerLifetimeScope();
            builder.RegisterType<EncompassDocumentRepository>().As<IEncompassDocumentRepository>().InstancePerLifetimeScope();
        }
    }
}
