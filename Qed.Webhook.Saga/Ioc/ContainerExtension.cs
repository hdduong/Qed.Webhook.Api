using Autofac;
using NServiceBus;

namespace Qed.Webhook.Saga.Ioc
{
    public static class ContainerExtension
    {
        public static void RegisterSaga(this ContainerBuilder builder, IEndpointInstance endpoint)
        {
            builder.Register(x => endpoint).As<IEndpointInstance>().SingleInstance();
        }
    }
}
