using Qed.Webhook.Api.Repository.Interfaces;

namespace Qed.Webhook.Api.Repository
{
    public class RepositoryConfiguration : IRepositoryConfiguration
    {
        private readonly string _stagingConnectionString;
        private readonly string _magnusConnectionString;

        public RepositoryConfiguration(string queueConnectionString, string magnusConnectionString)
        {
            _stagingConnectionString = queueConnectionString;
            _magnusConnectionString = magnusConnectionString;
        }

        public string GetStagingConnectionString()
        {
            return _stagingConnectionString;
        }

        public string GetMagnusConnectionString()
        {
            return _magnusConnectionString;
        }
    }
}
