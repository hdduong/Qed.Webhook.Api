using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using NServiceBus;
using Qed.Webhook.Api.Shared.Bus.Document.Events;
using Qed.Webhook.Api.Shared.Helpers;
using Qed.Webhook.JobMaster.Interfaces;

namespace Qed.Webhook.JobMaster.Handlers
{
    public class DocumentSagaUpdatedHandler : IHandleMessages<DocumentMainMessage>
    {
        Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IJobMasterService _jobMasterService;

        public DocumentSagaUpdatedHandler(IJobMasterService jobMasterService)
        {
            _jobMasterService = jobMasterService;
        }

        public async Task Handle(DocumentMainMessage message, IMessageHandlerContext context)
        {
            try
            {
                _logger.Info(
                    $"{message.CorrelationId} - JobMaster.DocumentMainMessage.Handle Start - {JsonConvert.SerializeObject(message)}");

                await _jobMasterService.UpdateEncompassEvent(message).ConfigureAwait(false);
                _logger.Info(
                    $"{message.CorrelationId} - JobMaster.DocumentMainMessage.Handle UpdateEncompassEvent done");

                // reply to saga that all done
                var updatededEncompassEventMsg = message.ToDocumentMainMessageResponse();
                await context.Reply(updatededEncompassEventMsg);

                _logger.Info($"{message.CorrelationId} - JobMaster.DocumentMainMessage.Handle End");
            }
            catch (Exception ex)
            {
                _logger.Error($"{message.CorrelationId} {ex}");
                throw;
            }
            
        }
    }
}
