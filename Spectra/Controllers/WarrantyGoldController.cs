using System;
using System.Collections.Generic;
using System.Data;
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
    public class WarrantyGoldController : ControllerBase
    {
        private readonly AppDBContext _context;

        public WarrantyGoldController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/WarrantyGold
        [HttpGet]
        public IEnumerable<WarrantyGold> GetWarrantyGolds()
        {
            return _context.WarrantyGolds;
        }

        [HttpGet]
        [Route("WrantyGoldPage")]
        public IActionResult WarrantyResult(int? page, int pagesize = 5)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var countDetails = _context.WarrantyGolds.AsNoTracking().Count();
                var currentPage = page ?? 1;
                using (var context = _context)
                {
                    var warantyQuery = context.WarrantyGolds
                        .Select(x => new WarrantyGoldDisplay
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Email = x.Email,
                            Phone = x.Phone,
                            ProductName = x.ProductName,
                            ProductSeri = x.ProductSeri,
                            DateBuy = x.DateBuy,
                            GtriHĐ = x.GtriHĐ,
                            PhiDVBH = x.PhiDVBH,
                            ModifiedDate = x.ModifiedDate,
                            StartDate = x.StartDate,
                            Address = x.Address,
                            Status = x.Status,
                            CreatedDate = x.CreatedDate
                            
                        })
                        .OrderByDescending(x => x.Id)
                        .AsNoTracking()
                        .Skip((currentPage - 1) * pagesize)
                        .Take(pagesize);

                    var result = new PageResult<WarrantyGoldDisplay>
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
        [AllowAnonymous]
        public async Task<FileResult> ExportExcel()
        {
            var data = await _context.WarrantyGolds.ToListAsync();
            var fileName = "baohanhgoldSpectra.xlsx";
            return GenrateExcel(fileName, data);

        }

        private FileResult GenrateExcel(string filename, IEnumerable<WarrantyGold> warrantyGolds)
        {
            DataTable dataTable = new DataTable("dbo.Spectra_WarrantyGold");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Tên"),
                new DataColumn("Email"),
                new DataColumn("Số điện thoại"),
                new DataColumn("Địa chỉ"),
                new DataColumn("Sản phẩm"),
                new DataColumn("Seri sản phẩm"),
                new DataColumn("Ngày mua sản phẩm"),
                new DataColumn("Giá trị hợp đồng"),
                new DataColumn("Phí dịch vụ bảo hành"),
                new DataColumn("Ngày đăng ký"),
                new DataColumn("File hợp đồng"),
                new DataColumn("Ngày bắt đầu BH"),
                new DataColumn("Ngày hết hạn BH"),
            });
            string fileUrlPrefix = "https://spectrababy.com.vn/dataApi/images/";
            foreach (var warranty in warrantyGolds)
            {
                dataTable.Rows.Add(warranty.Name, warranty.Email, warranty.Phone,warranty.Address, warranty.ProductName,
                                    warranty.ProductSeri, warranty.DateBuy.ToString("dd/MM/yyyy"), warranty.GtriHĐ, warranty.PhiDVBH, warranty.CreatedDate.ToString("dd/MM/yyyy"), fileUrlPrefix + warranty.File, warranty.StartDate.ToString("dd/MM/yyyy"), warranty.ModifiedDate.ToString("dd/MM/yyyy"));
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
        [HttpPost]
        [Route("SendEmailWarranty")]
        [AllowAnonymous]
        public ActionResult SendEmailWarranty([FromBody] WarrantyGold warrantyGold)
        {
            var date = DateTime.Now;
            string fileUrlPrefix = "https://spectrababy.com.vn/dataApi/images/";
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("info@vietlife.com.vn", "Spectra Việt Nam");
                    var receiverEmail = new MailAddress(warrantyGold.Email, "Receiver");
                    var password = "Welc0me!!!";
                    var subject = "HỢP ĐỒNG BẢO HÀNH VÀNG SẢN PHẨM SPECTRA - " + warrantyGold.ProductName;
                    string body = "<div style='font-family: unset; font-size: 15px;'>"
                        + "<p style='text-align: center;'><img style='margin-left: 45px;' src='https://spectra.vn/assets/images/logo/logo_black_1x.png'></p>"
                        + "<p style='text-align: center;'><img src='https://spectra.vn/assets/images/output-onlinegiftools.gif'></p>"
                        + "<h2 style='color: #10cb04;font-size: 22px;'>Bạn đã đăng ký hợp đồng bảo hành vàng thành công sản phẩm của Spectra.</h2>"
                        + "<div style='text-align: left;'>"
                        + "<h3>Kính gửi: Quý khách hàng thân mến,</h3>"
                        + "<p>Spectra xin chân thành cảm ơn vì đã tin tưởng và lựa chọn sản phẩm và dịch vụ của chúng tôi. Đây là một niềm vinh dự lớn lao của Spectra khi được đồng hành cùng Mẹ & Bé.</p>"
                        + "<h5 style='font-size: 16px;'>Thông tin hợp đồng bảo hành vàng sản phẩm:</h5>"
                        + "</div>"
                        + "<table style='border: 1px solid; width: 100%; '>"
                        + "<thead>"
                        + "<tr>"
                        + "<th style='padding: 10px;border-bottom: 1px solid;border-right: 1px solid; '>Thông tin khách hàng</th>"
                        + "<th style='padding: 10px;border-bottom: 1px solid; '>Thông tin hợp đồng bảo hành vàng Spectra</th>"
                        + "</tr>"
                        + "</thead>"
                        + "<tbody>"
                        + "<tr>"
                        + "<td style='border-right: 1px solid;'>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Họ và tên: </strong>" + warrantyGold.Name
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Email: </strong>" + warrantyGold.Email
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Số điện thoại: </strong>" + warrantyGold.Phone
                        + "</div>"
                        + "</td>"
                        + "<td>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Sản phẩm bảo hành: </strong>" + warrantyGold.ProductName
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Mã seri sản phẩm: </strong>" + warrantyGold.ProductSeri
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong><strong>Ngày mua sản phẩm: </strong>" + warrantyGold.DateBuy
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong><strong>Giá trị hợp đồng bảo hành: </strong>" + warrantyGold.GtriHĐ + " VNĐ"
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong><strong>Phí dịch vụ bảo hành: </strong>" + warrantyGold.PhiDVBH + " VNĐ"
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Ngày đăng ký bảo hành: </strong>" + date.ToString("dd/MM/yyyy")
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Ngày bắt đầu bảo hành: </strong>" + warrantyGold.StartDate.ToString("dd/MM/yyyy")
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Ngày hết hạn bảo hành: </strong>" + warrantyGold.ModifiedDate.ToString("dd/MM/yyyy")
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>File hợp đồng: </strong> <a href='" + fileUrlPrefix + warrantyGold.File + "'>" + fileUrlPrefix + warrantyGold.File + "</a>"
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
        public ActionResult SendEmailWarrantyConfirm([FromBody] WarrantyGold warrantyGold)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string fileUrlPrefix = "https://spectrababy.com.vn/dataApi/images/";
                    var senderEmail = new MailAddress("info@vietlife.com.vn", "Spectra Việt Nam");
                    var receiverEmail = new MailAddress(warrantyGold.Email, "Receiver");
                    var password = "Welc0me!!!";
                    var subject = "EMAIL XÁC NHẬN HỢP ĐỒNG BẢO HÀNH VÀNG SẢN PHẨM SPECTRA - " + warrantyGold.ProductName;
                    string body = "<div style='font-family: unset; font-size: 15px;'>"
                        + "<p style='text-align: center;'><img style='margin-left: 45px;' src='https://spectra.vn/assets/images/logo/logo_black_1x.png'></p>"
                        + "<h2 style='color: #10cb04;font-size: 22px;'>Đây là Mail xác nhận hợp đồng bảo hành vàng từ phía Spectra.</h2>"
                        + "<div style='text-align: left;'>"
                        + "<h3>Kính gửi: Quý khách hàng thân mến,</h3>"
                        + "<p>Spectra xin chân thành cảm ơn vì đã tin tưởng và lựa chọn sản phẩm và dịch vụ của chúng tôi. Đây là một niềm vinh dự lớn lao của Spectra khi được đồng hành cùng Mẹ & Bé.</p>"
                        + "<p>Từ phía Spectra, chúng tôi đã nhận được thông tin đăng ký hợp đồng bảo hành vàng từ phía bạn, dưới đây là thông tin hợp đồng bảo hành sau khi chúng tôi đã xem xét và xác nhận.</p>"
                        + "<h5 style='font-size: 16px;'>Thông tin hợp đồng bảo hành sản phẩm:</h5>"
                        + "</div>"
                        + "<table style='border: 1px solid; width: 100%; '>"
                        + "<thead>"
                        + "<tr>"
                        + "<th style='padding: 10px;border-bottom: 1px solid;border-right: 1px solid; '>Thông tin khách hàng</th>"
                        + "<th style='padding: 10px;border-bottom: 1px solid; '>Thông tin bảo hành vàng Spectra</th>"
                        + "</tr>"
                        + "</thead>"
                        + "<tbody>"
                        + "<tr>"
                        + "<td style='border-right: 1px solid;'>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Họ và tên: </strong>" + warrantyGold.Name
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Email: </strong>" + warrantyGold.Email
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Số điện thoại: </strong>" + warrantyGold.Phone
                        + "</div>"
                        + "</td>"
                        + "<td>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Sản phẩm bảo hành: </strong>" + warrantyGold.ProductName
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Mã seri sản phẩm: </strong>" + warrantyGold.ProductSeri
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong><strong>Ngày mua sản phẩm: </strong>" + warrantyGold.DateBuy
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong><strong>Giá trị hợp đồng bảo hành: </strong>" + warrantyGold.GtriHĐ + " VNĐ"
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong><strong>Phí dịch vụ bảo hành: </strong>" + warrantyGold.PhiDVBH + " VNĐ"
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Ngày đăng ký bảo hành: </strong>" + warrantyGold.CreatedDate.ToString("dd/MM/yyyy")
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Ngày bắt đầu bảo hành: </strong>" + warrantyGold.StartDate.ToString("dd/MM/yyyy")
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>Ngày hết hạn bảo hành: </strong>" + warrantyGold.ModifiedDate.ToString("dd/MM/yyyy")
                        + "</div>"
                        + "<div style='padding: 10px '>"
                        + "<strong>File hợp đồng: </strong> <a href='" + fileUrlPrefix + warrantyGold.File + "'>" + fileUrlPrefix + warrantyGold.File + "</a>"
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
        // GET: api/WarrantyGold/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWarrantyGold([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var warrantyGold = await _context.WarrantyGolds.FindAsync(id);

            if (warrantyGold == null)
            {
                return NotFound();
            }

            return Ok(warrantyGold);
        }

        // PUT: api/WarrantyGold/5
        [HttpPost]
        [Route("PutWarrantyGold")]
        public async Task<IActionResult> PutWarrantyGold([FromBody] WarrantyGold warrantyGold)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(warrantyGold).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
            }

            return NoContent();
        }

        // POST: api/WarrantyGold
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostWarrantyGold([FromBody] WarrantyGold warrantyGold)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            warrantyGold.CreatedDate = DateTime.Now;
            _context.WarrantyGolds.Add(warrantyGold);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWarrantyGold", new { id = warrantyGold.Id }, warrantyGold);
        }

        // DELETE: api/WarrantyGold/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarrantyGold([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var warrantyGold = await _context.WarrantyGolds.FindAsync(id);
            if (warrantyGold == null)
            {
                return NotFound();
            }

            _context.WarrantyGolds.Remove(warrantyGold);
            await _context.SaveChangesAsync();

            return Ok(warrantyGold);
        }

        private bool WarrantyGoldExists(int? id)
        {
            return _context.WarrantyGolds.Any(e => e.Id == id);
        }
    }
}