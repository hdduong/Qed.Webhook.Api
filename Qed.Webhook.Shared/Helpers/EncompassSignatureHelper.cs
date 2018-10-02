using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Qed.Webhook.Api.Shared.Helpers
{
    public class EncompassSignatureHelper
    {
        public static bool Validate(string signature, string clientSecret, string message)
        {
            var hmacKeyInited = new HMACSHA256(Encoding.ASCII.GetBytes(clientSecret));
            var msgHashed = Convert.ToBase64String(hmacKeyInited.ComputeHash(Encoding.ASCII.GetBytes(message)));

            return msgHashed.Equals(signature, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
