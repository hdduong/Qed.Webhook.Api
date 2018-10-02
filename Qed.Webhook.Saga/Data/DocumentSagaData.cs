using System;
using NServiceBus;

namespace Qed.Webhook.Saga.Data
{
    public class DocumentSagaData : IContainSagaData
    {
        // From IContainSagaData
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        // DocumentData
        public string CmdId { get; set; }
        public Guid EventId { get; set; }
        public Guid ResourceId { get; set; }
        public string InstanceId { get; set; }
        public int RowId { get; set; }
        public DateTimeOffset EnqueueDtTm { get; set; }
        public DateTimeOffset? PickupDtTm { get; set; }
        public Guid CorrelationId { get; set; }
        public int StatusId { get; set; }
        public int? ThrottleId { get; set; }
        public int WorkerId { get; set; }

        public DateTime SagaStartTimeUtc { get; set; }
        public bool IsEventDispatchedToDocumentWorkerBit { get; set; }
        public bool IsDocumentDownloadedBit { get; set; }
        public bool IsMainEventUpdatedBit { get; set; }
    }
}
