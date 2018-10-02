using Qed.Webhook.Service.Interfaces;
using IDocumentWorkerConfiguration = Qed.Document.Worker.Interfaces.IDocumentWorkerConfiguration;

namespace Qed.Document.Worker.Configurations
{
    public class DocumentWorkerConfiguration : IDocumentWorkerConfiguration
    {
        private readonly string _documentDownloadPath;

        public DocumentWorkerConfiguration(string documentDownloadPath)
        {
            _documentDownloadPath = documentDownloadPath;
        }

        public string GetDownloadPath()
        {
            return _documentDownloadPath;
        }


    }
}