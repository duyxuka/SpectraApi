using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_ProductVariant_Attributes")]
    public class ProductVariantAttributes
    {
        [Key]
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
        public int ValueAttributeId { get; set; }
        public ValueAttribute ValueAttribute { get; set; }
    }
}
