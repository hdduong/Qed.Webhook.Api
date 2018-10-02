using System.Threading.Tasks;
using Qed.Webhook.Api.Repository.Entities;
using Qed.Webhook.Api.Shared.Bus.Document;
using Qed.Webhook.Api.Shared.Bus.Document.Events;

namespace Qed.Webhook.JobMaster.Interfaces
{
    public interface IJobMasterService
    {
        Task<WebhookEventQueueEntity> GetProcessingEventAsync();
        Task<DocumentSagaStartCmd> GetDocumentDownloadCmdAsync(int id);
        Task<int> UpdateEncompassEvent(DocumentMainMessage mainMessage);
    }
}