using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using NServiceBus;
using Qed.Document.Worker.Interfaces;
using Qed.Webhook.Api.Shared.Bus.Document;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Service.Helpers;
using Qed.Webhook.Service.Models.Requests.Document;

namespace Qed.Document.Worker.Handlers
{
    public class DocumentEventHandler : IHandleMessages<DocumentWorkerEvent>
    {
        Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDocumentService _documentService;

        public DocumentEventHandler(IDocumentService documentService)
        {
            _documentService = documentService;
        }


        public async Task Handle(DocumentWorkerEvent message, IMessageHandlerContext context)
        {
            try
            {
                _logger.Info($"{message.CorrelationId} - DocumentWorker.Handle Start - {JsonConvert.SerializeObject(message)}");

                var processDocumentResults = await _documentService.ProcessDocumentAsync(new ProcessDocumentRequest
                {
                    LoanGuid = message.ResourceId
                }).ConfigureAwait(false);

                _logger.Info($"{message.CorrelationId} - Document Processing results - {JsonConvert.SerializeObject(processDocumentResults)} ");

                var doucmentEventResposne = processDocumentResults.ToDocumentEventResponse();
                doucmentEventResposne.CmdId = message.CmdId;
                doucmentEventResposne.CorrelationId = message.CorrelationId;
                doucmentEventResposne.Id = message.Id;
                doucmentEventResposne.StatusId = message.StatusId;
                doucmentEventResposne.EventId = message.EventId;
                doucmentEventResposne.ResourceId = message.ResourceId;
                doucmentEventResposne.InstanceId = message.InstanceId;

                _logger.Info($"{message.CorrelationId} - Document Event Replied to {ConstantString.SagaEndpoint} - {JsonConvert.SerializeObject(doucmentEventResposne)} ");

                await context.Reply(doucmentEventResposne).ConfigureAwait(false);

                _logger.Info($"{message.CorrelationId} - DocumentWorker.Handle End ");
            }
            catch (Exception ex)
            {
                _logger.Error($"{message.CorrelationId} {ex}");
                throw;
            }
            
        }
    }
}
