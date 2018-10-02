namespace Qed.Webhook.Api.Shared.Enums
{
    public enum EventStatusEnum 
    {
        Unknown,
        InQueue,
        Processing,
        Error,
        Done,
        Skip,
        NotSupported,
        CacheOutOfSync
    }
}
