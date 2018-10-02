using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting.Internal;
using Qed.Webhook.Api.Shared.Constants;

namespace Qed.Document.Worker
{
    public class Startup
    {
        private static IConfigurationBuilder Configure(IConfigurationBuilder config, string environmentName)
        {
            if (string.IsNullOrEmpty(environmentName) || environmentName.Equals(ConstantString.ProdEnv, StringComparison.InvariantCultureIgnoreCase))
            {
                return config
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            }

            return config
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables();

        }

        public static IConfiguration CreateConfiguration()
        {
            var env = new HostingEnvironment
            {
                EnvironmentName = Environment.GetEnvironmentVariable(ConstantString.AspNetCoreEnvVarName),
                ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
                ContentRootFileProvider = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory)
            };

            var config = new ConfigurationBuilder();
            var configured = Configure(config, env.EnvironmentName);
            return configured.Build();
        }

    }
}
