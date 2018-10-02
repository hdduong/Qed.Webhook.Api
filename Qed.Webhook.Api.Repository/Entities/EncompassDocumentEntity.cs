using System;
using Qed.Webhook.Api.Shared.Enums;

namespace Qed.Webhook.Api.Repository.Entities
{
    public class EncompassDocumentEntity
    {
        public int Id { get; set; }
        public Guid LoanGuid { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentTypeName { get; set; }
        public DateTimeOffset DocumentDtTm { get; set; }
        public Guid DocumentGuid { get; set; }
        public string DocumentId { get; set; }
        public string AddedById { get; set; }
        public string AddedByName { get; set; }
        public string FileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public bool? DownloadSucceededBit { get; set; }
        public string DownloadFileFullPath { get; set; }
        public DateTimeOffset? StartDownloadDtTm { get; set; }
        public DateTimeOffset? EndDownloadDtTm { get; set; }
        public bool? IsActiveBit { get; set; }
        public DocumentStatusEnum DownloadStatusTypeId { get; set; }
        public string ErrorMsgTxt { get; set; }
    }
}
