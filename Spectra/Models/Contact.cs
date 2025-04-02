using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_Contact")]
    public class Contact
    {
        [Key]
        public int? Id { get; set; }
        public int Code { get; set; }
        [Required(ErrorMessage = "This field is REQUIRED")]
        [MinLength(2, ErrorMessage = "This field can't least 2 characters")]
        public string Name { get; set; }
        [RegularExpression(@"^([\w-\.]{2,30})@([\w]{2,12})(\.)([\w]{2,4})$", ErrorMessage = "Email is INVALID")]
        [DefaultValue(null)]
        public string Email { get; set; }
        [RegularExpression(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$", ErrorMessage = "Phone is INVALID")]
        [Required(ErrorMessage = "This field is REQUIRED")]
        public string Phone { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
