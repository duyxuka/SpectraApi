using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_GiftUser")]
    public class GiftUser
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string NameFacebook { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Name { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string LinkArticle { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Prize { get; set; }
        [RegularExpression(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$", ErrorMessage = "Phone is INVALID")]
        [Required(ErrorMessage = "This field is REQUIRED")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Address { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public DateTime BirthDay { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
       
    }
}
