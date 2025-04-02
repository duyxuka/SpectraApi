using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Spectra.Models.OrderDetail;
using Spectra.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Cors;

namespace Spectra.Controllers
{
    [EnableCors("AddCors")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderDetailController : ControllerBase
    {
        private readonly AppDBContext _context;

        public OrderDetailController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/OrderDetail
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<OrderDetail> GetOrderDetail()
        {
            return _context.OrderDetail;
        }

        // GET: api/OrderDetail/5
        [HttpGet]
        [Route("OderAcc/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderAccount([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = await _context.OrderDetail.Join(_context.Order, ai => ai.OrderId,
              al => al.Id, (ai, al) => new { ai, al }).Join(_context.AccountUsers, ac => ac.al.AccountUserId,
              ae => ae.Id, (ac, ae) => new { ac, ae }).Where(x => x.ac.ai.OrderId == id).Select(x => new DisplayOrderDetail
              {
                  Id = x.ac.ai.Id,
                  ProductName = x.ac.ai.Product.Name,
                  ProductId = x.ac.ai.ProductId,
                  Quantity = x.ac.ai.Quantity,
                  Price = x.ac.ai.Price,
                  Status = x.ac.ai.Status,
                  StatusOrder = x.ac.al.Status,
                  OrderId = x.ac.ai.OrderId,
                  DiscountVoucher = x.ac.ai.DiscountVoucher,
                  CreatedDate = x.ac.ai.CreatedDate,
                  ModifiedDate = x.ac.ai.ModifiedDate,
                  ProductCode = x.ac.ai.Product.Code,
                  Gift = x.ac.ai.Gift,
                  Brand = x.ac.ai.Brand
              }).ToListAsync();
            return Ok(data);
        }


        // GET: api/OrderDetails/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderDetail([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderDetail = await _context.OrderDetail
                .Join(_context.Products, od => od.ProductId,
                p => p.Id, (od, p) => new
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    Status = od.Status,
                    Brand = od.Brand,
                    OrderId = od.OrderId,
                    DiscountVoucher = od.DiscountVoucher,
                    CreatedDate = od.CreatedDate,
                    ModifiedDate = od.ModifiedDate,
                    ProductCode = p.Code,
                    ProductName = p.Name,
                    Gift = od.Gift,
                }).Select(x => new DisplayOrderDetail()
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Status = x.Status,
                    OrderId = x.OrderId,
                    Brand = x.Brand,
                    DiscountVoucher = x.DiscountVoucher,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName,
                    Gift = x.Gift,
                }).Where(x => x.OrderId == id).ToListAsync();

            if (orderDetail == null)
            {
                return NotFound();
            }

            return Ok(orderDetail);
        }



        // PUT: api/OrderDetails/5
        [HttpPost]
        [Route("PutProductQuantity")]
        public async Task<IActionResult> PutProductQuantity([FromBody] OrderDetail orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var orderDetails = _context.OrderDetails.Include(x => x.Product).Where(x => x.ProductId == orderDetail.ProductId).FirstOrDefault();
            var product = _context.Products.Where(x => x.Id == orderDetail.ProductId).FirstOrDefault();
            //product.Quantity = product.Quantity - orderDetail.Quantity;
            _context.Entry(product).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/OrderDetails
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostOrderDetail([FromBody] OrderDetail orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            orderDetail.CreatedDate = DateTime.Now;
            _context.OrderDetail.Add(orderDetail);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetOrderDetail", new { id = orderDetail.Id }, orderDetail);
        }

        [HttpPost]
        [Route("SendEmailCancel")]
        [AllowAnonymous]
        public ActionResult SendEmailCancel([FromBody] OrderEmailDto orderEmail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("mayhutsuaspectra@gmail.com", "Spectra");
                    var receiverEmail = new MailAddress(orderEmail.Email, "Receiver");
                    var password = "mieopkmqngqmotfk";
                    var subject = "[Spectra]Xác nhận hủy đơn hàng #" + orderEmail.Code;
                    var body = "<p style='text-align: center;'><img style='margin-left: 45px;' src='https://spectrababy.com.vn/assets/images/logo/logo_black_1x.png'></p>"
                    + "<h2 style='text-align: center;'>Đơn hàng của bạn đã được hủy</h2>"
                    + "<p>Xin chào, "+ orderEmail.Email+".</p>"
                    + "<p>Đơn hàng với mã <span style='font-weight: bold;'> #" + orderEmail.Code + "</span> đã được hủy theo yêu cầu của bạn.</p>"
                    + "<p>Spectra mong bạn sớm tìm được sản phẩm phù hợp trong quá trình nuôi con bằng sữa mẹ sắp tới nhé!</p>"
                    + "<h4>Mọi thắc mắc xin liên hệ:</h4>"
                    + "<p>SĐT: 0916001923.</p>"
                    + "<a href='https://www.facebook.com/spectra.vietnam'>Facebook: Spectra Baby</a>"
                    + "<h3>Cảm ơn quý khách đã quan tâm tới Spectra, thương hiệu máy hút sữa và phụ kiện hàng đầu Hàn Quốc!</h3></div>";
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    })
                    {
                        smtp.Send(mess);
                    }
                    return NoContent();
                }
            }
            catch (Exception)
            {

            }
            return NoContent();
        }

        [HttpPost]
        [Route("SendEmail")]
        [AllowAnonymous]
        public async Task<ActionResult> SendEmail([FromBody] OrderEmailDto orderEmail)
        {
            var date = DateTime.Now;
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("mayhutsuaspectra@gmail.com", "Spectra");
                    var receiverEmail = new MailAddress(orderEmail.Email, "Receiver");
                    var password = "mieopkmqngqmotfk";
                    var subject = "Đơn đặt hàng Spectra";

                    // Fetch product details for the order
                    var productDetailsList = await _context.Products.Join(orderEmail.OrderDetails, ai => ai.Id, al => al.ProductId, (ai, al) => new { ai, al })
                        .Select(p => new
                        {
                            p.ai.Id,
                            p.ai.Name,
                            p.ai.Images
                        }).ToListAsync(); // Thêm await ở đây

                    string accountorder = "<table style='width: 100%; border-collapse: collapse;'>"
                    + "<thead>"
                    + "<tr style='background-color: #f2f2f2;'>"
                    + "<th style='padding: 10px; border: 1px solid #ddd;'>Khách hàng</th>"
                    + "<th style='padding: 10px; border: 1px solid #ddd;'>Số điện thoại</th>"
                    + "<th style='padding: 10px; border: 1px solid #ddd;'>Địa chỉ đặt hàng</th>"
                    + "<th style='padding: 10px; border: 1px solid #ddd;'>Phương thức thanh toán</th>"
                    + "</tr>"
                    + "</thead>"
                    + "<tbody>"
                    + "<tr>"
                    + $"<td style='padding: 10px; border: 1px solid #ddd;'>{orderEmail.AccountName}</td>"
                    + $"<td style='padding: 10px; border: 1px solid #ddd;'>{orderEmail.Phone}</td>"
                    + $"<td style='padding: 10px; border: 1px solid #ddd;'>{orderEmail.Address}</td>"
                    + $"<td style='padding: 10px; border: 1px solid #ddd;'>{orderEmail.Payment}</td>"
                    + "</tr>"
                    + "</tbody></table>";

                    // Create email content with product details
                    string productDetails = "<table style='width: 100%; border-collapse: collapse;'>"
                    + "<thead>"
                    + "<tr style='background-color: #f2f2f2;'>"
                    + "<th style='padding: 10px; border: 1px solid #ddd;'>Tên sản phẩm</th>"
                    + "<th style='padding: 10px; border: 1px solid #ddd;'>Ảnh</th>"
                    + "<th style='padding: 10px; border: 1px solid #ddd;'>Quà tặng</th>"
                    + "<th style='padding: 10px; border: 1px solid #ddd;'>Size or color</th>"
                    + "<th style='padding: 10px; border: 1px solid #ddd;'>Số lượng</th>"
                    + "<th style='padding: 10px; border: 1px solid #ddd;'>Giá (VNĐ)</th>"
                    + "<th style='padding: 10px; border: 1px solid #ddd;'>Tổng giá (VNĐ)</th>"
                    + "</tr>"
                    + "</thead>"
                    + "<tbody>";

                    foreach (var item in orderEmail.OrderDetails)
                    {
                        var product = productDetailsList.FirstOrDefault(p => p.Id == item.ProductId);
                        var productName = product?.Name ?? "Unknown Product";
                        var productImage = !string.IsNullOrEmpty(product?.Images)
                            ? $"<img src='https://spectrababy.com.vn/dataApi/images/{product.Images}' style='width: 100px; height: auto;' />"
                            : "No Image";

                        productDetails += "<tr>"
                            + $"<td style='padding: 10px; border: 1px solid #ddd;'>{productName}</td>"
                            + $"<td style='padding: 10px; border: 1px solid #ddd;'>{productImage}</td>"
                            + $"<td style='padding: 10px; border: 1px solid #ddd;'>{item.Gift}</td>"
                            + $"<td style='padding: 10px; border: 1px solid #ddd;'>{item.Brand}</td>"
                            + $"<td style='padding: 10px; border: 1px solid #ddd;'>{item.Quantity}</td>"
                            + $"<td style='padding: 10px; border: 1px solid #ddd;'>{item.Price.ToString("#,##0")} VNĐ</td>"
                            + $"<td style='padding: 10px; border: 1px solid #ddd;'>{(item.Price * item.Quantity).ToString("#,##0")} VNĐ</td>"
                            + "</tr>";
                    }
                    var total = "<tr style='text-align: right;'> "
                        + "<td style='padding: 20px; border: 1px solid #ddd;font-size: 18px;' colspan='5'><b>Tổng tiền thanh toán:</b> " + orderEmail.TotalPrice.ToString("#,##0") + " VNĐ</td>"
                        + "</tr>";
                    productDetails += total + "</tbody></table>";

                    string body = "<div style='text-align: center;font-family: unset; font-size: 15px;'>"
                        + "<p style='text-align: center;'><img style='margin-left: 45px;' src='https://spectrababy.com.vn/assets/images/logo/logo_black_1x.png'></p>"
                        + "<p style='text-align: center;'><img src='https://spectrababy.com.vn/assets/images/output-onlinegiftools.gif'></p>"
                        + "<h2 style='color: #10cb04;font-size: 19px;'>Bạn đã đặt hàng thành công sản phẩm của Spectra.</h2>"
                        + "<p>Bộ phận xác nhận đơn hàng của chúng tôi sẽ liên hệ để xác nhận đơn hàng với bạn.</p>"
                        + "<p>Mã đơn hàng của bạn: " + orderEmail.Code + "</p>"
                        + "<p>Ngày đặt hàng: " + date + "</p>"
                        + "<h4>Thông tin khách hàng đặt hàng:</h4>"
                        + accountorder
                        + "<h4>Chi tiết đơn hàng:</h4>"
                        + productDetails
                        + "<h4>Sau khi xác nhận đơn hàng, sản phẩm sẽ được giao tới bạn trong vòng 2-3 ngày tới.</h4>"
                        + "<h4>Mọi thắc mắc xin liên hệ:</h4>"
                        + "<p>SĐT: 0916001923.</p>"
                        + "<a href='https://www.facebook.com/spectra.vietnam'>Facebook: Spectra Baby</a>"
                        + "<h3>Cảm ơn quý khách đã tin tưởng đặt hàng của chúng tôi!</h3></div>";

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };

                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    })
                    {
                        smtp.Send(mess);
                    }
                }
                return NoContent();
            }
            catch (Exception)
            {
                return NoContent();
            }
        }


        // DELETE: api/OrderDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderDetail = await _context.OrderDetail.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            _context.OrderDetail.Remove(orderDetail);
            await _context.SaveChangesAsync();

            return Ok(orderDetail);
        }

        private bool OrderDetailExists(int? id)
        {
            return _context.OrderDetail.Any(e => e.Id == id);
        }
    }
}