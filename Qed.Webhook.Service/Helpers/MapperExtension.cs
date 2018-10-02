using Qed.Webhook.Api.Repository.Entities;
using Qed.Webhook.Api.Shared.Bus.Document;
using Qed.Webhook.Api.Shared.Bus.Document.Events;
using Qed.Webhook.Service.Models;
using Qed.Webhook.Service.Models.Responses.Document;

namespace Qed.Webhook.Service.Helpers
{
    public static class MapperExtension
    {
        public static EncompassSdkConfig ToEncompassSdkConfig(this EncompassSdkConfigEntity sdkConfigEntity)
        {
            return new EncompassSdkConfig
            {
                EncryptionBit = sdkConfigEntity.EncryptionBit,
                Name = sdkConfigEntity.Name,
                EncryptedPassword = sdkConfigEntity.Password,
                DecryptedPassword = string.Empty,
                InstanceId = string.Empty,
                SeqNum = sdkConfigEntity.SeqNum,
                ServerUri = sdkConfigEntity.ServerUri,
                UserId = sdkConfigEntity.UserId,
                ClientId = sdkConfigEntity.ApiClientId,
                ClientSecret = sdkConfigEntity.ApiClientSecret
            };
        }

        public static DocumentSagaStartCmd ToDocumentStartCmd(this EncompassWebhookEventQueueEnitty encompassEventDtl)
        {
            return new DocumentSagaStartCmd
            {
                CmdId = $"{encompassEventDtl.Id}-{encompassEventDtl.CorrelationId}",
                Id = encompassEventDtl.Id,
                CorrelationId = encompassEventDtl.CorrelationId,
                StatusId = encompassEventDtl.StatusId,
                EventId = encompassEventDtl.EventId,
                InstanceId = encompassEventDtl.InstanceId,
                ResourceId = encompassEventDtl.ResourceId,
                EnqueueDtTm = encompassEventDtl.EnqueueDtTm,
                PickupDtTm = encompassEventDtl.PickupDtTm,
                ThrottleId = encompassEventDtl.ThrottleId,
                WorkerId = encompassEventDtl.WorkerId
            };
        }

        public static DocumentWorkerEventResponse ToDocumentEventResponse(this ProcessDocumentResponse processDocumentDoneResponse)
        {
            return new DocumentWorkerEventResponse
            {
                ErrorMsgTxt = processDocumentDoneResponse.ErrorMsgTxt,
                EventReason = processDocumentDoneResponse.EventReason,
                EventStatus = processDocumentDoneResponse.EventStatus,
                SuccessBit = processDocumentDoneResponse.SuccessBit
            };
        }

        public static WebhookEventQueueEntity ToWebhookEventQueueEntity(this DocumentMainMessage documentDoneMessage)
        {
            return new WebhookEventQueueEntity
            {
                Id = documentDoneMessage.Id,
                CorrelationId = documentDoneMessage.CorrelationId,
                EnqueueDtTm = documentDoneMessage.EnqueueDtTm,
                ErrorMsgTxt = documentDoneMessage.ErrorMsgTxt,
                FinishDtTm = documentDoneMessage.FinishDtTm,
                PickupDtTm = documentDoneMessage.PickupDtTm,
                StatusId = documentDoneMessage.StatusId,
                ThrottleId = documentDoneMessage.ThrottleId,
                WorkerId = documentDoneMessage.WorkerId
            };
        }
    }
}
