using System;
using System.Threading.Tasks;
using Qed.Webhook.Api.Repository.Interfaces;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Api.Shared.Helpers;
using Qed.Webhook.Service.Helpers;
using Qed.Webhook.Service.Interfaces;
using Qed.Webhook.Service.Models;

namespace Qed.Webhook.Service.Configurations
{
    public class EncompassSdkConfiguration : IEncompassSdkConfiguration
    {
        private readonly IEncompassSdkRepository _encompassSdkRepository;

        public EncompassSdkConfiguration(IEncompassSdkRepository encompasSdkRepository)
        {
            _encompassSdkRepository = encompasSdkRepository;
        }

        public async Task<EncompassSdkConfig> GetEncompassSdkConfigAsync()
        {
            var sdkConfigEntity = await _encompassSdkRepository.GetEncomopassCredentialAsync(ConstantString.DefaultEncompassSdkName)
                .ConfigureAwait(false);

            var sdkConfig = sdkConfigEntity.ToEncompassSdkConfig();

            if (string.IsNullOrEmpty(sdkConfig.EncryptedPassword))
            {
                throw new Exception(string.Format(ConstantString.EncompassSdkUnavailableConfig, nameof(sdkConfig.EncryptedPassword)));
            }

            if (string.IsNullOrEmpty(sdkConfig.ServerUri))
            {
                throw new Exception(string.Format(ConstantString.EncompassSdkUnavailableConfig, nameof(sdkConfig.ServerUri)));
            }

            if (string.IsNullOrEmpty(sdkConfig.UserId))
            {
                throw new Exception(string.Format(ConstantString.EncompassSdkUnavailableConfig, nameof(sdkConfig.UserId)));
            }

            if (string.IsNullOrEmpty(sdkConfig.ClientId))
            {
                throw new Exception(string.Format(ConstantString.EncompassSdkUnavailableConfig, nameof(sdkConfig.ClientId)));
            }

            if (string.IsNullOrEmpty(sdkConfig.ClientSecret))
            {
                throw new Exception(string.Format(ConstantString.EncompassSdkUnavailableConfig, nameof(sdkConfig.ClientSecret)));
            }

            sdkConfig.DecryptedPassword = sdkConfig.EncryptionBit ? EncompassSdkHelper.Decrypt(sdkConfig.EncryptedPassword) : sdkConfig.EncryptedPassword;

            sdkConfig.InstanceId = EncompassSdkHelper.GetEncompassInstance(sdkConfig.ServerUri);

            return sdkConfig;

        }


    }
}
