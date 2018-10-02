namespace Qed.Webhook.Api.Shared.Cache
{
    public interface ILocalCache
    {
        void Add(string key, object value);
        T Get<T>(string key);
    }
}