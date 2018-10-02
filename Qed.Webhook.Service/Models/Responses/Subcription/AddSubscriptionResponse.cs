using Newtonsoft.Json;

namespace Qed.Webhook.Service.Models.Responses.Subcription
{
    public class AddSubscriptionResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
