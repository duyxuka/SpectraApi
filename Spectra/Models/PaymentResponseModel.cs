using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    public class PaymentResponseModel
    {
        public string OrderDescription { get; set; }
        public long Amount { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentId { get; set; }
        public string Success { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
        public string VnPayTransitionStatus { get; set; }
    }
}
