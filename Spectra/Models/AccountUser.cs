using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_AccountUser")]
    public class AccountUser
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Code { get; set; }
        [MaxLength(250, ErrorMessage = "Max of length is 30 characters")]
        [MinLength(2, ErrorMessage = "This field can't least 2 characters")]
        public string Name { get; set; }
        [DefaultValue(true)]
        public bool? Gender { get; set; }
        [Required(ErrorMessage = "This field is REQUIRED")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Number phone is REQUIRED")]
        [RegularExpression(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$", ErrorMessage = "Phone is INVALID")]
        public string Phone { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        [Required(ErrorMessage = "This field can't empty")]
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ICollection<Order> orders { get; set; }
    }
}
