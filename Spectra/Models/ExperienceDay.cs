using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_ExperienceDay")]
    public class ExperienceDay
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Name { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Email { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Old { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Mom { get; set; }
        public string Breastpump { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Private { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Time { get; set; }
        public byte Website { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
