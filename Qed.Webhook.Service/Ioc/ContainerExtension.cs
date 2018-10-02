using Autofac;
using Microsoft.Extensions.Configuration;
using Qed.Webhook.Api.Repository.Ioc;
using Qed.Webhook.Service.Configurations;
using Qed.Webhook.Service.Interfaces;
using Qed.Webhook.Service.Services;

namespace Qed.Webhook.Service.Ioc
{
    public static class ContainerExtension
    {
        public static void RegisterWebhookServices(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterWebhookRepositories(configuration);
            builder.RegisterType<EncompassSdkConfiguration>().As<IEncompassSdkConfiguration>().SingleInstance();
            builder.RegisterType<EncompassWebhookDictonaryDataService>().As<IEncompassWebhookDictonaryDataService>().InstancePerLifetimeScope();
            builder.RegisterType<EncompassEventService>().As<IEncompassEventService>().InstancePerLifetimeScope();
        }
    }
}
