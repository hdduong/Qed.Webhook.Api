using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Service.Models;
using Qed.Webhook.Service.Models.Responses;

namespace Qed.Webhook.Service.Helpers
{
    public class FileDownloadHelper
    {
        public static async Task<ApiResponse<DownloadFileResponse>> DownloadFileAsync(HttpClient httpClient, string downloadUrl, string fileNameWithPath)
        {
            var result = new ApiResponse<DownloadFileResponse>(new DownloadFileResponse());
            var response = new HttpResponseMessage();
            int bufferSize = ConstantValue.DefaultDownloadBufferSizeBytes;
            int flushToDiskSize = ConstantValue.LimitBytesFlushToDisk;
            try
            {

                using (response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false))
                {
                    using (var remoteStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        using (var contentStream = File.Create(fileNameWithPath))
                        {
                            int bytesRead;
                            int totalByteRead = 0;
                            var buffer = new byte[flushToDiskSize + bufferSize];
                            while ((bytesRead = await remoteStream.ReadAsync(buffer, totalByteRead, bufferSize).ConfigureAwait(false)) != 0)
                            {
                                totalByteRead += bytesRead;
                                if (totalByteRead >= flushToDiskSize)
                                {
                                    await FlushToDiskAsync(contentStream, buffer, totalByteRead).ConfigureAwait(false);
                                    totalByteRead = 0;
                                }                               
                            }
                            if (totalByteRead > 0)
                            {
                                await FlushToDiskAsync(contentStream, buffer, totalByteRead).ConfigureAwait(false);
                            }
                        }
                    }
                }
                result.StatusCode = response.StatusCode;
                result.Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
                result.Body.IsSuccessDownload = true;
            }
            catch (Exception ex)
            {
                result.StatusCode = response.StatusCode;
                result.Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
                result.Body.IsSuccessDownload = false;
                result.Body.ErrorResponse.ErrorCode = response.StatusCode.ToString();
                result.Body.ErrorResponse.Error = JsonConvert.SerializeObject(new { message = JsonConvert.SerializeObject(ex.Message), stackTrace = ex.StackTrace });
            }

            return result;
        }

        private static async Task FlushToDiskAsync(FileStream stream, byte[] buffer, int bytesToWrite)
        {
            await stream.WriteAsync(buffer, 0, bytesToWrite).ConfigureAwait(false);
            await stream.FlushAsync().ConfigureAwait(false);
        }

    }
}
