using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Spectra.Models;

namespace Spectra.Controllers
{
    [EnableCors("AddCors")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountUserController : ControllerBase
    {
        private static Random random = new Random();
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public AccountUserController(AppDBContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: api/AccountUser
        [HttpGet]
        public IEnumerable<AccountUser> GetAccountUsers()
        {
            return _context.AccountUsers
                .AsNoTracking()
                .Where(b => b.Status)
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        [HttpGet]
        [Route("getAll")]
        [AllowAnonymous]
        public IEnumerable<AccountUser> GetAllAccountUsers()
        {
            return _context.AccountUsers
                .AsNoTracking()
                .Select(x => new AccountUser
                {
                    Email = x.Email,
                    Phone = x.Phone
            // Thêm các trường cần thiết khác nếu có
        })
                .ToList();
        }


        [HttpGet]
        [Route("TrashAccountUsers")]
        public IEnumerable<AccountUser> GetTrashAccountUsers()
        {
            return _context.AccountUsers.Where(b => b.Status == false);
        }

        // GET: api/AccountUser/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountUser([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var accountUser = await _context.AccountUsers.FindAsync(id);

            if (accountUser == null)
            {
                return NotFound();
            }
            accountUser.Password = null;

            return Ok(accountUser);
        }

        // PUT: api/AccountUser/5
        [HttpPost]
        [Route("PutAccountUser")]
        public async Task<IActionResult> PutAccountUser([FromBody] AccountUser accountUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(accountUser).State = EntityState.Modified;

            try
            {
                accountUser.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        [HttpPost]
        [Route("RepeatAccountUsers")]
        public async Task<IActionResult> RepeatAccountUsers([FromBody] AccountUser accountUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(accountUser).State = EntityState.Modified;

            try
            {
                accountUser.Status = true;
                accountUser.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        [HttpPost]
        [Route("TemporaryDelete")]
        public async Task<IActionResult> TemporaryDelete([FromBody] AccountUser accountUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(accountUser).State = EntityState.Modified;

            try
            {
                accountUser.Status = false;
                //categoryProduct.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/AccountUser
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostAccountUser([FromBody] AccountUser accountUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(accountUser.Password); // Mã hóa
            var user = new AccountUser
            {
                Code = accountUser.Code,
                Name = accountUser.Name,
                Email = accountUser.Email,
                Phone = accountUser.Phone,
                Gender = accountUser.Gender,
                Status = true,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Password = hashedPassword
            };
            _context.AccountUsers.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccountUser", new { id = accountUser.Id }, accountUser);
        }
        [HttpGet]
        [Route("excel")]
        public async Task<FileResult> ExportExcel()
        {
            var data = await _context.AccountUsers.ToListAsync();
            var fileName = "khach-hang.xlsx";
            return GenrateExcel(fileName, data);

        }

        private FileResult GenrateExcel(string filename, IEnumerable<AccountUser> accountUsers)
        {
            DataTable dataTable = new DataTable("dbo.Spectra_AccountUser");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Mã khách hàng"),
                new DataColumn("Tên"),
                new DataColumn("Email"),
                new DataColumn("Số điện thoại"),
                new DataColumn("Ngày đăng ký tài khoản")
            });

            foreach (var acc in accountUsers)
            {
                dataTable.Rows.Add(acc.Code,acc.Name, acc.Email, acc.Phone, acc.CreatedDate);
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
        // DELETE: api/AccountUser/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccountUser([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accountUser = await _context.AccountUsers.FindAsync(id);
            if (accountUser == null)
            {
                return NotFound();
            }

            _context.AccountUsers.Remove(accountUser);
            await _context.SaveChangesAsync();

            return Ok(accountUser);
        }

        private bool AccountUserExists(int? id)
        {
            return _context.AccountUsers.Any(e => e.Id == id);
        }

        // POST: api/AccountUsers/Login
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] AccountUserLogin login)
        {
            // Tìm user theo email hoặc số điện thoại và status = true
            var result = _context.AccountUsers
                .FirstOrDefault(acc =>
                    acc.Status == true &&
                    (acc.Email == login.Emailorphone || acc.Phone == login.Emailorphone)
                );

            // Nếu tìm thấy và mật khẩu khớp
            if (result != null && BCrypt.Net.BCrypt.Verify(login.Password, result.Password))
            {
                var token = GenerateJwtToken(result);

                return Ok(new
                {
                    Token = token,
                    User = new
                    {
                        Id = result.Id,
                        Email = result.Email,
                        Phone = result.Phone,
                        Name = result.Name
                    }
                });
            }

            return Unauthorized();
        }


        private string GenerateJwtToken(AccountUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? user.Phone),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("Name", user.Name ?? ""),
                new Claim("Phone", user.Phone ?? ""),
                new Claim("Email", user.Email ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // POST: api/AccountUsers/ChangePassword/1
        [HttpPost]
        [Route("ChangePassword/{id}")]
        public async Task<IActionResult> ChangePassword([FromRoute] int? id, [FromBody] string password)
        {
            var userIdFromToken = User.FindFirst("UserId")?.Value;

            if (userIdFromToken != id?.ToString())
                return Forbid();

            var result = await _context.AccountUsers.FindAsync(id);
            if (result != null)
            {
                result.Password = BCrypt.Net.BCrypt.HashPassword(password);
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        [HttpPost]
        [Route("SendEmail")]
        [AllowAnonymous]
        public ActionResult SendEmail([FromBody] string email)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string new_pass = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            try
            {
                if (ModelState.IsValid)
                {
                    var account = _context.AccountUsers.Where(p => p.Email == email).FirstOrDefault();
                    if (account == null)
                    {
                        return NotFound("Email not exist");
                    }
                    else
                    {
                        var senderEmail = new MailAddress("mayhutsuaspectra@gmail.com", "Spectra");
                        var receiverEmail = new MailAddress(email, "Receiver");
                        var password = "mieopkmqngqmotfk";
                        var subject = "Thư yêu cầu thay đổi mật khẩu của bạn";
                        var body = "<p>Xin chào,</p>" + "<p>Bạn đã yêu cầu đặt lại mật khẩu của mình.</p>"
                        + "<p> bên dưới để thay đổi mật khẩu của bạn:</p>"
                        + "<h4>Mật khẩu mới của bạn là : <b>" + new_pass + "</b></h4>"
                        + "<h3 style='color: red;'><i>Vui lòng không chia sẻ email này cho bất kì ai!</i></h3>"
                        + "<br><p>Liên kết này sẽ hết hạn trong vòng một giờ tới. "
                        + "<b>(If this is a spam message, please click  it is not spam)<b>";
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
                        account.Password = new_pass;
                        _context.Update(account);
                        _context.SaveChanges();
                    }
                    return NoContent();
                }
            }
            catch (Exception)
            {

            }
            return NoContent();
        }
        
    }
}