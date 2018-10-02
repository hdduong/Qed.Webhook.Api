using System;
using System.Collections.Generic;
using System.Text;
using Qed.Webhook.Api.Shared.Bus.Document;
using Qed.Webhook.Api.Shared.Bus.Document.Events;

namespace Qed.Webhook.Api.Shared.Helpers
{
    public static class MessageBusExtension
    {
        public static DocumentWorkerEvent ToDocumentWorkerEvent(this DocumentSagaStartCmd documentStartCmd)
        {
            return new DocumentWorkerEvent
            {
                CmdId = documentStartCmd.CmdId,
                CorrelationId = documentStartCmd.CorrelationId,
                EventId = documentStartCmd.EventId,
                Id = documentStartCmd.Id,
                InstanceId = documentStartCmd.InstanceId,
                ResourceId = documentStartCmd.ResourceId,
                StatusId = documentStartCmd.StatusId
            };
        }

        public static DocumentMainMessage ToDouDocumentMainMessage(this DocumentWorkerEventResponse response)
        {
            return new DocumentMainMessage
            {
                CmdId = response.CmdId,
                Id = response.Id,
                CorrelationId = response.CorrelationId,
                StatusId = response.StatusId,
                EventId = response.EventId,
                ResourceId = response.ResourceId,
                InstanceId = response.InstanceId,
                SuccessBit = response.SuccessBit,
                EventStatus = response.EventStatus,
                EventReason = response.EventReason,
                ErrorMsgTxt = response.ErrorMsgTxt
            };
        }

        public static DocumentMainMessageResponse ToDocumentMainMessageResponse(this DocumentMainMessage response)
        {
            return new DocumentMainMessageResponse
            {
                CmdId = response.CmdId,
                CorrelationId = response.CorrelationId,
                Id = response.Id
            };
        }
    }
}
