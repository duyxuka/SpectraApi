using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_Warranty")]
    public class Warranty
    {
        [Key]
        public int? Id { get; set; } 
        public string Phone { get; set; }
        [MaxLength(250, ErrorMessage = "Max of length is 250 characters")]
        public string Name { get; set; }
        public string Email { get; set; }
        public string ProductName { get; set; }
        public string ProductSeri { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string StoreCode { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    public class WarrantyDisplay : Warranty
    {
        public string LinkName { get; set; }
        public string LocationName { get; set; }
    }
}
