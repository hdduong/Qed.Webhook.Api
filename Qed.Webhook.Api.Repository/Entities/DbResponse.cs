using System;
using System.Collections.Generic;
using System.Text;

namespace Qed.Webhook.Api.Repository.Entities
{
    public class DbResponse<T>
    {
        public T Body { get; set; }
        public bool IsSuccessBit {get; set; }
        public string ErrorMsgTxt { get; set; }

        public DbResponse(T body)
        {
            Body = body;
            IsSuccessBit = false;
            ErrorMsgTxt = string.Empty;
        }
    }
}
