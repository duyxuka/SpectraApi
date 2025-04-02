using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    public class PaymentIPN
    {
        public PaymentIPN()
        {

        }
        public PaymentIPN(string rspCode , string massge)
        {
            RspCode = rspCode;
            Message = massge;
        }
        public void Set(string rspCode, string massge)
        {
            RspCode = rspCode;
            Message = massge;
        }
        public string RspCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

    }
}
