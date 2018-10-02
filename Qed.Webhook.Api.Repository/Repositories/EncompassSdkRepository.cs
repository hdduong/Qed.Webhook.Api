using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Qed.Webhook.Api.Repository.Entities;
using Qed.Webhook.Api.Repository.Interfaces;
using Qed.Webhook.Api.Shared.Constants;

namespace Qed.Webhook.Api.Repository.Repositories
{
    public class EncompassSdkRepository : IEncompassSdkRepository
    {
        private readonly IDbConnectionFactory _dbConnection;

        public EncompassSdkRepository(IDbConnectionFactory dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<EncompassSdkConfigEntity> GetEncomopassCredentialAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = ConstantString.DefaultEncompassSdkName;

            EncompassSdkConfigEntity sdkConfigEntity;
            using (IDbConnection connection = _dbConnection.CreateMagnusConnection())
            {
                var sdkConfigEntities = await connection.QueryAsync<EncompassSdkConfigEntity>(
                    $@"select Id
                    , [Name]
                    , [SeqNum]
                    , [ServerUri]
                    , [UserId]
                    , [Password]
                    , [EncryptionBit]  
                    , [ApiClientId]
                    , [ApiClientSecret]
                    from dbo.EncompassSdkConfigActive where Name=@Name", new
                    {
                        Name = name
                    }, commandType: CommandType.Text).ConfigureAwait(false);

                sdkConfigEntity = sdkConfigEntities.ToList().FirstOrDefault();
            }
            return sdkConfigEntity;
        }
    }
}
