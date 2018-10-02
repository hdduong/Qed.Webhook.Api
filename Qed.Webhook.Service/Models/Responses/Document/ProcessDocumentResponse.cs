using System;
using System.Collections.Concurrent;
using Qed.Webhook.Api.Shared.Enums;

namespace Qed.Webhook.Service.Models.Responses.Document
{
    public class ProcessDocumentResponse
    {
        public bool SuccessBit { get; set; }
        public EventStatusEnum EventStatus { get; set; }
        public string EventReason { get; set; }
        public string ErrorMsgTxt { get; set; }
        public ConcurrentDictionary<Guid, DocumentErrorDtl> DocErrors { get; set; }

        public ProcessDocumentResponse()
        {
            SuccessBit = false;
            EventStatus = EventStatusEnum.Unknown;
            EventReason = string.Empty;
            ErrorMsgTxt = string.Empty;
            DocErrors = new ConcurrentDictionary<Guid, DocumentErrorDtl>();
        }
    }
}
