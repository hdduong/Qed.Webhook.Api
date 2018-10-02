using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Qed.Webhook.Api.Repository.Entities;
using Qed.Webhook.Api.Repository.Entities.EncompassEventEntities;
using Qed.Webhook.Api.Repository.Interfaces;

namespace Qed.Webhook.Api.Repository.Repositories
{
    public class EncompassEventRepository : IEncompassEventRepository
    {
        private readonly IDbConnectionFactory _dbConnection;

        public EncompassEventRepository(IDbConnectionFactory dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> AddEventAsync(EnqueueEncompassEventEntity encompassEvent)
        {
            int insertedEventId;
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                using (IDbConnection connection = _dbConnection.CreateStagingConnection())
                {

                    var eventId = await connection.QueryFirstOrDefaultAsync<int>(
                        "[dbo].[AddWebhookEvent]", new
                        {
                            SrcTypeId = encompassEvent.WebhookEvent.SourceTypeId,
                            encompassEvent.WebhookEvent.EventDtTm
                        }, commandType: CommandType.StoredProcedure).ConfigureAwait(false);


                    await connection.QueryFirstOrDefaultAsync<int>(
                        "[dbo].[AddEncompassWebhookEventDtl]", new
                        {
                            Id = eventId,
                            encompassEvent.EventDetail.EventId,
                            encompassEvent.EventDetail.EventUtcDtTm,
                            encompassEvent.EventDetail.EventTypeId,
                            encompassEvent.EventDetail.InstanceId,
                            encompassEvent.EventDetail.UserId,
                            encompassEvent.EventDetail.ResourceTypeId,
                            encompassEvent.EventDetail.ResourceId,
                            encompassEvent.EventDetail.MsgTxt
                        }, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                    await connection.QueryFirstOrDefaultAsync<int>(
                        "[dbo].[AddWebhookEventQueue]", new
                        {
                            Id = eventId,
                            encompassEvent.EventQueue.CorrelationId,
                            encompassEvent.EventQueue.EnqueueDtTm,
                            encompassEvent.EventQueue.PickupDtTm,
                            encompassEvent.EventQueue.StatusId,
                            encompassEvent.EventQueue.ThrottleId,
                            encompassEvent.EventQueue.WorkerId,
                            encompassEvent.EventQueue.FinishDtTm,
                            encompassEvent.EventQueue.ErrorMsgTxt
                        }, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                    insertedEventId = eventId;
                }

                transaction.Complete();
            }
            return insertedEventId;
        }


        public async Task<int> RemoveEventAsync(int webhookEventId)
        {
            int affectedRow = 0;
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                using (IDbConnection connection = _dbConnection.CreateStagingConnection())
                {
                    affectedRow += await connection.ExecuteAsync(
                        @"delete from dbo.EncompassWebhookEventDtl where Id=@Id", new
                        {
                            ID = webhookEventId
                        }, commandType: CommandType.Text).ConfigureAwait(false);

                    affectedRow += await connection.ExecuteAsync(
                        @"delete from dbo.WebhookEventQueue where Id=@Id", new
                        {
                            ID = webhookEventId
                        }, commandType: CommandType.Text).ConfigureAwait(false);

                    affectedRow += await connection.ExecuteAsync(
                        @"delete from dbo.WebhookEvent where Id=@Id", new
                        {
                            ID = webhookEventId
                        }, commandType: CommandType.Text).ConfigureAwait(false);
                }

                transaction.Complete();
            }
            return affectedRow;
        }

        public async Task<List<int>> SelectInQueueEventAsync(int workerId, int queueStatusId)
        {
            List<int> result;

            using (IDbConnection connection = _dbConnection.CreateStagingConnection())
            {
                var ids = await connection.QueryAsync<int>(
                "[dbo].[EncompassEarliestResourceEventInQueue]", new
                {
                    WorkerId = workerId,
                    QueueStatusId = queueStatusId
                }, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                result = ids.ToList();
            }
            return result;
        }

        public async Task<int> ThrottlingEventAsync(string updatingIds, int throttleId, int throttleStatusId, DateTimeOffset pickupDateTime, DateTimeOffset finishDateTime)
        {
            int affectRows;

            using (IDbConnection connection = _dbConnection.CreateStagingConnection())
            {
                affectRows = await connection.ExecuteAsync(
                    "[dbo].[EncompassThrottlingEvent]", new
                    {
                        UpdatingIds = updatingIds,
                        ThrottleId = throttleId,
                        ThrottleStatusId = throttleStatusId,
                        PickUpDtTm = pickupDateTime,
                        FinishDtTm = finishDateTime
                    }, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            }

            return affectRows;
        }

        public async Task<WebhookEventQueueEntity> SelectEventAsync(int id)
        {
            WebhookEventQueueEntity webhookEvent;

            using (IDbConnection connection = _dbConnection.CreateStagingConnection())
            {
                var webhookEvents = await connection.QueryAsync<WebhookEventQueueEntity>(
                   $@"select Id
                    , [CorrelationId]
                    , [EnqueueDtTm]
                    , [PickupDtTm]
                    , [StatusId]
                    , [ThrottleId]
                    , [WorkerId]
                    , [FinishDtTm]
                    , [ErrorMsgTxt]
                    from dbo.WebhookEventQueue where Id=@Id", new
                   {
                       Id = id
                   }, commandType: CommandType.Text).ConfigureAwait(false);

                webhookEvent = webhookEvents.FirstOrDefault();
            }

            return webhookEvent;
        }

        public async Task<int> UpdateEventAsync(WebhookEventQueueEntity updatingEvent)
        {
            int affectRows;
            using (IDbConnection connection = _dbConnection.CreateStagingConnection())
            {
                affectRows = await connection.ExecuteAsync(
                    $@"update dbo.WebhookEventQueue
                    set [CorrelationId] = @CorrelationId 
                    , [EnqueueDtTm] = @EnqueueDtTm
                    , [PickupDtTm] = @PickupDtTm
                    , [StatusId] = @StatusId
                    , [ThrottleId] = @ThrottleId
                    , [WorkerId] = @WorkerId
                    , [FinishDtTm] = @FinishDtTm
                    , [ErrorMsgTxt] = @ErrorMsgTxt
                    where Id=@Id", new
                    {
                        updatingEvent.CorrelationId,
                        updatingEvent.EnqueueDtTm,
                        updatingEvent.PickupDtTm,
                        updatingEvent.StatusId,
                        updatingEvent.ThrottleId,
                        updatingEvent.WorkerId,
                        updatingEvent.FinishDtTm,
                        updatingEvent.ErrorMsgTxt,
                        updatingEvent.Id
                    }, commandType: CommandType.Text).ConfigureAwait(false);

            }
            return affectRows;
        }

        public async Task<int> PerformEventThrottlingAsync(string updatingIds, int throttleId, int throttleStatusId,
            DateTimeOffset pickupDateTime, DateTimeOffset finishDateTime,
            WebhookEventQueueEntity updatingEvent)
        {
            int throttledEvents;
            int processingEvent;

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                throttledEvents =
                    await ThrottlingEventAsync(updatingIds, throttleId, throttleStatusId, pickupDateTime,
                        finishDateTime).ConfigureAwait(false);

                processingEvent = await UpdateEventAsync(updatingEvent).ConfigureAwait(false);

                transaction.Complete();
            }

            return throttledEvents + processingEvent;
        }

        public async Task<EncompassWebhookEventQueueEnitty> GetEncompassWebhookEventDetailAsync(int id)
        {
            EncompassWebhookEventQueueEnitty result;
            using (IDbConnection connection = _dbConnection.CreateStagingConnection())
            {
                var eventDetails = await connection.QueryAsync<EncompassWebhookEventQueueEnitty>(
                    $@"select ewed.Id
                    , ewed.EventId
                    , ewed.EventUtcDtTm
                    , ewed.EventTypeId
                    , ewed.InstanceId	
                    , ewed.UserId
                    , ewed.ResourceTypeId
                    , ewed.ResourceId
                    , ewed.MsgTxt
                    , weq.CorrelationId
                    , weq.EnqueueDtTm
                    , weq.PickupDtTm
                    , weq.StatusId
                    , weq.ThrottleId
                    , weq.WorkerId
                    , weq.FinishDtTm
                    , weq.ErrorMsgTxt
                    from dbo.EncompassWebhookEventDtl  ewed
                    inner join dbo.WebhookEventQueue weq on ewed.Id = weq.Id
                    where ewed.Id=@Id", new
                    {
                       Id = id
                    }, commandType: CommandType.Text).ConfigureAwait(false);
                result = eventDetails.FirstOrDefault();
            }
            return result;
        }
    }
}
