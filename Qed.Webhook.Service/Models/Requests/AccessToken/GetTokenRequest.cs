namespace Qed.Webhook.Service.Models.Requests.AccessToken
{
    public class GetTokenRequest
    {
        public string SmartClientUserName { get; set; }
        public string SmartClientPassword { get; set; }
        public string InstanceId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
