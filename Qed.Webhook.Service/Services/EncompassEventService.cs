using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Qed.Webhook.Api.Repository.Entities.EncompassEventEntities;
using Qed.Webhook.Api.Repository.Interfaces;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Api.Shared.Enums;
using Qed.Webhook.Service.Helpers;
using Qed.Webhook.Service.Interfaces;
using Qed.Webhook.Service.Models.Requests.Event;
using Qed.Webhook.Service.Models.Responses.Event;

namespace Qed.Webhook.Service.Services
{
    public class EncompassEventService : IEncompassEventService
    {
        private readonly IEncompassEventRepository _eventRepository;
        private readonly IRedisCacheApiClient _redisCacheApiClient;

        public EncompassEventService(IEncompassEventRepository eventRepository, IRedisCacheApiClient redisCacheApiClient)
        {
            _eventRepository = eventRepository;
            _redisCacheApiClient = redisCacheApiClient;
        }

        private EncompassEventDbConfigValidationResult ValidateDbConfigurationForEvent(AddEncompassEventRequest request)
        {
            var response = new EncompassEventDbConfigValidationResult();

            var eventSources = request.EventSource;
            var eventStatuses = request.EventStatus;
            var eventTypes = request.EventType;
            var resourceTypes = request.EventResourceType;

            response.EncompassSource = WebhookDictionaryDataHelper.Get(eventSources, EventSourceEnum.Encompass.ToString());
            if (!response.EncompassSource.IsValidKeyBit)
            {
                response.EncompassEventResponse.ErrorMsg += string.Format(ConstantString.EnumValueNotFound, nameof(request.EventSource), EventSourceEnum.Encompass.ToString());
            }

            response.EventStatusNotStart = WebhookDictionaryDataHelper.Get(eventStatuses, EventStatusEnum.InQueue.ToString());
            if (!response.EventStatusNotStart.IsValidKeyBit)
            {
                response.EncompassEventResponse.ErrorMsg += string.Format(ConstantString.EnumValueNotFound, nameof(request.EventStatus), EventStatusEnum.InQueue.ToString());
            }

            response.EventStatusNotSupport = WebhookDictionaryDataHelper.Get(eventStatuses, EventStatusEnum.NotSupported.ToString());
            if (!response.EventStatusNotSupport.IsValidKeyBit)
            {
                response.EncompassEventResponse.ErrorMsg += string.Format(ConstantString.EnumValueNotFound, nameof(request.EventStatus), EventStatusEnum.NotSupported.ToString());
            }

            response.EventType = WebhookDictionaryDataHelper.Get(eventTypes, request.EncompassEvent.EventType);
            if (!response.EventType.IsValidKeyBit)
            {
                response.EncompassEventResponse.ErrorMsg += string.Format(ConstantString.EnumValueNotFound, nameof(request.EventType), request.EncompassEvent.EventType);
            }

            response.ResourceType = WebhookDictionaryDataHelper.Get(resourceTypes, request.EncompassEvent.Meta.ResourceType);
            if (!response.ResourceType.IsValidKeyBit)
            {
                response.EncompassEventResponse.ErrorMsg += string.Format(ConstantString.EnumValueNotFound, nameof(request.EventResourceType), request.EncompassEvent.Meta.ResourceType);
            }

            if (!string.IsNullOrEmpty(response.EncompassEventResponse.ErrorMsg))
            {
                response.EncompassEventResponse.IsSuccessBit = false;
            }

            return response;
        }


        private async Task<int> GetWorkerIdForEvent(Guid resourceId)
        {
            // set workerId 
            var workerId = await _redisCacheApiClient.GetWorkerIdAsync(resourceId).ConfigureAwait(false);
            var workerIdAssignBit = await _redisCacheApiClient.SetWorkerIdAsync(resourceId, workerId).ConfigureAwait(false);

            if (!workerIdAssignBit)
            {
                return int.Parse(ConstantString.DefaultWorkerId);
            }

            return workerId;
        }

        public async Task<AddEncompassEventResponse> AddEventToDbAsync(AddEncompassEventRequest request)
        {
            var response = ValidateDbConfigurationForEvent(request);
            if (!response.EncompassEventResponse.IsSuccessBit)
            {
                return response.EncompassEventResponse;
            }

            var enqueueingEvent = new EnqueueEncompassEventEntity();

            var currentOffsetDateTime = DateTimeOffset.Now;
            enqueueingEvent.WebhookEvent.SourceTypeId = response.EncompassSource.Id;
            enqueueingEvent.WebhookEvent.EventDtTm = currentOffsetDateTime;

            enqueueingEvent.EventDetail.EventId = request.EncompassEvent.EventId;
            enqueueingEvent.EventDetail.EventTypeId = response.EventType.Id;
            enqueueingEvent.EventDetail.EventUtcDtTm = request.EncompassEvent.EventDtTmUtc.ToUniversalTime();
            enqueueingEvent.EventDetail.InstanceId = request.EncompassEvent.Meta.InstanceId;
            enqueueingEvent.EventDetail.MsgTxt = JsonConvert.SerializeObject(request.EncompassEvent);
            enqueueingEvent.EventDetail.ResourceId = request.EncompassEvent.Meta.ResourceId;
            enqueueingEvent.EventDetail.ResourceTypeId = response.ResourceType.Id;
            enqueueingEvent.EventDetail.UserId = request.EncompassEvent.Meta.UserId;

            enqueueingEvent.EventQueue.CorrelationId = Guid.NewGuid();
            enqueueingEvent.EventQueue.EnqueueDtTm = currentOffsetDateTime;
            enqueueingEvent.EventQueue.PickupDtTm = null;
            enqueueingEvent.EventQueue.ErrorMsgTxt = string.Empty;
            enqueueingEvent.EventQueue.ThrottleId = null;

            if (request.EncompassEvent.Meta.ResourceType.Equals(EncompassResourceTypeEnum.Loan.ToString(),
                StringComparison.InvariantCultureIgnoreCase))
            {
                enqueueingEvent.EventQueue.StatusId = response.EventStatusNotStart.Id;
                enqueueingEvent.EventQueue.FinishDtTm = null;
            }
            else
            {
                enqueueingEvent.EventQueue.StatusId = response.EventStatusNotSupport.Id;
                enqueueingEvent.EventQueue.FinishDtTm = currentOffsetDateTime;
            }

            var workerId = await GetWorkerIdForEvent(request.EncompassEvent.Meta.ResourceId);
            enqueueingEvent.EventQueue.WorkerId = workerId;

            response.EncompassEventResponse.EventId = await _eventRepository.AddEventAsync(enqueueingEvent).ConfigureAwait(false);

            return response.EncompassEventResponse;
        }
    }
}
