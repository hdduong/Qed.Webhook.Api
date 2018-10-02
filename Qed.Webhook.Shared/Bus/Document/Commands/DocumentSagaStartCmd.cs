using System;
using NServiceBus;

namespace Qed.Webhook.Api.Shared.Bus.Document
{
    
    public class DocumentSagaStartCmd : ICommand
    {
        public string CmdId { get; set; }
        public Guid EventId { get; set; }
        public Guid ResourceId { get; set; }
        public string InstanceId { get; set; }
        public int Id { get; set; }
        public DateTimeOffset EnqueueDtTm { get; set; }
        public DateTimeOffset? PickupDtTm { get; set; }
        public Guid CorrelationId { get; set; }
        public int StatusId { get; set; }
        public int? ThrottleId { get; set; }
        public int WorkerId { get; set; }    
    }
}
