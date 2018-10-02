using System.Collections.Concurrent;
using Qed.Webhook.Api.Shared.Models.WebhookNotification.Encompass;

namespace Qed.Webhook.Service.Models.Requests.Event
{
    public class AddEncompassEventRequest
    {
        public Notification EncompassEvent { get; set; }
        public ConcurrentDictionary<string, int> EventSource { get; set; }
        public ConcurrentDictionary<string, int> EventStatus { get; set; }
        public ConcurrentDictionary<string, int> EventType { get; set; }
        public ConcurrentDictionary<string, int> EventResourceType { get; set; }
    }
}
