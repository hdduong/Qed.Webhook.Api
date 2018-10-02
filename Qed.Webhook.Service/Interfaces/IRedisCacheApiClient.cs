using System;
using System.Threading.Tasks;

namespace Qed.Webhook.Service.Interfaces
{
    public interface IRedisCacheApiClient
    {
        Task<int> GetWorkerIdAsync(Guid resourceId);
        Task<bool> SetWorkerIdAsync(Guid resourceId, int workerId);
        Task<string> GetAttachmentByLoanGuidAsync(Guid resourceId);
        Task<bool> SetLoanAttachmentAsync(Guid resourceId, string attachmentTxt);
    }
}