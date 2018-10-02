using Newtonsoft.Json;

namespace Qed.Webhook.Service.Models.Responses.Document
{
    public class GetAttachmentUrlSuccessResponse
    {
        [JsonProperty(PropertyName = "mediaUrl")]
        public string MediaUrl { get; set; }
    }
}
