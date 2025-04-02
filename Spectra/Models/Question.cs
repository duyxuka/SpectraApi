using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_QuestionPro")]
    public class Question
    {
        [Key]
        public int? Id { get; set; }
        public string Code { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Title { get; set; }
        public string Description { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
    public class QuestionModels : Question
    {
        public string ProductName { get; set; }
        public string LinkName { get; set; }
    }
}
