using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_ProductVariant")]
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public float Price { get; set; }
        public float SalePrice { get; set; }
        public string JobId { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ICollection<ProductVariantAttributes> ProductVariantAttributes { get; set; }
    }
}
