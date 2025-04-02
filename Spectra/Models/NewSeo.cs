using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_NewSeo")]
    public class NewSeo
    {
        [Key]
        public int? Id { get; set; }
        public string TitleSeo { get; set; }
        public string MetaKeyWords { get; set; }
        public string MetaDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    public class NewSeoDisplay : NewSeo
    {

        public string LinkName { get; set; }
    }
}
