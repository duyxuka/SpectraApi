using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_OrderCus")]
    public class OrderCus
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
            public int TotalQuantity { get; set; }
            public int TotalAmount { get; set; }
            [DefaultValue(false)]
            public byte Status { get; set; }
            public int AccountCusId { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime ModifiedDate { get; set; }
            public ICollection<OrderDetailCus> OrderDetailCus { get; set; }
    }
}
