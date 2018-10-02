using System;
using System.IO;
using Qed.Webhook.Api.Shared.Constants;

namespace Qed.Webhook.Api.Shared.Configurations
{
    public class NLogConfiguration
    {
        public static string GetNLogFileNamePlatformDeployment()
        {
            var nlogFileName = ConstantString.DefaultNLogConfigFileName;
            var aspnetEnvironment = Environment.GetEnvironmentVariable(ConstantString.AspNetCoreEnvVarName);

            var environmentSpecificLogFileName = $"nlog.{aspnetEnvironment}.config";

            if (File.Exists(environmentSpecificLogFileName))
            {
                nlogFileName = environmentSpecificLogFileName;
            }
            return nlogFileName;
        }

    }
}
