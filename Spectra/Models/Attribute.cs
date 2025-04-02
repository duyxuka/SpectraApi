using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_Attribute")]
    public class Attribute
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        public string Name { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ICollection<Item> Items { get; set; }
        public ICollection<ValueAttribute> ValueAttributes { get; set; }
    }
}
