using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_WarrantyGold_Log")]
    public class WarrantyGoldLog
    {
        [Key]
        public int? Id { get; set; }
        public int? WarrantyGoldId { get; set; }
        public WarrantyGold WarrantyGold { get; set; }
        public string ChangedBy { get; set; }
        public string WarrantyContent { get; set; }
        public float OldValue { get; set; }
        public float NewValue { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
