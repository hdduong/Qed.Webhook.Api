using System.Collections.Concurrent;
using System.Threading.Tasks;
using Qed.Webhook.Api.Repository.Interfaces;
using Qed.Webhook.Service.Interfaces;

namespace Qed.Webhook.Service.Services
{
    public class EncompassWebhookDictonaryDataService : IEncompassWebhookDictonaryDataService
    {
        private readonly IEncompassWebhookDictionaryDataRepository _dictionaryDataRepository;

        public EncompassWebhookDictonaryDataService(IEncompassWebhookDictionaryDataRepository dictionaryDataRepository)
        {
            _dictionaryDataRepository = dictionaryDataRepository;
        }

        public async Task<ConcurrentDictionary<string, int>> GetEncompassEventSourceAsync()
        {
            var data = new ConcurrentDictionary<string, int>();
            var eventSourceEntities = await _dictionaryDataRepository.GetEventSourceAsync().ConfigureAwait(false);

            foreach (var eventSource in eventSourceEntities)
            {
                data.TryAdd(eventSource.WebhookEventSrcName, eventSource.WebhookEventSrcId);
            }
            return data;
        }

        public async Task<ConcurrentDictionary<string, int>> GetEncompassEventStatusAsync()
        {
            var data = new ConcurrentDictionary<string, int>();
            var eventStatusEntities = await _dictionaryDataRepository.GetEventStatusAsync().ConfigureAwait(false);

            foreach (var eventStatus in eventStatusEntities)
            {
                data.TryAdd(eventStatus.WebhookEventStatusName, eventStatus.WebhookEventStatusId);
            }
            return data;
        }

        public async Task<ConcurrentDictionary<string, int>> GetEncompassEventTypeAsync()
        {
            var data = new ConcurrentDictionary<string, int>();
            var eventTypeEntities = await _dictionaryDataRepository.GetEventTypeAsync().ConfigureAwait(false);

            foreach (var eventType in eventTypeEntities)
            {
                data.TryAdd(eventType.WebhookEventTypeName, eventType.WebhookEventTypeId);
            }
            return data;
        }

        public async Task<ConcurrentDictionary<string, int>> GetEncompassResourceTypeAsync()
        {
            var data = new ConcurrentDictionary<string, int>();
            var resourceTypeEntities = await _dictionaryDataRepository.GetResourceTypeAsync().ConfigureAwait(false);

            foreach (var resourceType in resourceTypeEntities)
            {
                data.TryAdd(resourceType.WebhookResrcTypeName, resourceType.WebhookResrcTypeId);
            }
            return data;
        }

        public async Task<ConcurrentDictionary<string, int>> GetEncompassDocumentStatusAsync()
        {
            var data = new ConcurrentDictionary<string, int>();
            var documentStatuses = await _dictionaryDataRepository.GetDocumentStatusAsync().ConfigureAwait(false);

            foreach (var documentStatus in documentStatuses)
            {
                data.TryAdd(documentStatus.DownloadStatusTypeName, documentStatus.DownloadStatusTypeId);
            }
            return data;
        }
    }

}
