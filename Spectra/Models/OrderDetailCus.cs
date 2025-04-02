using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_OrderDetailCus")]
    public class OrderDetailCus
    {
        [Key]
        public int? Id { get; set; }
        // Foreign Key - tblProduct
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public string Gift { get; set; }
        public string Brand { get; set; }
        public float Price { get; set; }
        [DefaultValue(false)]
        public bool Status { get; set; }
        public int OrderCusId { get; set; }
        public OrderCus OrderCus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public class DisplayOrderDetailCus : OrderDetailCus
        {
            public string ProductCode { get; set; }
            public int TotalPrice { get; set; }
            public int TotalQuantity { get; set; }
            public string ProductName { get; set; }
        }
    }
}
