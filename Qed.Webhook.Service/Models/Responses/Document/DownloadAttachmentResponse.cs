using System.IO;
using Qed.Webhook.Api.Repository.Entities;
using Qed.Webhook.Api.Shared.Models.Document;

namespace Qed.Webhook.Service.Models.Responses.Document
{
    public class DownloadAttachmentResponse
    {
        public bool IsSuccessBit { get; set; }
        public string LoanId { get; set; }        
        public string MediaUrl { get; set; }
        public string DownloadFullPath { get; set; }
        public Attachment Attachment { get; set; }
        public GenericFailuareResponse FailureResponse { get; set; }
        public EncompassDocumentEntity EncompassDocument { get; set; }

        public DownloadAttachmentResponse()
        {
            IsSuccessBit = false;
            LoanId = string.Empty;
            MediaUrl = string.Empty;
            DownloadFullPath = string.Empty;
            Attachment  = new Attachment();
            FailureResponse = new GenericFailuareResponse();
            EncompassDocument = new EncompassDocumentEntity();
        }
    }
}
