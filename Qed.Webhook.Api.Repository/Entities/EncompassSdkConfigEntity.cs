namespace Qed.Webhook.Api.Repository.Entities
{
    public class EncompassSdkConfigEntity
    {
        public int Id { get; set; }
        public string Name { get; set;}
        public int SeqNum { get; set; }
        public string ServerUri { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public bool EncryptionBit { get; set; }
        public string ApiClientId { get; set; }
        public string ApiClientSecret { get; set; }
    }
}
