using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectra.Models;
using static Spectra.Models.OrderDetail;
using static Spectra.Models.OrderDetailCus;

namespace Spectra.Controllers
{
    [EnableCors("AddCors")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderDetailCusController : ControllerBase
    {
        private readonly AppDBContext _context;

        public OrderDetailCusController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/OrderDetailCus
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<OrderDetailCus> GetOrderDetailCus()
        {
            return _context.OrderDetailCus;
        }

        // GET: api/OrderDetailCus/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderDetailCus([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var orderDetail = await _context.OrderDetailCus.Join(_context.OrderCus, ai => ai.OrderCusId,
              al => al.Id, (ai, al) => new { ai, al }).Join(_context.Products, ac => ac.ai.ProductId,
              ae => ae.Id, (ac, ae) => new { ac, ae }).Where(x => x.ac.ai.OrderCusId == id).Select(x => new DisplayOrderDetailCus
              {
                  Id = x.ac.ai.Id,
                  ProductName = x.ac.ai.Product.Name,
                  ProductId = x.ac.ai.ProductId,
                  Quantity = x.ac.ai.Quantity,
                  Price = x.ac.ai.Price,
                  Status = x.ac.ai.Status,
                  Brand = x.ac.ai.Brand,
                  TotalPrice = x.ac.al.TotalAmount,
                  TotalQuantity = x.ac.al.TotalQuantity,
                  OrderCusId = x.ac.ai.OrderCusId,
                  CreatedDate = x.ac.ai.CreatedDate,
                  ModifiedDate = x.ac.ai.ModifiedDate,
                  ProductCode = x.ac.ai.Product.Code,
                  Gift = x.ac.ai.Gift
              }).ToListAsync();

            if (orderDetail == null)
            {
                return NotFound();
            }

            return Ok(orderDetail);
        }

        // PUT: api/OrderDetailCus/5
        [HttpPost]
        [Route("PutProductQuantity")]
        public async Task<IActionResult> PutProductQuantity([FromBody] OrderDetailCus orderDetailCus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var orderDetails = _context.OrderDetails.Include(x => x.Product).Where(x => x.ProductId == orderDetail.ProductId).FirstOrDefault();
            var product = _context.Products.Where(x => x.Id == orderDetailCus.ProductId).FirstOrDefault();
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

        // POST: api/OrderDetailCus
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostOrderDetailCus([FromBody] OrderDetailCus orderDetailCus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            orderDetailCus.CreatedDate = DateTime.Now;
            _context.OrderDetailCus.Add(orderDetailCus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderDetailCus", new { id = orderDetailCus.Id }, orderDetailCus);
        }

        
        [HttpPost]
        [Route("SendEmail")]
        [AllowAnonymous]
        public ActionResult SendEmail([FromBody] string email)
        {
            var date = DateTime.Now;
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("mayhutsuaspectra@gmail.com", "Spectra");
                    var receiverEmail = new MailAddress(email, "Receiver");
                    var password = "mieopkmqngqmotfk";
                    var subject = "Đơn đặt hàng Spectra";
                    string body = "<div style='text-align: center;font-family: unset; font-size: 15px;'>"
                         + "<p style='text-align: center;'><img style='margin-left: 45px;' src='https://spectrababy.com.vn/assets/images/logo/logo_black_1x.png'></p>"
                         + "<p style='text-align: center;'><img src='https://spectrababy.com.vn/assets/images/output-onlinegiftools.gif'></p>"
                         + "<h2 style='color: #10cb04;font-size: 19px;'>Bạn đã đặt hàng thành công sản phẩm của Spectra.</h2>"
                         + "<p>Bộ phận xác nhận đơn hàng của chúng tôi sẽ liên hệ để xác nhận đơn hàng với bạn.</p>"
                         + "<p>Ngày đặt hàng: " + date + "</p>"
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

            }
            return NoContent();
        }
        // DELETE: api/OrderDetailCus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetailCus([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderDetail = _context.OrderDetailCus.Where(x => x.OrderCusId == id).ToList();
            if (orderDetail == null)
            {
                return NotFound();
            }
            for (int i = 0; i < orderDetail.Count; i++)
            {
                _context.OrderDetailCus.Remove(orderDetail[i]);
                await _context.SaveChangesAsync();
            }



            return Ok(orderDetail);
        }

        private bool OrderDetailCusExists(int? id)
        {
            return _context.OrderDetailCus.Any(e => e.Id == id);
        }
    }
}