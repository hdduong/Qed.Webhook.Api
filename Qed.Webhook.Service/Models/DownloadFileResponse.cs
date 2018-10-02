using Qed.Webhook.Service.Models.Responses;

namespace Qed.Webhook.Service.Models
{
    public class DownloadFileResponse
    {
        public bool IsSuccessDownload { get; set; }
        public GenericFailuareResponse ErrorResponse { get; set; }

        public DownloadFileResponse()
        {
            IsSuccessDownload = false;
            ErrorResponse = new GenericFailuareResponse();
        }
    }
}
