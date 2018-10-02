using System;
using NServiceBus;

namespace Qed.Webhook.Api.Shared.Bus.Document.Events
{
    public class DocumentMainMessageResponse : IMessage
    {
        public string CmdId { get; set; }
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
