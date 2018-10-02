using System;

namespace Qed.Webhook.Api.Shared.Loggings
{
    public class ApiNotFoundException : Exception
    {
        public ApiNotFoundException()
        {
        }

        public ApiNotFoundException(string message)
            : base(message)
        {
        }

        public ApiNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
