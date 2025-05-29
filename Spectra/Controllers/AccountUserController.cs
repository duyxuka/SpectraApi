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
        [BinaryAuthorize("UserManager", ActionType.Xem)]
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
        [BinaryAuthorize("UserManager", ActionType.Xem)]
        public IEnumerable<AccountUser> GetAllAccountUsers()
        {
            return _context.AccountUsers
                .AsNoTracking()
                .Select(x => new AccountUser
                {
                    Email = x.Email,
                    Phone = x.Phone
                }).ToList();
        }

        [HttpGet]
        [Route("LoyalCustomers")]
        [BinaryAuthorize("Dashboard", ActionType.Xem)]
        public async Task<IActionResult> GetLoyalCustomers(int minOrders = 2, int months = 6)
        {
            // Kiểm tra tính hợp lệ của Model State
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Tính toán ngày bắt đầu khoảng thời gian xét duyệt
                var startDate = DateTime.Now.AddMonths(-months);

                // Bước 1: Lấy các đơn hàng trong khoảng thời gian đã cho
                // và nhóm theo AccountUserId để đếm số lượng đơn hàng và ngày đặt hàng cuối cùng.
                // AsNoTracking() được sử dụng để tối ưu hiệu suất cho truy vấn chỉ đọc.
                var customerOrderStats = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.CreatedDate >= startDate) // Lọc đơn hàng theo thời gian
                    .GroupBy(o => o.AccountUserId)
                    .Select(g => new
                    {
                        AccountUserId = g.Key,
                        OrderCount = g.Count(), // Đếm số lượng đơn hàng của mỗi khách hàng
                        LastOrderDate = g.Max(o => o.CreatedDate) // Lấy ngày đặt hàng cuối cùng
                    })
                    .Where(g => g.OrderCount >= minOrders) // Lọc những khách hàng có số đơn hàng tối thiểu
                    .ToListAsync(); // Thực thi truy vấn đến đây để lấy dữ liệu thống kê khách hàng

                // Nếu không có khách hàng nào đủ điều kiện, trả về kết quả rỗng sớm
                if (!customerOrderStats.Any())
                {
                    return Ok(new { LoyalCustomerCount = 0, Customers = new List<object>() });
                }

                // Bước 2: Lấy thông tin chi tiết của các AccountUser dựa trên AccountUserId đã tìm được
                // Điều này giúp tránh việc join toàn bộ bảng AccountUsers nếu có quá nhiều user.
                var loyalCustomerIds = customerOrderStats.Select(s => s.AccountUserId).ToList();
                var accountUsers = await _context.AccountUsers
                    .AsNoTracking()
                    .Where(u => loyalCustomerIds.Contains(u.Id))
                    .Select(u => new
                    {
                        u.Id,
                        u.Email,
                        // Giả sử tên khách hàng được lưu ở trường FullName, PhoneNumber ở trường Phone
                        // Bạn cần thay đổi tên trường cho phù hợp với cấu trúc DB của bạn
                        FullName = u.Name, // Tên đầy đủ của khách hàng
                        PhoneNumber = u.Phone // Số điện thoại của khách hàng
            })
                    .ToListAsync();

                // Bước 3: Kết hợp thông tin thống kê đơn hàng với thông tin chi tiết khách hàng
                var loyalCustomers = customerOrderStats
                    .Join(accountUsers,
                        stats => stats.AccountUserId,
                        user => user.Id,
                        (stats, user) => new
                        {
                            AccountUserId = stats.AccountUserId,
                            Email = user.Email,
                            FullName = user.FullName,      // Thêm tên khách hàng
                            PhoneNumber = user.PhoneNumber, // Thêm số điện thoại
                            OrderCount = stats.OrderCount,
                            LastOrderDate = stats.LastOrderDate
                        })
                    .OrderByDescending(c => c.OrderCount)
                    .Take(5)// Sắp xếp lại theo số lượng đơn hàng
                    .ToList(); // Chuyển kết quả cuối cùng thành List

                // Trả về số lượng và danh sách khách hàng thân thiết
                return Ok(new { LoyalCustomerCount = loyalCustomers.Count, Customers = loyalCustomers });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy dữ liệu khách hàng thân thiết: {ex.Message}");
                // Log lỗi chi tiết hơn nếu có thể
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xử lý yêu cầu khách hàng thân thiết.", error = ex.Message, innerError = ex.InnerException?.Message });
            }
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

            // Kiểm tra quyền View cho module UserProfile (dành cho Customer) hoặc UserManager (dành cho Admin)
            bool hasUserProfileViewPermission = HasPermission("User", 1); // 1 = View
            bool isAdmin = HasPermission("UserManager", 1); // Admin có quyền View trên UserManager

            if (!hasUserProfileViewPermission && !isAdmin)
            {
                return Forbid("Bạn không có quyền xem tài khoản này.");
            }

            // Nếu không phải Admin, chỉ cho phép xem tài khoản của chính mình
            if (!isAdmin && userId != id)
            {
                return Forbid("Bạn chỉ có thể xem thông tin tài khoản của chính mình.");
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
        [BinaryAuthorize("User", ActionType.Sua)]
        public async Task<IActionResult> PutAccountUser([FromBody] AccountUser accountUser)
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

            // Kiểm tra quyền Edit cho module UserProfile (dành cho Customer) hoặc UserManager (dành cho Admin)
            bool hasUserProfileEditPermission = HasPermission("User", 4); // 4 = Edit
            bool isAdmin = HasPermission("UserManager", 4); // Admin có quyền Edit trên UserManager
            if (!hasUserProfileEditPermission && !isAdmin)
            {
                return Forbid("Bạn không có quyền chỉnh sửa tài khoản này.");
            }

            // Nếu không phải Admin, chỉ cho phép chỉnh sửa tài khoản của chính mình
            if (!isAdmin && userId != accountUser.Id)
            {
                return Forbid("Bạn chỉ có thể chỉnh sửa thông tin tài khoản của chính mình.");
            }

            // Kiểm tra tài khoản có tồn tại không
            var existingUser = await _context.AccountUsers.FindAsync(accountUser.Id);
            if (existingUser == null)
            {
                return NotFound("Tài khoản không tồn tại.");
            }

            // Cập nhật các thuộc tính từ accountUser
            existingUser.Code = accountUser.Code;
            existingUser.Name = accountUser.Name;
            existingUser.Email = accountUser.Email;
            existingUser.Phone = accountUser.Phone;
            existingUser.Status = accountUser.Status;
            existingUser.ModifiedDate = DateTime.Now;

            // Băm mật khẩu nếu được cung cấp
            if (!string.IsNullOrEmpty(accountUser.Password))
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(accountUser.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Lỗi đồng bộ hóa dữ liệu.");
            }
        }
        private bool HasPermission(string module, int requiredPermission)
        {
            var permissionsClaim = User.FindFirst("Permissions")?.Value;
            if (string.IsNullOrEmpty(permissionsClaim))
                return false;

            var permissions = JsonConvert.DeserializeObject<Dictionary<string, int>>(permissionsClaim);
            return permissions.ContainsKey(module) && (permissions[module] & requiredPermission) == requiredPermission;
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

                // Lấy danh sách module
                var modules = await _context.Modules.ToListAsync();
                var permissionList = new List<Permissions>();

                // Lấy các quyền hiện có của vai trò
                var existingPermissions = await _context.Permissions
                    .Where(p => p.RolesId == role.Id)
                    .ToListAsync();

                var orderModule = modules.FirstOrDefault(m => m.Name == "Order");
                if (orderModule != null && !existingPermissions.Any(p => p.ModulesId == orderModule.Id))
                {
                    permissionList.Add(new Permissions
                    {
                        RolesId = role.Id,
                        ModulesId = orderModule.Id,
                        PermissionValue = 7 // View+Add+Edit
                    });
                }

                var userModule = modules.FirstOrDefault(m => m.Name == "User");
                if (userModule != null && !existingPermissions.Any(p => p.ModulesId == userModule.Id))
                {
                    permissionList.Add(new Permissions
                    {
                        RolesId = role.Id,
                        ModulesId = userModule.Id,
                        PermissionValue = 5 // Edit and View
                    });
                }

                if (permissionList.Any())
                {
                    _context.Permissions.AddRange(permissionList);
                    await _context.SaveChangesAsync();
                }

                return CreatedAtAction("GetAccountUser", new { id = user.Id }, accountUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }



        [HttpGet]
        [Route("excel")]
        [BinaryAuthorize("UserManager", ActionType.XuatFile)]
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
        [BinaryAuthorize("UserManager", ActionType.Xoa)]
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
                expires: DateTime.UtcNow.AddDays(1),
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
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            bool isAdmin = HasPermission("UserManager", 5); // Admin có quyền Edit trên UserManager
            if (!isAdmin && userId != id)
            {
                return Forbid("Bạn chỉ có thể thay đổi mật khẩu của chính mình.");
            }

            var user = await _context.AccountUsers.FindAsync(id);
            if (user == null)
                return NotFound("Tài khoản không tồn tại.");

            if (!BCrypt.Net.BCrypt.Verify(model.PasswordOld, user.Password))
                return BadRequest("Mật khẩu hiện tại không chính xác.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class ForgotEmailRequest
        {
            public string Email { get; set; }
        }

        [HttpPost]
        [Route("SendEmail")]
        [AllowAnonymous]
        public ActionResult SendEmail([FromBody] ForgotEmailRequest request)
        {
            if (string.IsNullOrEmpty(request?.Email))
            {
                return BadRequest("Email is required.");
            }

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string new_pass = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            try
            {
                if (ModelState.IsValid)
                {
                    var account = _context.AccountUsers.FirstOrDefault(p => p.Email == request.Email);
                    if (account == null)
                    {
                        return NotFound("Email not exist");
                    }

                    var senderEmail = new MailAddress("mayhutsuaspectra@gmail.com", "Spectra");
                    var receiverEmail = new MailAddress(request.Email, "Receiver");
                    var password = "kdmlwkyeqazbxloo"; // Cần thay bằng App Password hợp lệ
                    var subject = "Thư yêu cầu thay đổi mật khẩu của bạn";
                    var body = "<p>Xin chào,</p>" +
                               "<p>Bạn đã yêu cầu đặt lại mật khẩu của mình.</p>" +
                               "<p>Mật khẩu mới của bạn là: <b>" + new_pass + "</b></p>" +
                               "<h3 style='color: red;'><i>Vui lòng không chia sẻ email này cho bất kì ai!</i></h3>";

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
                        Console.WriteLine($"Email sent successfully to {request.Email}");
                    }

                    account.Password = BCrypt.Net.BCrypt.HashPassword(new_pass);
                    _context.Update(account);
                    _context.SaveChanges();

                    return NoContent();
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email to {request.Email}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, $"Failed to send email: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("HashAllPasswords")]
        [AllowAnonymous]
        //[BinaryAuthorize("User", ActionType.Sua)]
        public async Task<IActionResult> HashAllPasswords()
        {
            try
            {
                // Lấy tất cả tài khoản
                var users = await _context.AccountUsers
                    .Where(u => u.Status == true) // Chỉ xử lý tài khoản đang hoạt động
                    .ToListAsync();

                int updatedCount = 0;
                foreach (var user in users)
                {
                    // Kiểm tra xem mật khẩu đã được băm chưa
                    // BCrypt hashed passwords thường bắt đầu bằng "$2a$" hoặc "$2b$"
                    if (!string.IsNullOrEmpty(user.Password) && !user.Password.StartsWith("$2"))
                    {
                        // Mã hóa mật khẩu bằng BCrypt
                        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                        user.ModifiedDate = DateTime.Now;
                        _context.Entry(user).State = EntityState.Modified;
                        updatedCount++;
                    }
                }

                if (updatedCount > 0)
                {
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    Message = $"Đã mã hóa thành công {updatedCount} mật khẩu.",
                    TotalUsersProcessed = users.Count,
                    UpdatedCount = updatedCount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi mã hóa mật khẩu: {ex.Message}");
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

    }
}