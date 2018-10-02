using System;

namespace Qed.Webhook.Service.Models.Requests.Document
{
    public class GetAttachmentRequest
    {
        public string AccessToken { get; set; }
        public Guid LoanGuid { get; set; }
    }
}
