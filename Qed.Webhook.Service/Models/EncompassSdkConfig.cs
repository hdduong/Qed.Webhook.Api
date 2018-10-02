namespace Qed.Webhook.Service.Models
{
    public class EncompassSdkConfig
    {
        public string Name { get; set; }
        public int SeqNum { get; set; }
        public string ServerUri { get; set; }
        public string InstanceId { get; set; }
        public string UserId { get; set; }
        public string EncryptedPassword { get; set; }
        public string DecryptedPassword { get; set; }
        public bool EncryptionBit { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
