using System;
using Newtonsoft.Json;

namespace Qed.Webhook.Api.Shared.Models.Document
{
    public class Attachment
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("attachmentId")]
        public string AttachmentId { get; set; }

        [JsonProperty("document")]
        public Document Document { get; set; }

        [JsonProperty("dateCreated")]
        public DateTimeOffset DateCreated { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdByName")]
        public string CreatedByName { get; set; }

        [JsonProperty("isRemoved")]
        public bool IsRemoved { get; set; }

        [JsonProperty("attachmentType")]
        public long AttachmentType { get; set; }

        [JsonProperty("fileSize")]
        public long FileSize { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("rotation")]
        public long Rotation { get; set; }
    }
}
