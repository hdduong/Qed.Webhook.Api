using System.Data;
using System.Data.SqlClient;
using Qed.Webhook.Api.Repository.Interfaces;

namespace Qed.Webhook.Api.Repository
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IRepositoryConfiguration _repositoryConfiguration;

        public DbConnectionFactory(IRepositoryConfiguration repositoryConfiguration)
        {
            _repositoryConfiguration = repositoryConfiguration;
        }

        public IDbConnection CreateStagingConnection()
        {
            var conn = new SqlConnection(_repositoryConfiguration.GetStagingConnectionString());
            conn.Open();
            return conn;
        }

        public IDbConnection CreateMagnusConnection()
        {
            var conn = new SqlConnection(_repositoryConfiguration.GetMagnusConnectionString());
            conn.Open();
            return conn;
        }
    }
}
