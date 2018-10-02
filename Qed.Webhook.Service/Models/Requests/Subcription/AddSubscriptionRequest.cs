namespace Qed.Webhook.Service.Models.Requests.Subcription
{
    public class AddSubscriptionRequest
    {
        public string AccessToken { get; set; }
        public EncompassSubscription AddingSubscription { get; set; }
    }
}
