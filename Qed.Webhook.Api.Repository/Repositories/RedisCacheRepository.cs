using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qed.Webhook.Api.Repository.Interfaces;
using Qed.Webhook.Api.Shared.Models;

namespace Qed.Webhook.Api.Repository.Repositories
{
    public class RedisCacheRepository : IRedisCacheRepository
    {
        private readonly IDbConnectionFactory _dbConnection;

        public RedisCacheRepository(IDbConnectionFactory dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<LoanDocumentEnitty>> LoadDownloadDocument()
        {
            List<LoanDocumentEnitty> result;
            using (IDbConnection connection = _dbConnection.CreateStagingConnection())
            {
                var documents = await connection.QueryAsync<LoanDocumentEnitty>(
                    "[dbo].[LoadDocumentDownloaded]", commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                result = documents.ToList();
            }

            return result;
        }
    }
}
