using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_ValueAttribute")]
    public class ValueAttribute
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        public string Name { get; set; }
        public int? AttributeId { get; set; }
        public Attribute Attribute { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ICollection<Item> Items { get; set; }
    }
    public class ValueDisplay : ValueAttribute
    {

        public string AttributeName { get; set; }
    }
}
