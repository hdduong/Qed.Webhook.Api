using System;
using System.Net.Http;
using Autofac;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using Qed.Document.Worker.Configurations;
using Qed.Document.Worker.Interfaces;
using Qed.Document.Worker.Services;
using Qed.Webhook.Api.Shared.Cache;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Service.Interfaces;
using Qed.Webhook.Service.Services;
using IDocumentWorkerConfiguration = Qed.Document.Worker.Interfaces.IDocumentWorkerConfiguration;

namespace Qed.Document.Worker.Ioc
{
    public static class ContainerExtension
    {
        public static void RegisterDocumentWorker(this ContainerBuilder builder, IConfiguration configuration, IEndpointInstance endpoint)
        {
            builder.Register(ctx =>
                {
                    var downloadPath = configuration[ConstantString.DownloadPathConfig]; 
                    return new DocumentWorkerConfiguration(downloadPath);
                })
                .As<IDocumentWorkerConfiguration>()
                .SingleInstance();
            builder.RegisterType<LocalCache>().As<ILocalCache>().SingleInstance();
            builder.RegisterType<DocumentService>().As<IDocumentService>().InstancePerLifetimeScope();

            // httpClient for Encompass endpoint
            var apiEndpointUri = configuration[ConstantString.ApiKeyConfig];
            if (string.IsNullOrEmpty(apiEndpointUri)) throw new Exception(string.Format(ConstantString.EmptyConfiguration, ConstantString.ApiKeyConfig));

            builder.Register(ctx => new HttpClient { BaseAddress = new Uri(apiEndpointUri), Timeout = TimeSpan.FromSeconds(int.Parse(configuration[ConstantString.TimeoutConfigKey]))})
                .Named<HttpClient>(ConstantString.EncompassApiHttpClientName)
                .SingleInstance();

            builder.RegisterType<EncompassClient>().As<IEncompassClient>()
                .WithParameter(
                    (pi, c) => pi.ParameterType == (typeof(HttpClient)) && pi.Name.Equals(ConstantString.HttpClientInputParameterName, StringComparison.InvariantCultureIgnoreCase),
                    (pi, c) => c.ResolveNamed<HttpClient>(ConstantString.EncompassApiHttpClientName))
                .InstancePerLifetimeScope();

            // httpClient for RedisCache endpoint
            var redisCacheApiEndpoint = configuration[ConstantString.RedisApiConfig];
            if (string.IsNullOrEmpty(redisCacheApiEndpoint)) throw new Exception(string.Format(ConstantString.EmptyConfiguration, ConstantString.RedisApiConfig));

            builder.Register(ctx => new HttpClient { BaseAddress = new Uri(redisCacheApiEndpoint) })
                .Named<HttpClient>(ConstantString.RedisCacheApiHttpClientName)
                .SingleInstance();

            builder.RegisterType<RedisCacheApiClient>().As<IRedisCacheApiClient>()
                .WithParameter(
                    (pi, c) => pi.ParameterType == (typeof(HttpClient)) && pi.Name.Equals(ConstantString.HttpClientInputParameterName, StringComparison.InvariantCultureIgnoreCase),
                    (pi, c) => c.ResolveNamed<HttpClient>(ConstantString.RedisCacheApiHttpClientName))
                .InstancePerLifetimeScope();

            builder.Register(x => endpoint).As<IEndpointInstance>().SingleInstance();
        }
    }
}
