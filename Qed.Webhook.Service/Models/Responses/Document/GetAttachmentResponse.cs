using Qed.Webhook.Api.Shared.Models.Document;
using System.Collections.Generic;

namespace Qed.Webhook.Service.Models.Responses.Document
{
    public class GetAttachmentResponse
    {
        public GenericFailuareResponse FailureResponse { get; set; }
        public IList<Attachment> SuccessResponse { get; set; }
    }
}
