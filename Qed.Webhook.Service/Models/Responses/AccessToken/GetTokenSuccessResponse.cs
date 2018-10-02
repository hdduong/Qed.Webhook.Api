using Newtonsoft.Json;

namespace Qed.Webhook.Service.Models.Responses.AccessToken
{
    public class GetTokenSuccessResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }
    }
}
