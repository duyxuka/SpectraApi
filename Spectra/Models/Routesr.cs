using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_Routes")]
    public class Routesr
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Path { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Component { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
