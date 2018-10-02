using Newtonsoft.Json;

namespace Qed.Webhook.Service.Models.Responses.AccessToken
{
    public class GetTokenFailureResponse
    {
        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }

        [JsonProperty(PropertyName = "details")]
        public string Details { get; set; }

        [JsonProperty(PropertyName = "error_code")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "error_description")]
        public string ErrorDescription { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}
