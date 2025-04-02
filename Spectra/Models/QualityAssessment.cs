using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_Quality_Assessment")]
    public class QualityAssessment
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Code { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Name { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Experience { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Email { get; set; }
        [RegularExpression(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$", ErrorMessage = "Phone is INVALID")]
        [Required(ErrorMessage = "This field is REQUIRED")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Advise { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Pack { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Expectation { get; set; }
        public string Desire { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
       
    }
}
