using System.Collections.Generic;
using System.Threading.Tasks;
using Qed.Webhook.Api.Repository.Entities.DictionaryEntities;

namespace Qed.Webhook.Api.Repository.Interfaces
{
    public interface IEncompassWebhookDictionaryDataRepository
    {
        Task<List<WebhookEventStatusEntity>> GetEventStatusAsync();
        Task<List<WebhookEventSourceEntity>> GetEventSourceAsync();
        Task<List<WebhookEventTypeEntity>> GetEventTypeAsync();
        Task<List<WebhookResourceTypeEntity>> GetResourceTypeAsync();
        Task<List<DocumentStatusTypeEntity>> GetDocumentStatusAsync();
    }
}