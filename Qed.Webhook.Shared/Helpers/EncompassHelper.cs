using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qed.Webhook.Api.Shared.Helpers
{
    public class EncompassHelper
    {
        // Encompass return DocumentGuid like: Attachment-d90a182b-3992-42b5-b43e-518ce4352a67.pdf
        // we want to get just d90a182b-3992-42b5-b43e-518ce4352a67
        // Encompass LoanId {d90a182b-3992-42b5-b43e-518ce4352a67} and LoanGuid is d90a182b-3992-42b5-b43e-518ce4352a67
        public static Guid GetGuid(string extendedGuid)
        {
            return new Guid(Regex
                .Matches(extendedGuid, @"[0-9A-Fa-f]{8}[-]?([0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}")
                .Select(m => m.Value)
                .FirstOrDefault());
        }

        public static string GetLoanId(Guid loanGuid)
        {
            return "{" + loanGuid + "}";
        }

        public static string GetAlphaNumeric(string mixed)
        {
            var rgx = new Regex("[^a-zA-Z0-9]");
            var result = rgx.Replace(mixed, "");
            return result;
        }
    }
}
