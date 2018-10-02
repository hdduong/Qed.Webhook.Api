using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Qed.Webhook.Api.Repository.Entities;
using Qed.Webhook.Api.Repository.Interfaces;
using Qed.Webhook.Api.Shared.Bus.Document;
using Qed.Webhook.Api.Shared.Bus.Document.Events;
using Qed.Webhook.Api.Shared.Cache;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Api.Shared.Enums;
using Qed.Webhook.JobMaster.Interfaces;
using Qed.Webhook.Service.Helpers;

namespace Qed.Webhook.JobMaster.Services
{
    public class JobMasterService : IJobMasterService
    {
        private readonly IEncompassEventRepository _encompassEventRepository;
        private readonly IJobMasterConfiguration _jobPickerConfiguration;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ILocalCache _cache;

        public JobMasterService(IEncompassEventRepository encompassEventRepository, IJobMasterConfiguration jobPickerConfiguration, 
             ILocalCache localCache)
        {
            _encompassEventRepository = encompassEventRepository;
            _jobPickerConfiguration = jobPickerConfiguration;
            _cache = localCache;
        }

        public async Task<WebhookEventQueueEntity> GetProcessingEventAsync()
        {
            var eventStatuses = _cache.Get<ConcurrentDictionary<string, int>>(ConstantString.EventStatusCacheKey);
            var queueEventStatusId = eventStatuses.GetValueOrDefault(EventStatusEnum.InQueue.ToString());
            var processingStatusId = eventStatuses.GetValueOrDefault(EventStatusEnum.Processing.ToString());
            var throttleStatusId = eventStatuses.GetValueOrDefault(EventStatusEnum.Skip.ToString());
            var inQueueIds = await _encompassEventRepository.SelectInQueueEventAsync(_jobPickerConfiguration.GetWorkerId(), queueEventStatusId).ConfigureAwait(false);

            if (inQueueIds.Count == 0)
            {
                _logger.Info(ConstantString.NoEventInQueue);
                return new WebhookEventQueueEntity{Id = 0};
            }

            var processingEventId = inQueueIds.LastOrDefault();
            var throttlingEventIds = inQueueIds.Take(inQueueIds.Count - 1);
            var throttlingEventIdParamter = string.Join(",", throttlingEventIds);

            var processingEvent =
                await _encompassEventRepository.SelectEventAsync(processingEventId).ConfigureAwait(false);

            var dtTmOffsetNow = DateTimeOffset.Now;
            processingEvent.PickupDtTm = dtTmOffsetNow;
            processingEvent.StatusId = processingStatusId;

            int affectedRows = 0;
            if (!throttlingEventIds.Any())
            {
                affectedRows = await _encompassEventRepository.UpdateEventAsync(processingEvent).ConfigureAwait(false);
            }
            else
            {
                affectedRows = await _encompassEventRepository.PerformEventThrottlingAsync(throttlingEventIdParamter, processingEventId,
                    throttleStatusId, dtTmOffsetNow, dtTmOffsetNow, processingEvent).ConfigureAwait(false);
            }
             
            return processingEvent;
        }

        public async Task<DocumentSagaStartCmd> GetDocumentDownloadCmdAsync(int id)
        {
            var encompassEventDetail =
                await _encompassEventRepository.GetEncompassWebhookEventDetailAsync(id).ConfigureAwait(false);

            var docStartCmd = encompassEventDetail.ToDocumentStartCmd();

            return docStartCmd;
        }

        public async Task<int> UpdateEncompassEvent(DocumentMainMessage mainMessage)
        {
            var processedEvent = mainMessage.ToWebhookEventQueueEntity();
            var affectedRows = await _encompassEventRepository.UpdateEventAsync(processedEvent).ConfigureAwait(false);
            affectedRows = await _encompassEventRepository.RemoveEventAsync(mainMessage.Id).ConfigureAwait(false);
            return affectedRows;
        }
    }
}

