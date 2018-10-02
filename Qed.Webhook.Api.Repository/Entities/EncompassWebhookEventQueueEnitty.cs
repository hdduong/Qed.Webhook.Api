using System;

namespace Qed.Webhook.Api.Repository.Entities
{
    public class EncompassWebhookEventQueueEnitty 
    {
        public int Id { get; set; }
        public Guid EventId { get; set; }
        public DateTime EventUtcDtTm { get; set; }
        public int EventTypeId { get; set; }
        public string InstanceId { get; set; }
        public string UserId { get; set; }
        public int ResourceTypeId { get; set; }
        public Guid ResourceId { get; set; }
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
