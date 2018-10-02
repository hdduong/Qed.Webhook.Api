using System;
using NServiceBus;

namespace Qed.Webhook.Api.Shared.Bus.Document
{
    public class DocumentWorkerEvent : IEvent
    {
        public string CmdId { get; set; }
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
        public int StatusId { get; set; }
        public Guid EventId { get; set; }
        public Guid ResourceId { get; set; }
        public string InstanceId { get; set; }
    }
}
