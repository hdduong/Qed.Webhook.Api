using Newtonsoft.Json;

namespace Qed.Webhook.Service.Models.Responses
{
    public class GenericFailuareResponse
    {
        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }

        [JsonProperty(PropertyName = "details")]
        public string Details { get; set; }

        [JsonProperty(PropertyName = "error_code")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "fault")]
        public FaultDownload Fault { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
