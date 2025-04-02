using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_Order")]
    public class Order
    {
        [Key]
        public int? Id { get; set; }
        [MaxLength(250, ErrorMessage = "Max of length is 250 characters")]
        [MinLength(2, ErrorMessage = "This field can't least 2 characters")]
        public string Code { get; set; }
        [MaxLength(250, ErrorMessage = "Max of length is 250 characters")]
        [MinLength(2, ErrorMessage = "This field can't least 2 characters")]
        public string Name { get; set; }
        [MaxLength(250, ErrorMessage = "Max of length is 250 characters")]
        [MinLength(2, ErrorMessage = "This field can't least 2 characters")]
        public string Address { get; set; }
        [Required(ErrorMessage = "This field is REQUIRED")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Number phone is REQUIRED")]
        public string Phone { get; set; }
        public string Note { get; set; } = null;
        public string PaymentMethod { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalAmount { get; set; }
        [DefaultValue(false)]
        public byte Status { get; set; }
        [DefaultValue(false)]
        public byte Website { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int AccountUserId { get; set; }
        public AccountUser AccountUser { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
