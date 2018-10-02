using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qed.Webhook.Api.Shared.Helpers
{
    public class EncompassSdkHelper
    {
        public static string Decrypt(string encryptedPassword)
        {
            return AesGcm.SimpleDecrypt(encryptedPassword, EncryptionKey);
        }

        public static byte[] EncryptionKey
        {
            get
            {
                string key = "25, 124, 10, 135, 93, 122, 174, 46, 166, 158, 13, 190, 100, 162, 204, 93, 75, 237, 91, 67, 96,184, 56, 150, 208, 169, 159, 39, 166, 48, 243, 3";
                return key.Split(',').Select(s => Convert.ToByte(s)).ToArray();
            }
        }

        // serverUri in format https://BE11165897.ea.elliemae.net$BE11165897
        // to return BE11165897
        public static string GetEncompassInstance(string serverUri)
        {
            var instanceIdRegex = new Regex(@"(?<=[$])\w*");
            var matchInstanceId = instanceIdRegex.Match(serverUri);

            return matchInstanceId.Value;
        }
    }
}
