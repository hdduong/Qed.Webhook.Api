using System;
using System.Threading.Tasks;

namespace Qed.Webhook.RedisCache.Api.Interfaces
{
    public interface IRedisCacheService
    {
        int GetWorkerIdForJob(Guid resourceId);
        bool SetWorkerIdForJob(Guid resourceId, int workerId);
        string GetAttachmentForLoan(Guid resourceId);
        bool SetAttachmentForLoan(Guid resourceId, string attachments);
        Task<bool> BootstrapDocumentAsync();
    }
}