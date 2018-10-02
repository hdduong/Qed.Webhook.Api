using System;

namespace Qed.Webhook.Api.Repository.Entities
{
    public class WebhookEventEntity
    {
        public int Id { get; set; }
        public int SourceTypeId { get; set; }
        public DateTimeOffset EventDtTm { get; set; }
    }
}
