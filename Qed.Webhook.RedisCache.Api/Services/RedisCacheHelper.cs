using Qed.Webhook.RedisCache.Api.Interfaces;
using ServiceStack.Redis;

namespace Qed.Webhook.RedisCache.Api.Services
{
    public class RedisCacheHelper : IRedisCacheHelper
    {
        private readonly IRedisCacheConfiguration _redisCacheConfiguration;

        public RedisCacheHelper(IRedisCacheConfiguration redisCacheConfiguration)
        {
            _redisCacheConfiguration = redisCacheConfiguration;
        }

        public string GetCacheItem(string key)
        {
            using (var manager = new PooledRedisClientManager(_redisCacheConfiguration.RedisCacheServerAddress))
            {
                using (var client = manager.GetClient())
                {
                    return client.Get<string>(key);
                }
            }           
        }

        public bool AddCacheItem(string key, string value)
        {
            using (var manager = new PooledRedisClientManager(_redisCacheConfiguration.RedisCacheServerAddress))
            {
                using (var client = manager.GetClient())
                {
                    if (string.IsNullOrEmpty(GetCacheItem(key)))
                    {
                        return client.Set(key, value);
                    }
                        
                }
            }
            return true;
        }
    }
}
