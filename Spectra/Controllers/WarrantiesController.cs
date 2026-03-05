using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectra.Models;
using Spectra.Models.Authorize;

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
        [BinaryAuthorize("Warranty", ActionType.Xem)]
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
                            Name = x.Name,
                            Email = x.Email,
                            Phone = x.Phone,
                            ProductName = x.ProductName,
                            ProductSeri = x.ProductSeri,
                            Image = x.Image,
                            Status = x.Status,
                            StoreCode = x.StoreCode,
                            StartDate = x.StartDate,
                            CreatedDate = x.CreatedDate,
                            ModifiedDate = x.ModifiedDate
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
        [BinaryAuthorize("Warranty", ActionType.Xem)]
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
                        .AsNoTracking()
                        .Where(s => s.Status == true)
                        .OrderByDescending(x => x.Id)
                        .Skip((currentPage - 1) * pagesize)
                        .Take(pagesize)
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
                        });

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
        [BinaryAuthorize("Warranty", ActionType.XuatFile)]
        public async Task<FileResult> ExportExcel(string query = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var data = await _context.Warranties.ToListAsync();

            // Lọc theo query
            if (!string.IsNullOrEmpty(query))
            {
                var normalizedQuery = RemoveVietnameseTones(query).ToLower();
                data = data.Where(e =>
                    (e.Name != null && RemoveVietnameseTones(e.Name).ToLower().Contains(normalizedQuery)) ||
                    (e.Email != null && RemoveVietnameseTones(e.Email).ToLower().Contains(normalizedQuery)) ||
                    (e.Phone != null && RemoveVietnameseTones(e.Phone).ToLower().Contains(normalizedQuery)) ||
                    (e.ProductName != null && RemoveVietnameseTones(e.ProductName).ToLower().Contains(normalizedQuery)) ||
                    (e.ProductSeri != null && RemoveVietnameseTones(e.ProductSeri).ToLower().Contains(normalizedQuery)) ||
                    (e.StoreCode != null && RemoveVietnameseTones(e.StoreCode).ToLower().Contains(normalizedQuery))
                ).ToList();
            }

            // Lọc theo ngày
            if (startDate.HasValue && endDate.HasValue)
            {
                data = data.Where(e => e.CreatedDate >= startDate && e.CreatedDate <= endDate).ToList();
            }
            else if (startDate.HasValue)
            {
                data = data.Where(e => e.CreatedDate >= startDate).ToList();
            }
            else if (endDate.HasValue)
            {
                data = data.Where(e => e.CreatedDate <= endDate).ToList();
            }

            Console.WriteLine($"Số bản ghi /excel: {data.Count}");
            var fileName = "baohanh.xlsx";
            return GenerateExcel(fileName, data);
        }
        private string RemoveVietnameseTones(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var normalized = text.Normalize(NormalizationForm.FormD);
            var result = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    result.Append(c);
                }
            }

            return result.ToString()
                .Replace("đ", "d")
                .Replace("Đ", "D")
                .Normalize(NormalizationForm.FormC);
        }
        private FileResult GenerateExcel(string fileName, IEnumerable<Warranty> warranties)
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
                dataTable.Rows.Add(
                    warranty.Name,
                    warranty.Email,
                    warranty.Phone,
                    warranty.ProductName,
                    warranty.ProductSeri,
                    warranty.Description,
                    warranty.StoreCode,
                    warranty.CreatedDate.ToString("dd/MM/yyyy"),
                    warranty.StartDate.ToString("dd/MM/yyyy"),
                    warranty.ModifiedDate.ToString("dd/MM/yyyy")
                );
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
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
                  StoreCode = x.StoreCode,
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
                        + "<td style='border-right: 1px solid ; '>193 Nguyễn Văn Thương, Phường 25, Quận Bình Thạnh, TP.Hồ Chí Minh</td>"
                        + "<td style='text-align: center;'>0934.317.299</td>"
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
                        Host = "zmhn092403.onemail.vn",
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
        [BinaryAuthorize("Warranty", ActionType.Sua)]
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
                        + "<td style='border-right: 1px solid ; '>193 Nguyễn Văn Thương, Phường 25, Quận Bình Thạnh, TP.Hồ Chí Minh</td>"
                        + "<td style='text-align: center;'>0934.317.299</td>"
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
                        Host = "zmhn092403.onemail.vn",
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
        [BinaryAuthorize("Warranty", ActionType.Xem)]
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
        [BinaryAuthorize("Warranty", ActionType.Sua)]
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

        [HttpGet]
        [Route("WarrantiesLast6Months")]
        [BinaryAuthorize("Dashboard", ActionType.Xem)]
        public async Task<ActionResult<IEnumerable<MonthlyWarrantyData>>> GetWarrantiesLast6Months()
        {
            try
            {
                // Calculate the date 6 months ago from the current date.
                var sixMonthsAgo = DateTime.Now.AddMonths(-6);

                // Define the culture for month formatting (e.g., for "M/yyyy" or "MM/yyyy").
                // Using "vi-VN" culture for potentially localized month names if you change the format later,
                // but for "M/yyyy", it won't have a significant visual difference compared to InvariantCulture.
                var culture = new CultureInfo("en-US"); // Using en-US for consistent numeric month/year format

                // Query the database to get warranty data within the last 6 months,
                // group it by year and month, and count the total for each group.
                var monthlyData = await _context.Warranties
                    .Where(w => w.CreatedDate >= sixMonthsAgo) // Filter data from the last 6 months
                    .GroupBy(w => new { w.CreatedDate.Year, w.CreatedDate.Month })
                    .Select(g => new MonthlyWarrantyData
                    {
                        MonthLabel = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("M/yyyy", culture), // Format: "Month/Year" (e.g., "5/2025")
                TotalWarranties = g.Count() // Count warranties in each month
            })
                    .ToListAsync();

                // Create a list to hold the final 6-month data, ensuring all months are present.
                var result = new List<MonthlyWarrantyData>();
                var currentMonth = DateTime.Now;

                // Iterate for the last 6 months (including the current month if applicable).
                for (int i = 5; i >= 0; i--)
                {
                    var monthToConsider = currentMonth.AddMonths(-i);
                    var monthLabel = monthToConsider.ToString("M/yyyy", culture);

                    // Find if data for this month already exists in the queried 'monthlyData'.
                    var existingData = monthlyData.FirstOrDefault(d => d.MonthLabel == monthLabel);

                    if (existingData != null)
                    {
                        result.Add(existingData);
                    }
                    else
                    {
                        // If no data exists for a month, add it with a count of 0.
                        result.Add(new MonthlyWarrantyData { MonthLabel = monthLabel, TotalWarranties = 0 });
                    }
                }

                // Order the final result by date to ensure proper chronological display on charts.
                result = result.OrderBy(x => DateTime.ParseExact(x.MonthLabel, "M/yyyy", culture)).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes.
                Console.Error.WriteLine($"Error fetching monthly warranty data: {ex.Message}");
                // Return a 500 Internal Server Error status code with a descriptive message.
                return StatusCode(500, "An error occurred while retrieving monthly warranty data. Please try again later.");
            }
        }

        // DELETE: api/Warranties/5
        [HttpDelete("{id}")]
        [BinaryAuthorize("Warranty", ActionType.Xoa)]
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