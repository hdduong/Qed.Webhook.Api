using System;

namespace Qed.Webhook.Api.Shared.Models
{
    public class LoanDocumentEnitty
    {
        public Guid LoanGuid { get; set; }
        public Guid DocumentGuid { get; set; }
        public DateTimeOffset LatestDowloadDtTm { get; set; }
    }
}
