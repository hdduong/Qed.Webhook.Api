using System.Data;

namespace Qed.Webhook.Api.Repository.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateStagingConnection();
        IDbConnection CreateMagnusConnection();
    }
}