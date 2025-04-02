using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    public class OrderEmailDto
    {
        public string Email { get; set; }
        public string AccountName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Payment { get; set; }
        public string Code { get; set; }
        public float TotalPrice { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }

    public class Orderprodetail: OrderEmailDto {
        public string Image { get; set; }
        public string ProductName { get; set; }

    }
}
