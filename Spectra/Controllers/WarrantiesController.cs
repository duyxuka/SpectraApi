using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectra.Models;

namespace Spectra.Controllers
{
    [EnableCors("AddCors")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarrantiesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public WarrantiesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Warranties
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Warranty> GetWarranties()
        {
            try
            {
                using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
                {
                    return context.Warranties
                        .Select(x => new WarrantyDisplay
                        {
                            Id = x.Id,
                            ProductName = x.ProductName,
                            ProductSeri = x.ProductSeri,
                            Status = x.Status,
                        })
                        .Where(s => s.Status == true)
                        .OrderByDescending(x => x.Id)
                        .AsNoTracking()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây, ví dụ log lại hoặc trả về lỗi phù hợp
                Console.WriteLine($"Lỗi trong quá trình lấy danh sách bảo hành: {ex.Message}");
                throw; // Ném ngoại lệ để lớp điều khiển xử lý tiếp tục xử lý
            }
        }

        [HttpGet]
        [Route("WrantyPage")]
        public IActionResult WarrantyResult(int? page, int pagesize = 5)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var countDetails = _context.Warranties.AsNoTracking().Count();
                var currentPage = page ?? 1;
                using (var context = _context)
                {
                    var warantyQuery = context.Warranties
                        .Select(x => new WarrantyDisplay
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Email = x.Email,
                            Phone = x.Phone,
                            ProductName = x.ProductName,
                            ProductSeri = x.ProductSeri,
                            Image = x.Image,
                            Description = x.Description,
                            Status = x.Status,
                            StoreCode = x.StoreCode,
                            StartDate = x.StartDate,
                            CreatedDate = x.CreatedDate,
                            ModifiedDate = x.ModifiedDate
                        })
                        .Where(s => s.Status == true)
                        .OrderByDescending(x => x.Id)
                        .AsNoTracking()
                        .Skip((currentPage - 1) * pagesize)
                        .Take(pagesize);

                    var result = new PageResult<WarrantyDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = pagesize,
                        Items = warantyQuery.ToList()
                };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("excel")]
        public async Task<FileResult> ExportExcel()
        {
            var data = await _context.Warranties.ToListAsync();
            var fileName = "baohanh.xlsx";
            return GenrateExcel(fileName, data);

        }

        private FileResult GenrateExcel(string filename, IEnumerable<Warranty> warranties)
        {
            DataTable dataTable = new DataTable("dbo.Spectra_Warranty");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Tên"),
                new DataColumn("Email"),
                new DataColumn("Số điện thoại"),
                new DataColumn("Sản phẩm"),
                new DataColumn("Seri sản phẩm"),
                new DataColumn("Ghi chú"),
                new DataColumn("Đại lý"),
                new DataColumn("Ngày đăng ký"),
                new DataColumn("Ngày bắt đầu BH"),
                new DataColumn("Ngày hết hạn BH"),
            });

            foreach (var warranty in warranties)
            {
                dataTable.Rows.Add(warranty.Name, warranty.Email, warranty.Phone, warranty.ProductName,
                                    warranty.ProductSeri, warranty.Description, warranty.StoreCode, warranty.CreatedDate.ToString("dd/MM/yyyy"), warranty.StartDate.ToString("dd/MM/yyyy"), warranty.ModifiedDate.ToString("dd/MM/yyyy"));
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        , filename);
                }
            }
        }

        [HttpGet]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSearch([FromQuery] string code)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productseri = await _context.Warranties.Select(x => new WarrantyDisplay
              {
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  ProductName = x.ProductName,
                  ProductSeri = x.ProductSeri,
                  Description = x.Description,
                  Image = x.Image,
                  Status = x.Status,
                  StartDate = x.StartDate,
                  CreatedDate = x.CreatedDate ,
                  ModifiedDate = x.ModifiedDate,

            }).Where(s => s.ProductSeri == code).FirstOrDefaultAsync();

            if (productseri == null)
            {
                return NotFound();
            }

            return Ok(productseri);
        }

        [HttpPost]
        [Route("SendEmailWarranty")]
        [AllowAnonymous]
        public ActionResult SendEmailWarranty([FromBody] Warranty warranty)
        {
            var date = DateTime.Now;
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("info@vietlife.com.vn", "Spectra Việt Nam");
                    var receiverEmail = new MailAddress(warranty.Email, "Receiver");
                    var password = "Welc0me!!!";
                    var subject = "BẢO HÀNH SẢN PHẨM SPECTRA - "+ warranty.ProductName;
                    string body = "<div style='font-family: unset; font-size: 15px;'>"
                        + "<p style='text-align: center;'><img style='margin-left: 45px;' src='https://spectra.vn/assets/images/logo/logo_black_1x.png'></p>"
                        + "<p style='text-align: center;'><img src='https://spectra.vn/assets/images/output-onlinegiftools.gif'></p>"
                        + "<h2 style='color: #10cb04;font-size: 22px;'>Bạn đã đăng ký bảo hành thành công sản phẩm của Spectra.</h2>"
                        + "<div style='text-align: left;'>"
                        + "<h3>Kính gửi: Quý khách hàng thân mến,</h3>"
                        + "<p>Spectra xin chân thành cảm ơn vì đã tin tưởng và lựa chọn sản phẩm và dịch vụ của chúng tôi. Đây là một niềm vinh dự lớn lao của Spectra khi được đồng hành cùng Mẹ & Bé.</p>"
                        + "<h5 style='font-size: 16px;'>Thông tin bảo hành sản phẩm:</h5>"
                        + "</div>"
                        + "<table style='border: 1px solid; width: 100%; '>"
                        + "<thead>"
                        + "<tr>"
                        + "<th style='padding: 10px;border-bottom: 1px solid;border-right: 1px solid; '>Thông tin khách hàng</th>"
                        + "<th style='padding: 10px;border-bottom: 1px solid; '>Thông tin bảo hành</th>"
                        + "</tr>"
                        + "</thead>"
                        + "<tbody>"
                        + "<tr>"
                        + "<td style='border-right: 1px solid;'>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Họ và tên: </strong>"+ warranty.Name
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Email: </strong>" + warranty.Email
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Số điện thoại: </strong>" + warranty.Phone
                        + "</div>"
                        + "</td>"
                        + "<td>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Sản phẩm bảo hành: </strong>" + warranty.ProductName
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Mã seri sản phẩm: </strong>" + warranty.ProductSeri
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong><strong>Đại lý: </strong>" + warranty.StoreCode
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Ngày đăng ký bảo hành: </strong>" + date.ToString("dd/MM/yyyy - HH:mm:ss tt zz")
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Ngày bắt đầu bảo hành: </strong>" + warranty.StartDate.ToString("dd/MM/yyyy")
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Ngày hết hạn bảo hành: </strong>" + warranty.ModifiedDate.ToString("dd/MM/yyyy")
                        + "</div>"
                        + "</td>"
                        + "</tr>"
                        + "</tbody>"
                        + "</table>"
                        + "<p>Trong quá trình sử dụng sản phẩm/dịch vụ, nếu có bất kỳ vấn đề hay thắc mắc, hãy liên hệ để chúng tôi có cơ hội được hỗ trợ kịp thời theo thông tin như sau:</p>"
                        + "<table style='border: 1px solid ; width: 100%; '>"
                        + "<thead>"
                        + "<tr>"
                        + "<th style='padding: 10px;border-bottom: 1px solid ;border-right: 1px solid ; '>Tỉnh thành phố</th>"
                        + "<th style='padding: 10px;border-bottom: 1px solid ;border-right: 1px solid ; '>Địa chỉ</th>"
                        + "<th style='padding: 10px;border-bottom: 1px solid ; '>Điện thoại/Zalo</th>"
                        + "</tr>"
                        + "</thead>"
                        + "<tbody>"
                        + "<tr>"
                        + "<td style='border-right: 1px solid ; '>TP.Hà Nội</td>"
                        + "<td style='border-right: 1px solid ; '>433 Nguyễn Khang, Phường Yên Hòa, Quận Cầu Giấy, TP.Hà Nội</td>"
                        + "<td style='text-align: center;'>0934.609.188</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td style='border-right: 1px solid ; '>TP.Hồ Chí Minh</td>"
                        + "<td style='border-right: 1px solid ; '>420 Nơ Trang Long, Phường 13, Quận Bình Thạnh, TP.Hồ Chí Minh</td>"
                        + "<td style='text-align: center;'>0934.317.299</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td style='border-right: 1px solid ; '>TP.Cần Thơ</td>"
                        + "<td style='border-right: 1px solid ; '>Tầng 5 tòa nhà Blossom Building 153Q Trần Hưng Đạo, Phường An Phú, Quận Ninh Kiều, TP.Cần Thơ</td>"
                        + "<td style='text-align: center;'>0916.001.823</td>"
                        + "</tr>"
                        + "</tbody>"
                        + "</table>"
                        + "<p>Một lần nữa, chúng tôi xin chân thành cảm ơn quý khách hàng và hy vọng được đón tiếp, đồng hành trong suốt hành trình khôn lớn của bé.</p>"
                        + "<p>Bộ phận xác nhận bảo hành của chúng tôi sẽ liên hệ để xác nhận thông tin với bạn.</p>"
                        + "<h4>Mọi thắc mắc xin liên hệ:</h4>"
                        + "<p>Chăm sóc khách hàng và chất lượng dịch vụ:</p>"
                        + "<div>"
                        + "<span style='color:#f7657e'>Điện thoại/Zalo: </span><a href='tel:+84936268085'>0936.268.085</a>"
                        + "</div>"
                        + "<div>"
                        + "Email: <a href='mailto:info@vietlife.com.vn'>info@vietlife.com.vn</a>"
                        + "</div>"
                        + "<div>"
                        + "Website: <a href='https://spectra.vn'>spectra.vn</a>"
                        + "</div>"
                        + "<div>"
                        + "Facebook: <a href='https://www.facebook.com/spectra.vn'>Spectra.VN</a>"
                        + "</div>"
                        + "<h3>Cảm ơn quý khách đã tin tưởng sử dụng sản phẩm của chúng tôi!</h3></div>";

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.mailer.inet.vn",
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

        [HttpPost]
        [Route("SendEmailWarrantyConfirm")]
        public ActionResult SendEmailWarrantyConfirm([FromBody] Warranty warranty)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("info@vietlife.com.vn", "Spectra Việt Nam");
                    var receiverEmail = new MailAddress(warranty.Email, "Receiver");
                    var password = "Welc0me!!!";
                    var subject = "Email XÁC NHẬN BẢO HÀNH SẢN PHẨM SPECTRA - " + warranty.ProductName;
                    string body = "<div style='font-family: unset; font-size: 15px;'>"
                        + "<p style='text-align: center;'><img style='margin-left: 45px;' src='https://spectra.vn/assets/images/logo/logo_black_1x.png'></p>"
                        + "<h2 style='color: #10cb04;font-size: 22px;'>Đây là Mail xác nhận bảo hành từ phía Spectra.</h2>"
                        + "<div style='text-align: left;'>"
                        + "<h3>Kính gửi: Quý khách hàng thân mến,</h3>"
                        + "<p>Spectra xin chân thành cảm ơn vì đã tin tưởng và lựa chọn sản phẩm và dịch vụ của chúng tôi. Đây là một niềm vinh dự lớn lao của Spectra khi được đồng hành cùng Mẹ & Bé.</p>"
                        + "<p>Từ phía Spectra, chúng tôi đã nhận được thông tin đăng ký bảo hành từ phía bạn, dưới đây là thông tin bảo hành sau khi chúng tôi đã xem xét và xác nhận.</p>"
                        + "<h5 style='font-size: 16px;'>Thông tin bảo hành sản phẩm:</h5>"
                        + "</div>"
                        + "<table style='border: 1px solid; width: 100%; '>"
                        + "<thead>"
                        + "<tr>"
                        + "<th style='padding: 10px;border-bottom: 1px solid;border-right: 1px solid; '>Thông tin khách hàng</th>"
                        + "<th style='padding: 10px;border-bottom: 1px solid; '>Thông tin bảo hành</th>"
                        + "</tr>"
                        + "</thead>"
                        + "<tbody>"
                        + "<tr>"
                        + "<td style='border-right: 1px solid;'>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Họ và tên: </strong>" + warranty.Name
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Email: </strong>" + warranty.Email
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Số điện thoại: </strong>" + warranty.Phone
                        + "</div>"
                        + "</td>"
                        + "<td>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Sản phẩm bảo hành: </strong>" + warranty.ProductName
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Mã seri sản phẩm: </strong>" + warranty.ProductSeri
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong><strong>Đại lý: </strong>" + warranty.StoreCode
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Ngày đăng ký bảo hành: </strong>" + warranty.CreatedDate.ToString("dd/MM/yyyy - HH:mm:ss tt zz")
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Ngày bắt đầu bảo hành: </strong>" + warranty.StartDate.ToString("dd/MM/yyyy")
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Ngày hết hạn bảo hành: </strong>" + warranty.ModifiedDate.ToString("dd/MM/yyyy")
                        + "</div>"
                        + "</td>"
                        + "</tr>"
                        + "</tbody>"
                        + "</table>"
                        + "<p>Trong quá trình sử dụng sản phẩm/dịch vụ, nếu có bất kỳ vấn đề hay thắc mắc, hãy liên hệ để chúng tôi có cơ hội được hỗ trợ kịp thời theo thông tin như sau:</p>"
                        + "<table style='border: 1px solid ; width: 100%; '>"
                        + "<thead>"
                        + "<tr>"
                        + "<th style='padding: 10px;border-bottom: 1px solid ;border-right: 1px solid ; '>Tỉnh thành phố</th>"
                        + "<th style='padding: 10px;border-bottom: 1px solid ;border-right: 1px solid ; '>Địa chỉ</th>"
                        + "<th style='padding: 10px;border-bottom: 1px solid ; '>Điện thoại/Zalo</th>"
                        + "</tr>"
                        + "</thead>"
                        + "<tbody>"
                        + "<tr>"
                        + "<td style='border-right: 1px solid ; '>TP.Hà Nội</td>"
                        + "<td style='border-right: 1px solid ; '>433 Nguyễn Khang, Phường Yên Hòa, Quận Cầu Giấy, TP.Hà Nội</td>"
                        + "<td style='text-align: center;'>0934.609.188</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td style='border-right: 1px solid ; '>TP.Hồ Chí Minh</td>"
                        + "<td style='border-right: 1px solid ; '>420 Nơ Trang Long, Phường 13, Quận Bình Thạnh, TP.Hồ Chí Minh</td>"
                        + "<td style='text-align: center;'>0934.317.299</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td style='border-right: 1px solid ; '>TP.Cần Thơ</td>"
                        + "<td style='border-right: 1px solid ; '>Tầng 5 tòa nhà Blossom Building 153Q Trần Hưng Đạo, Phường An Phú, Quận Ninh Kiều, TP.Cần Thơ</td>"
                        + "<td style='text-align: center;'>0916.001.823</td>"
                        + "</tr>"
                        + "</tbody>"
                        + "</table>"
                        + "<p>Một lần nữa, chúng tôi xin chân thành cảm ơn quý khách hàng và hy vọng được đón tiếp, đồng hành trong suốt hành trình khôn lớn của bé.</p>"
                        + "<p>Bộ phận xác nhận bảo hành của chúng tôi sẽ liên hệ để xác nhận thông tin với bạn.</p>"
                        + "<h4>Mọi thắc mắc xin liên hệ:</h4>"
                        + "<p>Chăm sóc khách hàng và chất lượng dịch vụ:</p>"
                        + "<div>"
                        + "<span style='color:#f7657e'>Điện thoại/Zalo: </span><a href='tel:+84936268085'>0936.268.085</a>"
                        + "</div>"
                        + "<div>"
                        + "Email: <a href='mailto:info@vietlife.com.vn'>info@vietlife.com.vn</a>"
                        + "</div>"
                        + "<div>"
                        + "Website: <a href='https://spectra.vn'>spectra.vn</a>"
                        + "</div>"
                        + "<div>"
                        + "Facebook: <a href='https://www.facebook.com/spectra.vn'>Spectra.VN</a>"
                        + "</div>"
                        + "<h3>Cảm ơn quý khách đã tin tưởng sử dụng sản phẩm của chúng tôi!</h3></div>";

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.mailer.inet.vn",
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
        // GET: api/Warranties/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWarranty([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var warranty = await _context.Warranties.FindAsync(id);

            if (warranty == null)
            {
                return NotFound();
            }

            return Ok(warranty);
        }

        // PUT: api/Warranties/5
        [HttpPost]
        [Route("PutWarranty")]
        public async Task<IActionResult> PutWarranty([FromBody] Warranty warranty)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(warranty).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(warranty);
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return NoContent();
        }

        // POST: api/Warranties
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostWarranty([FromBody] Warranty warranty)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            warranty.CreatedDate = DateTime.Now;
            _context.Warranties.Add(warranty);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWarranty", new { id = warranty.Id }, warranty);
        }

        // DELETE: api/Warranties/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarranty([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var warranty = await _context.Warranties.FindAsync(id);
            if (warranty == null)
            {
                return NotFound();
            }

            _context.Warranties.Remove(warranty);
            await _context.SaveChangesAsync();

            return Ok(warranty);
        }

        private bool WarrantyExists(int? id)
        {
            return _context.Warranties.Any(e => e.Id == id);
        }
    }
}