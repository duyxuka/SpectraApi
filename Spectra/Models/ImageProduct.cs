using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_ImageProduct")]
    public class ImageProduct
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string ImageName { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
    }
    public class ImageProductDisplay : ImageProduct
    {
        public string ProductName { get; set; }
        public int? ProductsId { get; set; }

    }
}
