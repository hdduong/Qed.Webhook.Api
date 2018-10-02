using Newtonsoft.Json;

namespace Qed.Webhook.Service.Models.Responses
{
    public class FaultDownload
    {
        [JsonProperty("details")]
        public string Details { get; set; }
    }
}
