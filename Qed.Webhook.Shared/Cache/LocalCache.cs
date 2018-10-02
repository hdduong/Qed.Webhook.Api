using System.Collections.Concurrent;

namespace Qed.Webhook.Api.Shared.Cache
{
    public class LocalCache : ILocalCache
    {
        private readonly ConcurrentDictionary<string, object> _cache;

        public LocalCache()
        {
            _cache = new ConcurrentDictionary<string, object>();
        }

        public void Add(string key, object value)
        {
            _cache.TryAdd(key, value);
        }

        public T Get<T>(string key)
        {
            if (!string.IsNullOrEmpty(key) && _cache.TryGetValue(key, out var value) && value != null && value.GetType() == typeof(T))
            {
                return (T)value;
            }

            return default(T);
        }
    }
}
