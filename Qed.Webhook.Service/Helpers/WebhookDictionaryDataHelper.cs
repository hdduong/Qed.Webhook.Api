using System.Collections.Concurrent;
using Qed.Webhook.Service.Models;

namespace Qed.Webhook.Service.Helpers
{
    public class WebhookDictionaryDataHelper
    {
        public static DictionaryId Get(ConcurrentDictionary<string, int> data, string lookupKey)
        {
            bool itemExist = data.TryGetValue(lookupKey, out var id);
            return new DictionaryId
            {
                Id = id,
                IsValidKeyBit = itemExist
            };
        }
    }
}
