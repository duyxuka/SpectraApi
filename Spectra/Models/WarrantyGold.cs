using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_WarrantyGold")]
    public class WarrantyGold
    {
        [Key]
        public int? Id { get; set; } 
        public string Phone { get; set; }
        [MaxLength(250, ErrorMessage = "Max of length is 250 characters")]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ProductName { get; set; }
        public string ProductSeri { get; set; }
        public DateTime DateBuy { get; set; }
        public float GtriHĐ { get; set; }
        public float PhiDVBH { get; set; }
        public string File { get; set; }
        [DefaultValue(true)]
        public byte Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ICollection<WarrantyGoldLog> WarrantyGoldLogs { get; set; }
    }
    public class WarrantyGoldDisplay : WarrantyGold
    {
        public string LinkName { get; set; }
        public string LocationName { get; set; }
    }
}
