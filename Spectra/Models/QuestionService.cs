using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_QuestionServi")]
    public class QuestionService
    {
        [Key]
        public int? Id { get; set; }
        public string Code { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Title { get; set; }
        public string Description { get; set; }
        public int? ServiceDetailId { get; set; }
        public ServiceDetail ServiceDetail { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
    public class QuestionDisplay: QuestionService
    {
        public string ServiceName { get; set; }
        public string LinkName { get; set; }
    }
}
