namespace Qed.Webhook.Service.Models.Responses.AccessToken
{
    public class GetTokenResponse
    {
        public GetTokenSuccessResponse SuccessResponse { get; set; }
        public GetTokenFailureResponse FailureResponse { get; set; }
    }
}
