using Qed.Webhook.Api.Repository.Entities.EncompassEventEntities;

namespace Qed.Webhook.Api.Repository.Entities.EncompassEventEntities
{
    public class EnqueueEncompassEventEntity
    {
        public WebhookEventEntity WebhookEvent { get; set; }
        public EncompassEventDetail EventDetail { get; set; }
        public WebhookEventQueueEntity EventQueue { get; set; }

        public EnqueueEncompassEventEntity()
        {
            WebhookEvent = new WebhookEventEntity();
            EventDetail = new EncompassEventDetail();
            EventQueue = new WebhookEventQueueEntity();
        }
    }
}
