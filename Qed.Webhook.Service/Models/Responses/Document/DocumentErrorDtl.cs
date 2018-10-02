using System;
using System.Collections.Generic;
using System.Text;

namespace Qed.Webhook.Service.Models.Responses.Document
{
    public class DocumentErrorDtl
    {
        public bool IsDownloadSuccessBit { get; set; }
        public string ErrorMsgTxt { get; set; }
    }
}
