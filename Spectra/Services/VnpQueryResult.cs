using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Services
{
    public class VnpQueryResult
    {
        public string vnp_ResponseCode { get; set; }
        public string vnp_TxnRef { get; set; }
        public string vnp_TransactionNo { get; set; }
        public string vnp_Amount { get; set; }
        public string vnp_TransactionStatus { get; set; }
        public string vnp_PayDate { get; set; }
        public string vnp_Message { get; set; }
    }

}
