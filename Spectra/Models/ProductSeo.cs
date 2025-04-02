using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_ProductSeo")]
    public class ProductSeo
    {
        [Key]
        public int? Id { get; set; }
        public string TitleSeo { get; set; }
        public string MetaKeyWords { get; set; }
        public string MetaDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    public class ProductSeoDisplay : ProductSeo
    {

        public string LinkName { get; set; }
    }
}
