using System;
using Newtonsoft.Json;

namespace Qed.Webhook.Api.Shared.Models.Document
{
    public class Document
    {
        [JsonProperty("entityId")]
        public Guid EntityId { get; set; }

        [JsonProperty("entityType")]
        public string EntityType { get; set; }

        [JsonProperty("entityName")]
        public string EntityName { get; set; }

        [JsonProperty("entityUri")]
        public string EntityUri { get; set; }
    }
}
