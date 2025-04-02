using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_VoucherUsage")]
    public class VoucherUsage
    {
        [Key]
        public int Id { get; set; }
        public int VoucherId { get; set; }
        public Voucher Voucher { get; set; }
        public int CustomerId { get; set; } // Mã khách hàng
        public DateTime UsedDate { get; set; } // Ngày sử dụng
    }
}
