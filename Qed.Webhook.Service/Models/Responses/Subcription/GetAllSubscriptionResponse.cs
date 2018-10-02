using System.Collections.Generic;

namespace Qed.Webhook.Service.Models.Responses.Subcription
{
    public class GetAllSubscriptionResponse
    {
        public List<SubscriptionSuccessResponse> SuccessResponse { get; set; }
        public GenericFailuareResponse FailureResponse { get; set; }
    }
}
