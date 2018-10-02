using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qed.Webhook.Api.Repository.Entities;
using LoanDocumentEnitty = Qed.Webhook.Api.Shared.Models.LoanDocumentEnitty;

namespace Qed.Webhook.Api.Repository.Interfaces
{
    public interface IEncompassDocumentRepository
    {
        Task<List<LoanDocumentEnitty>> GetDoucmentByLoanGuidAsync(Guid loanGuid);
        Task<DbResponse<EncompassDocumentEntity>> AddDocumentAsync(EncompassDocumentEntity addingDocument);
        Task<DbResponse<EncompassDocumentEntity>> UpdateDocumentAsync(EncompassDocumentEntity updatingDocument);
    }
}