namespace Qed.Webhook.Api.Shared.Interfaces
{
    public interface IEncompassAuthenticationConfiguration
    {
        string WebhookSecretKey { get; set; }
    }
}