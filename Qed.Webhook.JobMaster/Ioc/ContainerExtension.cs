using Autofac;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using Qed.JobPicker.Worker.Configurations;
using Qed.Webhook.Api.Shared.Cache;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.JobMaster.Interfaces;
using Qed.Webhook.JobMaster.Services;
using Qed.Webhook.Service.Interfaces;
using Qed.Webhook.Service.Services;

namespace Qed.JobPicker.Worker.Ioc
{
    public static class ContainerExtension
    {
        public static void RegisterJobPickerWorker(this ContainerBuilder builder, IConfiguration configuration, IEndpointInstance endpoint)
        {
            builder.Register(ctx =>
                {
                    var workerId = int.Parse(configuration[ConstantString.JobMasterIdConfig]);
                    return new JobPickerConfiguration(workerId);
                })
                .As<IJobMasterConfiguration>()
                .SingleInstance();
            builder.Register(x => endpoint).As<IEndpointInstance>().SingleInstance();
            builder.RegisterType<LocalCache>().As<ILocalCache>().SingleInstance();
            builder.RegisterType<EncompassWebhookDictonaryDataService>().As<IEncompassWebhookDictonaryDataService>().InstancePerLifetimeScope();
            builder.RegisterType<JobMasterService>().As<IJobMasterService>().InstancePerLifetimeScope();
        }
    }
}
