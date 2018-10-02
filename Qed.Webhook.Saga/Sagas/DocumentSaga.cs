using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using Qed.Webhook.Api.Shared.Bus.Document;
using Qed.Webhook.Api.Shared.Bus.Document.Events;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Api.Shared.Helpers;
using Qed.Webhook.Saga.Data;

namespace Qed.Webhook.Saga.Sagas
{
    public class DocumentSaga :
        SqlSaga<DocumentSagaData>,
        IAmStartedByMessages<DocumentSagaStartCmd>,
        IHandleMessages<DocumentWorkerEventResponse>,
        IHandleMessages<DocumentMainMessageResponse>,
        IHandleTimeouts<DocumentTimeout>

    {
        Logger _logger = LogManager.GetCurrentClassLogger();

        protected override void ConfigureMapping(IMessagePropertyMapper mapper)
        {
            mapper.ConfigureMapping<DocumentSagaStartCmd>(_ => _.CmdId);
            mapper.ConfigureMapping<DocumentWorkerEventResponse>(_ => _.CmdId);
            mapper.ConfigureMapping<DocumentMainMessageResponse>(_ => _.CmdId);
        }

        protected override string CorrelationPropertyName => nameof(DocumentSagaData.CmdId);

        public async Task Handle(DocumentSagaStartCmd message, IMessageHandlerContext context)
        {
            _logger.Info($"{message.CorrelationId} - DocumentSaga.StartCmd.Handle Start - {JsonConvert.SerializeObject(message)}");

            Data.SagaStartTimeUtc = DateTime.UtcNow;
            Data.EventId = message.EventId;
            Data.ResourceId = message.ResourceId;
            Data.InstanceId = message.InstanceId;
            Data.RowId = message.Id;
            Data.EnqueueDtTm = message.EnqueueDtTm;
            Data.PickupDtTm = message.PickupDtTm;
            Data.CorrelationId = message.CorrelationId;
            Data.StatusId = message.StatusId;
            Data.ThrottleId = message.ThrottleId;
            Data.WorkerId = message.WorkerId;

            // need to send message to document worker
            var documentEvent = message.ToDocumentWorkerEvent();
            if (!Data.IsEventDispatchedToDocumentWorkerBit)
            {
                await context.Publish(documentEvent).ConfigureAwait(false);
                Data.IsEventDispatchedToDocumentWorkerBit = true;
            }

            _logger.Info($"{message.CorrelationId} - DocumentSaga.StartCmd.Handle RequestTimeout {nameof(ConstantValue.DefaultSagaTimeoutInSeconds)} with values {ConstantValue.DefaultSagaTimeoutInSeconds}");
            await RequestTimeout<DocumentTimeout>(context, TimeSpan.FromSeconds(ConstantValue.DefaultSagaTimeoutInSeconds)).ConfigureAwait(false);

            _logger.Info($"{message.CorrelationId} - DocumentSaga.StartCmd.Handle End");
        }

        public async Task Timeout(DocumentTimeout state, IMessageHandlerContext context)
        {
            if (!Data.IsMainEventUpdatedBit && DateTime.UtcNow <
                Data.SagaStartTimeUtc.AddMinutes(ConstantValue.MaximumSagaTimeoutInMinutes))
            {
                await RequestTimeout<DocumentTimeout>(context, TimeSpan.FromSeconds(ConstantValue.DefaultSagaTimeoutInSeconds)).ConfigureAwait(false);
                return;
            }
            MarkAsComplete();
        }

        public async Task Handle(DocumentWorkerEventResponse message, IMessageHandlerContext context)
        {
            _logger.Info($"{message.CorrelationId} - DocumentSaga.DocumentWorkerResponse.Handle Start - {JsonConvert.SerializeObject(message)}");

            Data.IsDocumentDownloadedBit = true;

            var documentMainEvent = message.ToDouDocumentMainMessage();
            documentMainEvent.EnqueueDtTm = Data.EnqueueDtTm;
            documentMainEvent.PickupDtTm = Data.PickupDtTm;
            documentMainEvent.ThrottleId = Data.ThrottleId;
            documentMainEvent.FinishDtTm = DateTimeOffset.Now;
            documentMainEvent.WorkerId = Data.WorkerId;

            if (!Data.IsMainEventUpdatedBit)
            {
                await ReplyToOriginator(context, documentMainEvent).ConfigureAwait(false);
            }

            _logger.Info($"{message.CorrelationId} - DocumentSaga.DocumentWorkerResponse.Handle RequestTimeout {nameof(ConstantValue.DefaultSagaTimeoutInSeconds)} with values {ConstantValue.DefaultSagaTimeoutInSeconds}");
            await RequestTimeout<DocumentTimeout>(context, TimeSpan.FromSeconds(ConstantValue.DefaultSagaTimeoutInSeconds)).ConfigureAwait(false);


            _logger.Info($"{message.CorrelationId} - DocumentSaga.DocumentWorkerResponse.Handle End");
        }

        public Task Handle(DocumentMainMessageResponse message, IMessageHandlerContext context)
        {
            _logger.Info($"{message.CorrelationId} - DocumentSaga.JobMasterResponse.Handle Start - {JsonConvert.SerializeObject(message)}");

            Data.IsMainEventUpdatedBit = true;
            MarkAsComplete();

            _logger.Info($"{message.CorrelationId} - DocumentSaga.JobMasterResponse.Handle End");
            return Task.CompletedTask;
        }
    }
}
