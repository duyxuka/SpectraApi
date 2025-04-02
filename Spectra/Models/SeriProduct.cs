using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_SeriProduct")]
    public class SeriProduct
    {
        [Key]
        public int? Id { get; set; }
        public string ProductSeri { get; set; }
        public int? CityId { get; set; }
        public City City { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public int? LocationId { get; set; }
        public Location Location { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public DateTime? DealerSaleDate { get; set; } = null;
        public DateTime CreatedDate { get; set; }
    }
    public class SeriProductDisplay : SeriProduct
    {
        public string ProductName { get; set; }
        public int ProductWarranty { get; set; }
        public int? CategoryId { get; set; }
        public string CityName { get; set; }
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
    }
}
