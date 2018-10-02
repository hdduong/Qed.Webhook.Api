using System.Net;

namespace Qed.Webhook.Api.Shared.Helpers
{
    public static class HttpStatusCodeExtension
    {
        public static bool IsSuccessStatusCode(this HttpStatusCode code)
        {
            if ((int)code < 400)
                return true;

            return false;
        }
    }
}
