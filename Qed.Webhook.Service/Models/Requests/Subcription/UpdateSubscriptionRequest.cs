namespace Qed.Webhook.Service.Models.Requests.Subcription
{
    public class UpdateSubscriptionRequest
    {
        public string AccessToken { get; set; }
        public string SubscriptionId { get; set; }
        public EncompassSubscription UpdatingSubscription { get; set; }
    }
}
