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
using Newtonsoft.Json;
using Spectra.Models;
using Spectra.Models.Authorize;

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
        [BinaryAuthorize("User", ActionType.Xem)]
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
        [BinaryAuthorize("User", ActionType.Xem)]
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
        [BinaryAuthorize("User", ActionType.Xoa)]
        public IEnumerable<AccountUser> GetTrashAccountUsers()
        {
            return _context.AccountUsers.Where(b => b.Status == false);
        }

        // GET: api/AccountUser/5
        [HttpGet("{id}")]
        [BinaryAuthorize("User", ActionType.Xem)]
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
        [AllowAnonymous]
        //[BinaryAuthorize("User", ActionType.Sua)]
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
        [BinaryAuthorize("User", ActionType.Xoa)]
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
        [BinaryAuthorize("User", ActionType.Xoa)]
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
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(accountUser.Password);

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

                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
                if (role == null)
                    return BadRequest("Vai trò 'Customer' không tồn tại");

                _context.UserRoleCustomers.Add(new UserRoleCustomer
                {
                    AccountUserId = user.Id,
                    RolesId = role.Id
                });
                await _context.SaveChangesAsync();

                var modules = await _context.Modules.ToListAsync();
                var permissionList = new List<Permissions>();

                var orderModule = modules.FirstOrDefault(m => m.Name == "Order");
                if (orderModule != null)
                {
                    permissionList.Add(new Permissions
                    {
                        RolesId = role.Id,
                        ModulesId = orderModule.Id,
                        PermissionValue = 1 // View
                    });
                }

                var userModule = modules.FirstOrDefault(m => m.Name == "User");
                if (userModule != null)
                {
                    permissionList.Add(new Permissions
                    {
                        RolesId = role.Id,
                        ModulesId = userModule.Id,
                        PermissionValue = 5 // Edit
                    });
                }

                _context.Permissions.AddRange(permissionList);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAccountUser", new { id = user.Id }, accountUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }



        [HttpGet]
        [Route("excel")]
        [BinaryAuthorize("User", ActionType.XuatFile)]
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
        [BinaryAuthorize("User", ActionType.Xoa)]
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
        public async Task<IActionResult> Login([FromBody] AccountUserLogin login)
        {
            // Tìm user theo email hoặc số điện thoại và còn hoạt động
            var account = await _context.AccountUsers
                .Include(u => u.UserRoleCustomers)
                .ThenInclude(ur => ur.Roles)
                .FirstOrDefaultAsync(u =>
                    u.Status == true &&
                    (u.Email == login.Emailorphone || u.Phone == login.Emailorphone)
                );

            if (account == null || !BCrypt.Net.BCrypt.Verify(login.Password, account.Password))
                return BadRequest("Sai mật khẩu hoặc tài khoản đã bị khóa");

            // Lấy quyền từ tất cả role → tổng hợp từng module bằng bitwise OR
            var permissions = await (from ur in _context.UserRoleCustomers
                                     join rp in _context.Permissions on ur.RolesId equals rp.RolesId
                                     join m in _context.Modules on rp.ModulesId equals m.Id
                                     where ur.AccountUserId == account.Id
                                     select new
                                     {
                                         Module = m.Name,
                                         Permission = rp.PermissionValue
                                     }).ToListAsync();

            var permissionDict = permissions
                .GroupBy(p => p.Module)
                .ToDictionary(
                    g => g.Key,
                    g => g.Aggregate(0, (acc, val) => acc | val.Permission) // tổng hợp bitwise OR
                );

            // Tạo danh sách claim
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, account.Name ?? ""),
        new Claim("UserId", account.Id.ToString()),
        new Claim("Phone", account.Phone ?? ""),
        new Claim("Email", account.Email ?? ""),
        new Claim("Permissions", JsonConvert.SerializeObject(permissionDict)) // serialize quyền
    };

            var token = GenerateJwtToken(claims);

            return Ok(new
            {
                Token = token,
                User = new
                {
                    Id = account.Id,
                    Email = account.Email,
                    Phone = account.Phone,
                    Name = account.Name
                }
            });
        }

        private string GenerateJwtToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public class ChangePasswordDto
        {
            public string PasswordOld { get; set; }
            public string Password { get; set; }
        }

        // POST: api/AccountUsers/ChangePassword/1
        [HttpPost]
        [Route("ChangePassword/{id}")]
        [BinaryAuthorize("User", ActionType.Sua)]
        public async Task<IActionResult> ChangePassword([FromRoute] int? id, [FromBody] ChangePasswordDto model)
        {
            var userIdFromToken = User.FindFirst("UserId")?.Value;

            if (userIdFromToken != id?.ToString())
                return Forbid();

            var user = await _context.AccountUsers.FindAsync(id);
            if (user == null)
                return NotFound("Tài khoản không tồn tại.");

            // Kiểm tra mật khẩu cũ
            if (!BCrypt.Net.BCrypt.Verify(model.PasswordOld, user.Password))
                return BadRequest("Mật khẩu hiện tại không chính xác.");

            // Cập nhật mật khẩu mới
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            await _context.SaveChangesAsync();

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
                        account.Password = BCrypt.Net.BCrypt.HashPassword(new_pass);
                        _context.Update(account);
                        _context.SaveChanges();
                    }
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
            return NoContent();
        }
        
    }
}