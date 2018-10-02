using Qed.Webhook.Api.Repository.Entities;

namespace Qed.Webhook.Service.Models.Requests.Document
{
    public class DownloadAttachmentRequest
    {
        public string LoanId { get; set; }
        public string AccessToken { get; set; }
        public EncompassDocumentEntity EncompassDocument { get; set; }
        public string DownloadFullPath { get; set; }
    }
}
