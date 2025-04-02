using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_County")]
    public class County
    {
        [Key]
        public int? Id { get; set; }
        public string Name { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public int? CityId { get; set; }
        public City City { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        //public ICollection<Location> Locations { get; set; }
    }
}
