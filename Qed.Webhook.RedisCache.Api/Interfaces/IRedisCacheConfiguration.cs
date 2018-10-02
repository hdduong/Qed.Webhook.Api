namespace Qed.Webhook.RedisCache.Api.Interfaces
{
    public interface IRedisCacheConfiguration
    {
        int NumberOfWorkers { get; set; }
        string RedisCacheServerAddress { get; set; }
    }
}