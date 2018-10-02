using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qed.Webhook.Api.Repository.Entities;
using Qed.Webhook.Api.Repository.Entities.EncompassEventEntities;

namespace Qed.Webhook.Api.Repository.Interfaces
{
    public interface IEncompassEventRepository
    {
        Task<int> AddEventAsync(EnqueueEncompassEventEntity encompassEvent);
        Task<List<int>> SelectInQueueEventAsync(int workerId, int queueStatusId);
        Task<int> ThrottlingEventAsync(string ids, int throttleId, int throttleStatusId,
            DateTimeOffset pickupDateTime, DateTimeOffset finishDateTime);
        Task<WebhookEventQueueEntity> SelectEventAsync(int id);
        Task<int> UpdateEventAsync(WebhookEventQueueEntity updatingEvent);

        Task<int> PerformEventThrottlingAsync(string updatingIds, int throttleId, int throttleStatusId,
            DateTimeOffset pickupDateTime, DateTimeOffset finishDateTime, WebhookEventQueueEntity updatingEvent);

        Task<EncompassWebhookEventQueueEnitty> GetEncompassWebhookEventDetailAsync(int id);
        Task<int> RemoveEventAsync(int webhookEventId);
    }
}