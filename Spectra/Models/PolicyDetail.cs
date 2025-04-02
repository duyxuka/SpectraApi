using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_PolicyDetail")]
    public class PolicyDetail
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Code { get; set; }
        [MaxLength(250, ErrorMessage = "Max of length is 30 characters")]
        [MinLength(2, ErrorMessage = "This field can't least 2 characters")]
        public string Name { get; set; }
        public string Description { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public int? PolicyId { get; set; }
        public Policy Policy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class PolicyDetailDisplay : PolicyDetail
    {  
        public string CatePoName { get; set; }
        public string LinkName { get; set; }
    }
}
