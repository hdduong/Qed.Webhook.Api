using System;
using Newtonsoft.Json;

namespace Qed.Webhook.Api.Shared.Models.WebhookNotification.Encompass
{
    public class Notification
    {
        [JsonProperty("eventId")]
        public Guid EventId { get; set; }

        [JsonProperty("eventTime")]
        public DateTime EventDtTmUtc { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }
}
