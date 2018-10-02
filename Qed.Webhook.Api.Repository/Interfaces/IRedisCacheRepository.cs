using System.Collections.Generic;
using System.Threading.Tasks;
using Qed.Webhook.Api.Shared.Models;

namespace Qed.Webhook.Api.Repository.Interfaces
{
    public interface IRedisCacheRepository
    {
        Task<List<LoanDocumentEnitty>> LoadDownloadDocument();
    }
}