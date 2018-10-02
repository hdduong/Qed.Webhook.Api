using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Qed.Webhook.Service.Models.Responses.Subcription
{
    public class SubscriptionSuccessResponse
    {
        [JsonProperty("subscriptionId")]
        public Guid SubscriptionId { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("objectUrn")]
        public string ObjectUrn { get; set; }

        [JsonProperty("events")]
        public List<string> Events { get; set; }

        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }

        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty("resource")]
        public string Resource { get; set; }
    }
}
