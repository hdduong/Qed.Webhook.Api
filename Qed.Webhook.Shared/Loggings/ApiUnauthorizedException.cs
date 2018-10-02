using System;

namespace Qed.Webhook.Api.Shared.Loggings
{
    public class ApiUnauthorizedException : Exception
    {
        public ApiUnauthorizedException()
        {
        }

        public ApiUnauthorizedException(string message)
            : base(message)
        {
        }

        public ApiUnauthorizedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

