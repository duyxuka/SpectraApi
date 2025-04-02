using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_Voucher")]
    public class Voucher
    {
        [Key]
        public int? Id { get; set; }
        public string VoucherCode { get; set; }
        public float Discount { get; set; }
        public string DiscountType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [DefaultValue("0")]
        public string JobId { get; set; }
        [DefaultValue(false)]
        public bool Status { get; set; }
        [DefaultValue(false)]
        public bool ScheduleStatus { get; set; }
        public int Quantity { get; set; }
        public ICollection<VoucherUsage> VoucherUsages { get; set; }

    }
    public class VoucherDisplay : Voucher
    {
        public string ProductName { get; set; }
    }
}
