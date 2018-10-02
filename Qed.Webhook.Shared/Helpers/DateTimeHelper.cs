using System;

namespace Qed.Webhook.Api.Shared.Helpers
{
    public class DateTimeHelper
    {
        public static long ToEpochTime(DateTime dtTmInUtc)
        {
            var t = dtTmInUtc - new DateTime(1970, 1, 1);
            var seconds = (long)t.TotalSeconds;
            return seconds;
        }
    }
}
