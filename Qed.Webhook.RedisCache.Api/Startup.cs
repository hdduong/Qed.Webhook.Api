using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qed.Webhook.Api.Repository.Ioc;
using Qed.Webhook.RedisCache.Api.Ioc;
using Qed.Webhook.RedisCache.Api.Middlewares;

namespace Qed.Webhook.RedisCache.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterWebhookRepositories(Configuration);
            builder.RegisteRedisCacheApi(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
