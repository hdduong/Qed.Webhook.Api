using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Api.Shared.Loggings;
using Qed.Webhook.Api.Shared.Models.WebhookNotification.Redis.Requests;
using Qed.Webhook.Api.Shared.Models.WebhookNotification.Redis.Responses;
using Qed.Webhook.Service.Interfaces;

namespace Qed.Webhook.Service.Services
{
    public class RedisCacheApiClient : IRedisCacheApiClient
    {
        private readonly HttpClient _httpClient;

        public RedisCacheApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private string GetRedisCacheWorkerUri(Guid resourceId)
        {
            var requestingUri = ConstantString.RedisCacheWorkerUri.Replace(ConstantString.ReplacingResourceId, resourceId.ToString());
            requestingUri = ConstantString.RedisCacheUri + "/" + requestingUri;
            return requestingUri;
        }


        public async Task<int> GetWorkerIdAsync(Guid resourceId)
        {
            _httpClient.DefaultRequestHeaders.Clear();

            var apiResponse = await _httpClient.GetAsync(GetRedisCacheWorkerUri(resourceId)).ConfigureAwait(false);
            var content = await apiResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<GetWorkerIdJobRedisCacheResponse>(content);

            if (!apiResponse.IsSuccessStatusCode)
            {
                throw new ApiException(JsonConvert.SerializeObject(apiResponse));
            }

            return response.WorkerId;
        }

        public async Task<bool> SetWorkerIdAsync(Guid resourceId, int workerId)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            var requestBody = JsonConvert.SerializeObject(new AssignJobWorkerIdRedisCacheRequest{WorkerId = workerId});
            var assigningSubsContent = new StringContent(requestBody);
            assigningSubsContent.Headers.ContentType = new MediaTypeHeaderValue(ConstantString.JsonContentTypeValue);

            var apiResponse = await _httpClient.PostAsync(GetRedisCacheWorkerUri(resourceId), assigningSubsContent).ConfigureAwait(false);
            var content = await apiResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<AssignJobWokerIdRedisCacheResponse>(content);

            if (!apiResponse.IsSuccessStatusCode)
            {
                throw new ApiException(JsonConvert.SerializeObject(response));
            }

            return response.IsSuccessBit;
        }


        private string GetRedisCacheAttachmentUri(Guid resourceId)
        {
            var requestingUri = ConstantString.RedisCacheDocumentUri.Replace(ConstantString.ReplacingResourceId, resourceId.ToString());
            requestingUri = ConstantString.RedisCacheUri + "/" + requestingUri;
            return requestingUri;
        }

        public async Task<string> GetAttachmentByLoanGuidAsync(Guid resourceId)
        {
            var apiResponse = await _httpClient.GetAsync(GetRedisCacheAttachmentUri(resourceId)).ConfigureAwait(false);
            var content = await apiResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<GetDocumentRedisCacheResponse>(content);

            if (!apiResponse.IsSuccessStatusCode)
            {
                throw new ApiException(JsonConvert.SerializeObject(apiResponse));
            }

            return response.Attachments;
        }

        public async Task<bool> SetLoanAttachmentAsync(Guid resourceId, string attachmentTxt)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            var requestBody = JsonConvert.SerializeObject(new SetDocumentRedisCacheRequest { Attachment = attachmentTxt });
            var setAttachmentContent = new StringContent(requestBody);
            setAttachmentContent.Headers.ContentType = new MediaTypeHeaderValue(ConstantString.JsonContentTypeValue);

            var apiResponse = await _httpClient.PostAsync(GetRedisCacheAttachmentUri(resourceId), setAttachmentContent).ConfigureAwait(false);
            var content = await apiResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<SetDocumentRedisCacheResponse>(content);

            if (!apiResponse.IsSuccessStatusCode)
            {
                throw new ApiException(JsonConvert.SerializeObject(response));
            }

            return response.IsSuccessBit;
        }
    }
}
