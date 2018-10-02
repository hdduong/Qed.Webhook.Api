using Qed.Webhook.Api.Shared.Interfaces;

namespace Qed.Webhook.Api.Shared.Configurations
{
    public class EncompassAuthenticationConfiguration : IEncompassAuthenticationConfiguration
    {
        public string WebhookSecretKey { get; set; }

        public EncompassAuthenticationConfiguration(string secretKey)
        {
            WebhookSecretKey = secretKey;
        }
    }
}
