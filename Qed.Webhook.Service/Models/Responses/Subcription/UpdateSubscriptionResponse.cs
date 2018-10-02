using Newtonsoft.Json;

namespace Qed.Webhook.Service.Models.Responses.Subcription
{
    public class UpdateSubscriptionResponse
    {
        [JsonProperty("message")]
        public string Message{ get; set; }
    }
}
