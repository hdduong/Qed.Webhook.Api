using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qed.Webhook.Service.Models.Requests.Document;
using Qed.Webhook.Service.Models.Responses.Document;

namespace Qed.Document.Worker.Interfaces
{
    public interface IDocumentService
    {
        Task<ProcessDocumentResponse> ProcessDocumentAsync(ProcessDocumentRequest request);
        Task<HashSet<Guid>> GetPersistedDocumentAsync(Guid loanGuid);
    }
}