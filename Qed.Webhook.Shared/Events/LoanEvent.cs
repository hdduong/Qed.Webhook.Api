using System;

namespace Qed.Webhook.Api.Shared.Events
{
    public class LoanEvent
    {
        public Guid EventId { get; set; }
        public DateTimeOffset EventDateTime { get; set; }
        public string EventType { get; set; }
        public string EventAction { get; set; }
        public string InstanceId { get; set; }
        public Guid LoanId { get; set; }
        public string UserId { get; set; }
    }
}
