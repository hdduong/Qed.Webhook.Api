using Qed.Webhook.Api.Shared.Models.Document;

namespace Qed.Webhook.Service.Models.Responses.Document
{
    public class GetAttachmentUrlResponse
    {
        public GenericFailuareResponse FailureResponse { get; set; }
        public Attachment Attachment { get; set; }
        public GetAttachmentUrlSuccessResponse SuccessResponse { get; set; }
    }
}
