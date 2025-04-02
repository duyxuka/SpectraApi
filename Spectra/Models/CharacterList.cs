using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_CharacterList")]
    public class CharacterList
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public int? CharacteristicId { get; set; }
        public Characteristic Characteristic { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    public class CharacterListDisplay : CharacterList
    {

        public string CateNewName { get; set; }
        public string CateName { get; set; }
        public string LinkName { get; set; }
    }
}
