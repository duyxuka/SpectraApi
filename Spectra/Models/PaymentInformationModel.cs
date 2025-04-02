using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_Payment")]
    public class PaymentInformationModel
    {
        [Key]
        public int? Id { get; set; }
        public string Amount { get; set; }
        public string TransactionId { get; set; }
        public string PaymentCode { get; set; }
        public string PaymentInfor { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
