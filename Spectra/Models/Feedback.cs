using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_Feedback")]
    public class Feedback
    {
        [Key]
        public int? Id { get; set; }
        public string Code { get; set; }

        [MaxLength(250, ErrorMessage = "Max of length is 250 characters")]
        [MinLength(2, ErrorMessage = "This field can't least 2 characters")]
        public string Title { get; set; }
        public float Rating { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
    }
    public class ProFeedbackDisplay : Feedback
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public float ProductPrice { get; set; }
        public float ProductSalePrice { get; set; }
        public int ProductQuantity { get; set; }
        public string ProductImages { get; set; }
        public bool ProductStatus { get; set; }
        public string LinkName { get; set; }
        public float RatingAvg { get; set; }
    }
}
