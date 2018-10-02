using System;
using NServiceBus;

namespace Qed.Webhook.Api.Shared.Events
{
    public class DocumentEvent : IEvent
    {
        public int Id { get; set; }
        public Guid EventId { get; set; }
        public Guid ResourceId { get; set; }
        public DateTimeOffset EnqueueDtTm { get; set; }
        public DateTimeOffset? PickupDtTm { get; set; }
        public int StatusId { get; set; }
        public int? ThrottleId { get; set; }
        public int WorkerId { get; set; }
        public DateTimeOffset? FinishDtTm { get; set; }
        public string ErrorMsgTxt { get; set; }
    }
}
