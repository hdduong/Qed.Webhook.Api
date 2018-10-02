using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Qed.Webhook.Service.Interfaces
{
    public interface IEncompassWebhookDictonaryDataService
    {
        Task<ConcurrentDictionary<string, int>> GetEncompassEventSourceAsync();
        Task<ConcurrentDictionary<string, int>> GetEncompassEventStatusAsync();
        Task<ConcurrentDictionary<string, int>> GetEncompassEventTypeAsync();
        Task<ConcurrentDictionary<string, int>> GetEncompassResourceTypeAsync();
        Task<ConcurrentDictionary<string, int>> GetEncompassDocumentStatusAsync();
    }
}