using System.Collections.Generic;
using System.Net;

namespace Qed.Webhook.Service.Models.Responses
{
    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public T Body { get; set; }

        public ApiResponse(T body)
        {
            Body = body;
            Headers = new Dictionary<string, string>();
        }

        public ApiResponse(HttpStatusCode statusCode, IDictionary<string, string> headers, T body)
        {
            StatusCode = statusCode;
            Headers = headers;
            Body = body;
        }
    }
}
