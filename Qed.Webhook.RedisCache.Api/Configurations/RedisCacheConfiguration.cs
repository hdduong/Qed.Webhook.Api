using Qed.Webhook.RedisCache.Api.Interfaces;

namespace Qed.Webhook.RedisCache.Api.Configurations
{
    public class RedisCacheConfiguration : IRedisCacheConfiguration
    {
        public int NumberOfWorkers { get; set; }
        public string RedisCacheServerAddress { get; set; }

        public RedisCacheConfiguration(int numberOfWorkers, string redisCacheServerAddress)
        {
            NumberOfWorkers = numberOfWorkers;
            RedisCacheServerAddress = redisCacheServerAddress;
        }
    }
}
