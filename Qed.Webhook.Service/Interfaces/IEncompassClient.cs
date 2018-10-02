using System.Threading.Tasks;
using Qed.Webhook.Service.Models.Requests.AccessToken;
using Qed.Webhook.Service.Models.Requests.Document;
using Qed.Webhook.Service.Models.Requests.Subcription;
using Qed.Webhook.Service.Models.Responses;
using Qed.Webhook.Service.Models.Responses.AccessToken;
using Qed.Webhook.Service.Models.Responses.Document;
using Qed.Webhook.Service.Models.Responses.Subcription;

namespace Qed.Webhook.Service.Interfaces
{
    public interface IEncompassClient
    {
        Task<ApiResponse<GetTokenResponse>> GetSecurityTokenAsync(GetTokenRequest request);
        Task<ApiResponse<GetAllSubscriptionResponse>> GetAllSubscriptionAsync(GetAllSubscriptionRequest request);
        Task<ApiResponse<UpdateSubscriptionResponse>> UpdateSubscriptionAsync(UpdateSubscriptionRequest request);
        Task<ApiResponse<AddSubscriptionResponse>> AdddSubscriptionAsync(AddSubscriptionRequest request);
        Task<ApiResponse<GetAttachmentResponse>> QueryDocumentAsync(GetAttachmentRequest request);
        Task<ApiResponse<DownloadAttachmentResponse>> DownloadAttachmentAsync(DownloadAttachmentRequest request);
    }
}