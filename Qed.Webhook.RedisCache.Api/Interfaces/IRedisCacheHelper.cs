namespace Qed.Webhook.RedisCache.Api.Services
{
    public interface IRedisCacheHelper
    {
        string GetCacheItem(string key);
        bool AddCacheItem(string key, string value);
    }
}