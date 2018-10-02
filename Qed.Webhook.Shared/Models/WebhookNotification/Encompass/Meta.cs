using System;
using Newtonsoft.Json;

namespace Qed.Webhook.Api.Shared.Models.WebhookNotification.Encompass
{
    public class Meta
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("resourceId")]
        public Guid ResourceId { get; set; }

        [JsonProperty("resourceType")]
        public string ResourceType { get; set; }

        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }

        [JsonProperty("resourceRef")]
        public string ResourceRef { get; set; }
    }
}
