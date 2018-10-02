using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qed.Webhook.Api.Repository.Entities;
using Qed.Webhook.Api.Repository.Entities.DictionaryEntities;
using Qed.Webhook.Api.Repository.Interfaces;

namespace Qed.Webhook.Api.Repository.Repositories
{
    public class EncompassWebhookDictionaryDataRepository : IEncompassWebhookDictionaryDataRepository
    {
        private readonly IDbConnectionFactory _dbConnection;

        public EncompassWebhookDictionaryDataRepository(IDbConnectionFactory dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<WebhookEventStatusEntity>> GetEventStatusAsync()
        {
            List<WebhookEventStatusEntity> webhookStatuses;
            using (IDbConnection connection = _dbConnection.CreateStagingConnection())
            {
                var entities = await connection.QueryAsync<WebhookEventStatusEntity>(
                    $@"select [WebhookEventStatusId]
                    , [WebhookEventStatusName]
                    , [DescTxt]
                    from dbo.WebhookEventStatus ", commandType: CommandType.Text).ConfigureAwait(false);

                webhookStatuses = entities.ToList();
            }
            return webhookStatuses;
        }

        public async Task<List<WebhookEventSourceEntity>> GetEventSourceAsync()
        {
            List<WebhookEventSourceEntity> webhookSources;
            using (IDbConnection connection = _dbConnection.CreateStagingConnection())
            {
                var entities = await connection.QueryAsync<WebhookEventSourceEntity>(
                    $@"select [WebhookEventSrcId]
                    , [WebhookEventSrcName]
                    , [DescTxt]
                    from dbo.WebhookEventSrc ", commandType: CommandType.Text).ConfigureAwait(false);

                webhookSources = entities.ToList();
            }
            return webhookSources;
        }

        public async Task<List<WebhookEventTypeEntity>> GetEventTypeAsync()
        {
            List<WebhookEventTypeEntity> eventTypes;
            using (IDbConnection connection = _dbConnection.CreateStagingConnection())
            {
                var entities = await connection.QueryAsync<WebhookEventTypeEntity>(
                    $@"select [WebhookEventTypeId]
                    , [WebhookEventTypeName]
                    , [DescTxt]
                    from dbo.WebhookEventType ", commandType: CommandType.Text).ConfigureAwait(false);

                eventTypes = entities.ToList();
            }
            return eventTypes;
        }

        public async Task<List<WebhookResourceTypeEntity>> GetResourceTypeAsync()
        {
            List<WebhookResourceTypeEntity> resourceTypes;
            using (IDbConnection connection = _dbConnection.CreateStagingConnection())
            {
                var entities = await connection.QueryAsync<WebhookResourceTypeEntity>(
                    $@"select [WebhookResrcTypeId]
                    , [WebhookResrcTypeName]
                    , [DescTxt]
                    from dbo.WebhookResrcType ", commandType: CommandType.Text).ConfigureAwait(false);

                resourceTypes = entities.ToList();
            }
            return resourceTypes;
        }

        public async Task<List<DocumentStatusTypeEntity>> GetDocumentStatusAsync()
        {
            List<DocumentStatusTypeEntity> downloadStatuses;
            using (IDbConnection connection = _dbConnection.CreateStagingConnection())
            {
                var entities = await connection.QueryAsync<DocumentStatusTypeEntity>(
                    $@"select [DownloadStatusTypeId]
                    , [DownloadStatusTypeName]
                    , [DescTxt]
                    from dbo.DownloadStatusType ", commandType: CommandType.Text).ConfigureAwait(false);

                downloadStatuses = entities.ToList();
            }

            return downloadStatuses;
        }
    }
}
