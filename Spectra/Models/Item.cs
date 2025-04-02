using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_Item")]
    public class Item
    {
        [Key]
        public int? Id { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public int? ValueAttributeId { get; set; }
        public ValueAttribute ValueAttribute { get; set; }
        public int? AttributeId { get; set; }
        public Attribute Attribute { get; set; }
        public float Price { get; set; }
        public float PriceAgain { get; set; }
        public int? GiftId { get; set; }
        public Gift Gift { get; set; }
        public bool Status { get; set; }
        public string JobId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    public class ItemDisplay : Item
    {

        public string ProductName { get; set; }
        public string AttributeName { get; set; }
        public string ValueAttributeName { get; set; }
        public string GifeName { get; set; }
        public float GifePrice { get; set; }
        public int IntAttributeName { get; set; }
        public bool StatusColor { get; set; }
        public bool StatusSize { get; set; }
    }
}
