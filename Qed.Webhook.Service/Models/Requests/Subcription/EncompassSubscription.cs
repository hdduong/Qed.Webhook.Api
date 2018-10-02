using System.Collections.Generic;
using Newtonsoft.Json;

namespace Qed.Webhook.Service.Models.Requests.Subcription
{
    public class EncompassSubscription
    {
        [JsonProperty("events")]
        public List<string> Events { get; set; }

        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty("resource")]
        public string Resource { get; set; }
    }
}
