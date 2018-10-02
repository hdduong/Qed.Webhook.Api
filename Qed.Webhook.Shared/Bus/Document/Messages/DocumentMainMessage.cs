using System;
using NServiceBus;
using Qed.Webhook.Api.Shared.Enums;

namespace Qed.Webhook.Api.Shared.Bus.Document.Events
{
    public class DocumentMainMessage :  IMessage
    {
        public string CmdId { get; set; }
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
        public int StatusId { get; set; }
        public Guid EventId { get; set; }
        public Guid ResourceId { get; set; }
        public string InstanceId { get; set; }
        public DateTimeOffset EnqueueDtTm { get; set; }
        public DateTimeOffset? PickupDtTm { get; set; }
        public DateTimeOffset? FinishDtTm { get; set; }
        public int? ThrottleId { get; set; }
        public int WorkerId { get; set; }
        public bool SuccessBit { get; set; }
        public EventStatusEnum EventStatus { get; set; }
        public string EventReason { get; set; }
        public string ErrorMsgTxt { get; set; }
    }
}
