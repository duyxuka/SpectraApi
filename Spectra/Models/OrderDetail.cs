using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_OrderDetail")]
    public class OrderDetail
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
        public float DiscountVoucher { get; set; }
        [DefaultValue(false)]
        public bool Status { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public class DisplayOrderDetail : OrderDetail
        {
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public string BrandName { get; set; }
            public string TypeDisscount
            {
                get
                {
                    if (string.IsNullOrEmpty(Gift))
                    {
                        return "unknown";
                    }
                    return Gift.Contains("amount") ? "amount" : "percentage";
                }
            }

            public byte StatusOrder { get; set; }
        }
    }
}
