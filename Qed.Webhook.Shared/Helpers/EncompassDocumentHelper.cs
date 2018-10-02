using System;
using System.Collections.Generic;
using Qed.Webhook.Api.Shared.Models;

namespace Qed.Webhook.Api.Shared.Helpers
{
    public class EncompassDocumentHelper
    {
        public static Dictionary<Guid, HashSet<Guid>> GroupDoucmentByLoanId(List<LoanDocumentEnitty> documentsDownloaded)
        {
            var loanDocuments = new Dictionary<Guid, HashSet<Guid>>();

            foreach (var documentDownload in documentsDownloaded)
            {
                var loanId = documentDownload.LoanGuid;
                var documentId = documentDownload.DocumentGuid;

                if (loanDocuments.ContainsKey(loanId))
                {
                    var exisingDocuments = loanDocuments[loanId];
                    if (!exisingDocuments.Contains(documentId))
                    {
                        exisingDocuments.Add(documentId);
                    }
                }
                else
                {
                    loanDocuments.Add(loanId, new HashSet<Guid>
                    {
                        documentId
                    });
                }
            }
            return loanDocuments;
        }
    }
}
