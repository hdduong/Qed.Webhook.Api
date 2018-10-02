using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Api.Shared.Helpers;
using Qed.Webhook.Api.Shared.Models.Document;
using Qed.Webhook.Service.Helpers;
using Qed.Webhook.Service.Interfaces;
using Qed.Webhook.Service.Models.Requests.AccessToken;
using Qed.Webhook.Service.Models.Requests.Document;
using Qed.Webhook.Service.Models.Requests.Subcription;
using Qed.Webhook.Service.Models.Responses;
using Qed.Webhook.Service.Models.Responses.AccessToken;
using Qed.Webhook.Service.Models.Responses.Document;
using Qed.Webhook.Service.Models.Responses.Subcription;

namespace Qed.Webhook.Service.Services
{
    public class EncompassClient : IEncompassClient
    {
        private readonly HttpClient _httpClient;

        public EncompassClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<GetTokenResponse>> GetSecurityTokenAsync(GetTokenRequest request)
        {
            var tokenResponse = new ApiResponse<GetTokenResponse>(new GetTokenResponse());

            _httpClient.DefaultRequestHeaders.Clear();

            var authBody = new Dictionary<string, string>
            {
                {ConstantString.GrantTypeKey, ConstantString.GrantTypeValue},
                {
                    ConstantString.SmartClientUserKey,
                    string.Format(ConstantString.SmartClientUserValue, request.SmartClientUserName, request.InstanceId)
                },
                {ConstantString.SmartClientPasswordKey, request.SmartClientPassword},
                {ConstantString.ClientIdKey, request.ClientId},
                {ConstantString.ClientSecretKey, request.ClientSecret}
            };

            // there is no sandbox and uri for UAT and Prod are the same
            var authResponse = await _httpClient.PostAsync(ConstantString.OAuthTokenSuffix, new FormUrlEncodedContent(authBody)).ConfigureAwait(false);
            var content = await authResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            tokenResponse.StatusCode = authResponse.StatusCode;
            tokenResponse.Headers = authResponse.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());

            // examine response
            if (!authResponse.IsSuccessStatusCode)
            {
                tokenResponse.Body.FailureResponse = JsonConvert.DeserializeObject<GetTokenFailureResponse>(content);
            }
            else
            {
                tokenResponse.Body.SuccessResponse = JsonConvert.DeserializeObject<GetTokenSuccessResponse>(content);
            }
            return tokenResponse;
        }

        public async Task<ApiResponse<GetAllSubscriptionResponse>> GetAllSubscriptionAsync(GetAllSubscriptionRequest request)
        {
            var subsResponse = new ApiResponse<GetAllSubscriptionResponse>(new GetAllSubscriptionResponse());

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add(ConstantString.AuthorizationKey, string.Format(ConstantString.AuthorizationValue, request.AccessToken));

            var subsRequest = await _httpClient.GetAsync(ConstantString.SubscriptionSuffix).ConfigureAwait(false);
            var content = await subsRequest.Content.ReadAsStringAsync().ConfigureAwait(false);
            subsResponse.StatusCode = subsRequest.StatusCode;
            subsResponse.Headers = subsRequest.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());

            if (!subsRequest.IsSuccessStatusCode)
            {
                subsResponse.Body.FailureResponse = JsonConvert.DeserializeObject<GenericFailuareResponse>(content);
            }
            else
            {
                subsResponse.Body.SuccessResponse = JsonConvert.DeserializeObject<List<SubscriptionSuccessResponse>>(content);
            }
            return subsResponse;
        }

        public async Task<ApiResponse<UpdateSubscriptionResponse>> UpdateSubscriptionAsync(UpdateSubscriptionRequest request)
        {
            var updateResponse = new ApiResponse<UpdateSubscriptionResponse>(new UpdateSubscriptionResponse());
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add(ConstantString.AuthorizationKey, string.Format(ConstantString.AuthorizationValue, request.AccessToken));

            var subsBody = JsonConvert.SerializeObject(request.UpdatingSubscription);
            var updatingSubsContent = new StringContent(subsBody);
            updatingSubsContent.Headers.ContentType = new MediaTypeHeaderValue(ConstantString.JsonContentTypeValue);

            var subsRequest = await _httpClient.PutAsync(ConstantString.SubscriptionSuffix + "/" + request.SubscriptionId, updatingSubsContent).ConfigureAwait(false);
            var content = await subsRequest.Content.ReadAsStringAsync().ConfigureAwait(false);

            updateResponse.StatusCode = subsRequest.StatusCode;
            updateResponse.Headers = subsRequest.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
            updateResponse.Body = JsonConvert.DeserializeObject<UpdateSubscriptionResponse>(content);

            return updateResponse;
        }


        public async Task<ApiResponse<AddSubscriptionResponse>> AdddSubscriptionAsync(AddSubscriptionRequest request)
        {
            var addResponse = new ApiResponse<AddSubscriptionResponse>(new AddSubscriptionResponse());
            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Add(ConstantString.AuthorizationKey, string.Format(ConstantString.AuthorizationValue, request.AccessToken));
            
            var subsBody = JsonConvert.SerializeObject(request.AddingSubscription);
            var addingSubsContent = new StringContent(subsBody);
            addingSubsContent.Headers.ContentType = new MediaTypeHeaderValue(ConstantString.JsonContentTypeValue);

            var subsRequest = await _httpClient.PostAsync(ConstantString.SubscriptionSuffix, addingSubsContent).ConfigureAwait(false);
            var content = await subsRequest.Content.ReadAsStringAsync().ConfigureAwait(false);

            addResponse.StatusCode = subsRequest.StatusCode;
            addResponse.Headers = subsRequest.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
            addResponse.Body = JsonConvert.DeserializeObject<AddSubscriptionResponse>(content);

            return addResponse;
        }

        public async Task<ApiResponse<GetAttachmentResponse>> QueryDocumentAsync(GetAttachmentRequest request)
        {
            var getAttachmentResponse = new ApiResponse<GetAttachmentResponse>(new GetAttachmentResponse());

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add(ConstantString.AuthorizationKey, string.Format(ConstantString.AuthorizationValue, request.AccessToken));

            var attachmentRequest = await _httpClient.GetAsync(string.Format(ConstantString.AttachmentSuffix, request.LoanGuid)).ConfigureAwait(false);
            var content = await attachmentRequest.Content.ReadAsStringAsync().ConfigureAwait(false);
            getAttachmentResponse.StatusCode = attachmentRequest.StatusCode;
            getAttachmentResponse.Headers = attachmentRequest.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());

            if (!attachmentRequest.IsSuccessStatusCode)
            {
                getAttachmentResponse.Body.FailureResponse = JsonConvert.DeserializeObject<GenericFailuareResponse>(content);
            }
            else
            {
                getAttachmentResponse.Body.SuccessResponse = JsonConvert.DeserializeObject<List<Attachment>>(content);
            }

            return getAttachmentResponse;
        }

        private async Task<ApiResponse<GetAttachmentUrlResponse>> PullAttachmentDownloadUrlAsync(GetAttachmentUrlRequest request)
        {
            var attachmentUrlResponse = new ApiResponse<GetAttachmentUrlResponse>(new GetAttachmentUrlResponse());

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add(ConstantString.AuthorizationKey, string.Format(ConstantString.AuthorizationValue, request.AccessToken));

            var urlRequest = await _httpClient.PostAsync(string.Format(ConstantString.AttchmentUrlSuffix, request.LoanId, request.AttachmentId), null).ConfigureAwait(false);
            var content = await urlRequest.Content.ReadAsStringAsync().ConfigureAwait(false);
            attachmentUrlResponse.StatusCode = urlRequest.StatusCode;
            attachmentUrlResponse.Headers = urlRequest.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());

            if (!urlRequest.IsSuccessStatusCode)
            {
                attachmentUrlResponse.Body.FailureResponse = JsonConvert.DeserializeObject<GenericFailuareResponse>(content);
            }
            else
            {
                attachmentUrlResponse.Body.SuccessResponse = JsonConvert.DeserializeObject<GetAttachmentUrlSuccessResponse>(content);
            }
            return attachmentUrlResponse;
        }

        public async Task<ApiResponse<DownloadAttachmentResponse>> DownloadAttachmentAsync(DownloadAttachmentRequest request)
        {
            var downloadResponse =
                new ApiResponse<DownloadAttachmentResponse>(new DownloadAttachmentResponse())
                {
                    Body =
                    {
                        LoanId = request.LoanId,
                        EncompassDocument = request.EncompassDocument,
                        DownloadFullPath = request.DownloadFullPath
                    }
                };

            try
            {
                var getAttachmentUrl = await PullAttachmentDownloadUrlAsync(new GetAttachmentUrlRequest
                {
                    AccessToken = request.AccessToken,
                    AttachmentId = request.EncompassDocument.DocumentId,
                    LoanId = request.LoanId
                }).ConfigureAwait(false);

                if (!getAttachmentUrl.StatusCode.IsSuccessStatusCode())
                {
                    downloadResponse.Body.IsSuccessBit = false;
                    downloadResponse.Body.FailureResponse = getAttachmentUrl.Body.FailureResponse;
                    return downloadResponse;
                }

                var mediarUrl = getAttachmentUrl.Body.SuccessResponse.MediaUrl;
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add(ConstantString.AuthorizationKey,
                    string.Format(ConstantString.AuthorizationValue, request.AccessToken));

                var fileResponse = await FileDownloadHelper.DownloadFileAsync(_httpClient, mediarUrl, request.DownloadFullPath).ConfigureAwait(false);
                downloadResponse.Body.MediaUrl = mediarUrl;
                downloadResponse.StatusCode = fileResponse.StatusCode;
                downloadResponse.Headers = fileResponse.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
               
                if (!fileResponse.StatusCode.IsSuccessStatusCode())
                {
                    downloadResponse.Body.IsSuccessBit = false;
                    downloadResponse.Body.FailureResponse = fileResponse.Body.ErrorResponse;
                }
                else
                {
                    downloadResponse.Body.IsSuccessBit = true;
                }
            }
            catch (Exception ex)
            {
                downloadResponse.Body.IsSuccessBit = false;
                var result = JsonConvert.SerializeObject(new { message = JsonConvert.SerializeObject(ex.Message), stackTrace = ex.StackTrace });
                downloadResponse.Body.FailureResponse.Details = result;
                downloadResponse.Body.FailureResponse.ErrorCode = HttpStatusCode.InternalServerError.ToString();
            }

            return downloadResponse;
        }

    }
}
