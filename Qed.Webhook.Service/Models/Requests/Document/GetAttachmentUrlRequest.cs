using System;
using Qed.Webhook.Api.Shared.Models.Document;

namespace Qed.Webhook.Service.Models.Requests.Document
{
    public class GetAttachmentUrlRequest
    {
        public string AccessToken { get; set; }
        public string LoanId { get; set; }
        public string AttachmentId { get; set; }
    }
}
