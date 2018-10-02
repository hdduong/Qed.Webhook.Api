using System;

namespace Qed.Webhook.Api.Repository.Entities
{
    public class WebhookEventQueueEntity
    {
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTimeOffset EnqueueDtTm { get; set; }
        public DateTimeOffset? PickupDtTm { get; set; }
        public int StatusId { get; set; }
        public int? ThrottleId { get; set; }
        public int WorkerId { get; set; }
        public DateTimeOffset? FinishDtTm { get; set; }
        public string ErrorMsgTxt { get; set; }
    }
}
