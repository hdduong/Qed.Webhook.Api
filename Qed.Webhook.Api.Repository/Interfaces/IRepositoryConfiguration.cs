namespace Qed.Webhook.Api.Repository.Interfaces
{
    public interface IRepositoryConfiguration
    {
        string GetStagingConnectionString();
        string GetMagnusConnectionString();
    }
}