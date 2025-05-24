using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
    
    public class AccountAdminController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public AccountAdminController(AppDBContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: api/AccountAdmin
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<AdminDTO>> GetAccountAdmins()
        {
            var accounts = await (from ac in _context.AccountAdmins
                                  join ur in _context.UserRoleAdmins on ac.Id equals ur.AccountAdminId into urs
                                  from ur in urs.DefaultIfEmpty()
                                  join ro in _context.Roles on ur.RolesId equals ro.Id into ros
                                  from ro in ros.DefaultIfEmpty()
                                  select new
                                  {
                                      ac.Id,
                                      ac.Code,
                                      ac.Name,
                                      ac.Status,
                                      ac.Email,
                                      RoleName = ro != null ? ro.Name : null
                                  })
                                 .AsNoTracking()
                                 .ToListAsync();

            var accountPermissions = await _context.AccountPermissions
                .Include(ap => ap.Modules)
                .Select(ap => new
                {
                    ap.AccountAdminId,
                    ModuleName = ap.Modules.Name,
                    ap.PermissionValue
                })
                .ToListAsync();

            var grouped = accounts
                .GroupBy(a => new { a.Id, a.Code, a.Name, a.Status, a.Email })
                .Select(g => new AdminDTO
                {
                    Id = g.Key.Id,
                    Code = g.Key.Code,
                    Name = g.Key.Name,
                    Status = g.Key.Status,
                    Email = g.Key.Email,
                    RoleNames = g.Select(x => x.RoleName).Where(r => r != null).Distinct().ToList(),
                    AccountPermissions = accountPermissions
                        .Where(ap => ap.AccountAdminId == g.Key.Id)
                        .GroupBy(ap => ap.ModuleName)
                        .ToDictionary(
                            ap => ap.Key,
                            ap => ap.Aggregate(0, (acc, val) => acc | val.PermissionValue)
                        )
                });

            return grouped;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel user)
        {
            var account = await _context.AccountAdmins
                .Include(u => u.UserRoleAdmins)
                .ThenInclude(ur => ur.Roles)
                .Include(u => u.AccountPermissions) // Thêm quyền tài khoản
                .ThenInclude(ap => ap.Modules) // Liên kết với Modules
                .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (account == null || !PasswordHelper.VerifyPassword(user.Password, account.PasswordHash, account.PasswordSalt) || !account.Status)
                return BadRequest("Sai mật khẩu hoặc tài khoản đã bị khóa");

            // Lấy quyền từ vai trò
            var rolePermissions = await (from ur in _context.UserRoleAdmins
                                         join rp in _context.Permissions on ur.RolesId equals rp.RolesId
                                         join m in _context.Modules on rp.ModulesId equals m.Id
                                         where ur.AccountAdminId == account.Id
                                         select new
                                         {
                                             Module = m.Name,
                                             Permission = rp.PermissionValue
                                         }).ToListAsync();

            // Lấy quyền từ tài khoản
            var accountPermissions = account.AccountPermissions
                .Select(ap => new
                {
                    Module = ap.Modules.Name,
                    Permission = ap.PermissionValue
                }).ToList();

            // Tổng hợp quyền từ vai trò và tài khoản
            var allPermissions = rolePermissions.Concat(accountPermissions)
                .GroupBy(p => p.Module)
                .ToDictionary(
                    g => g.Key,
                    g => g.Aggregate(0, (acc, val) => acc | val.Permission) // Bitwise OR
                );

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Name),
                new Claim("UserId", account.Id.ToString()),
                new Claim("Permissions", JsonConvert.SerializeObject(allPermissions))
            };

            var token = GenerateJwtToken(claims);

            return Ok(new
            {
                token,
                User = new
                {
                    Id = account.Id,
                    Email = account.Email,
                    Name = account.Name,
                    RoleNames = account.UserRoleAdmins.Select(ur => ur.Roles.Name).ToList(),
                    Permissions = allPermissions // Trả về quyền tổng hợp
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

        //private string GenerateMD5(string password)
        //{
        //StringBuilder hash = new StringBuilder();
        // MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
        //byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(password));
        //
        //       for (int i = 0; i < bytes.Length; i++)
        //      {
        //hash.Append(bytes[i].ToString("x2"));
        //}
        //       return hash.ToString();
        //}


        // GET: api/AccountAdmin/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountAdmin([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accountAdmin = await _context.AccountAdmins
                .Include(a => a.AccountPermissions)
                .ThenInclude(ap => ap.Modules)
                .FirstOrDefaultAsync(ac => ac.Id == id);

            if (accountAdmin == null)
            {
                return NotFound();
            }

            var roles = await _context.UserRoleAdmins
                .Where(ur => ur.AccountAdminId == accountAdmin.Id)
                .Join(_context.Roles,
                      ur => ur.RolesId,
                      ro => ro.Id,
                      (ur, ro) => new
                      {
                          ro.Id,
                          ro.Name
                      })
                .ToListAsync();

            var accountPermissions = accountAdmin.AccountPermissions
                .GroupBy(ap => ap.Modules.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Aggregate(0, (acc, ap) => acc | ap.PermissionValue)
                );

            var result = new
            {
                accountAdmin.Id,
                accountAdmin.Code,
                accountAdmin.Name,
                accountAdmin.Status,
                accountAdmin.Email,
                Roles = roles,
                AccountPermissions = accountPermissions
            };

            return Ok(result);
        }


        [HttpGet]
        [Route("GetPermissionsByRoles")]
        public async Task<IActionResult> GetPermissionsByRoles([FromQuery] int[] roleIds, [FromQuery] int? accountId = null)
        {
            if (roleIds == null || roleIds.Length == 0)
            {
                return BadRequest("RoleIds are required.");
            }

            try
            {
                // Lấy tất cả modules
                var modules = await _context.Modules
                    .Select(m => new
                    {
                        m.Id,
                        m.Name
                    })
                    .ToListAsync();

                // Lấy quyền từ vai trò
                var rolePermissions = await _context.Permissions
                    .Where(p => roleIds.Contains(p.RolesId))
                    .Select(p => new PermissionDetail
                    {
                        ModulesId = p.ModulesId,
                        PermissionValue = p.PermissionValue
                    })
                    .ToListAsync();

                List<PermissionDetail> accountPermissions = new List<PermissionDetail>();
                if (accountId.HasValue)
                {
                    accountPermissions = await _context.AccountPermissions
                        .Where(ap => ap.AccountAdminId == accountId.Value)
                        .Select(ap => new PermissionDetail
                        {
                            ModulesId = ap.ModulesId,
                            PermissionValue = ap.PermissionValue
                        })
                        .ToListAsync();
                }

                // Tổng hợp quyền
                var allPermissions = rolePermissions.Concat(accountPermissions)
                    .GroupBy(p => p.ModulesId)
                    .Select(g => new
                    {
                        ModulesId = g.Key,
                        PermissionValue = g.Aggregate(0, (acc, p) => acc | p.PermissionValue)
                    })
                    .ToList();

                // Ánh xạ với modules
                var result = modules.Select(m => new
                {
                    ModulesId = m.Id,
                    ModuleName = m.Name,
                    PermissionValue = allPermissions.FirstOrDefault(p => p.ModulesId == m.Id)?.PermissionValue ?? 0
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
        public class PermissionDetail
        {
            public int ModulesId { get; set; }
            public int PermissionValue { get; set; }
        }

        [HttpPost("assign-account-permission")]
        public async Task<IActionResult> AssignAccountPermission([FromBody] AssignPermissionModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _context.AccountAdmins.FindAsync(model.AccountAdminId);
            if (account == null)
            {
                return NotFound(new { Message = "Tài khoản không tồn tại." });
            }

            var module = await _context.Modules.FindAsync(model.ModulesId);
            if (module == null)
            {
                return NotFound(new { Message = "Module không tồn tại." });
            }

            var existingPermission = await _context.AccountPermissions
                .FirstOrDefaultAsync(ap => ap.AccountAdminId == model.AccountAdminId && ap.ModulesId == model.ModulesId);

            if (existingPermission != null)
            {
                existingPermission.PermissionValue = model.PermissionValue;
            }
            else
            {
                var permission = new AccountPermissions
                {
                    AccountAdminId = model.AccountAdminId,
                    ModulesId = model.ModulesId,
                    PermissionValue = model.PermissionValue
                };
                _context.AccountPermissions.Add(permission);
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Gán quyền thành công." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                return StatusCode(500, new { Message = "Lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }

        public class AssignPermissionModel
        {
            public int AccountAdminId { get; set; }
            public int ModulesId { get; set; }
            public int PermissionValue { get; set; }
        }

        // PUT: api/AccountAdmin/5
        //[HttpPost]
        //[Route("PutAccountAdmin")]
        //public async Task<IActionResult> PutAccountAdmin([FromBody] AccountAdmin accountAdmin)
        //{
        //if (!ModelState.IsValid)
        // {
        //return BadRequest(ModelState);
        //}
        //_context.Entry(accountAdmin).State = EntityState.Modified;

        //try
        //{
        //accountAdmin.ModifiedDate = DateTime.Now;
        //accountAdmin.Password = GenerateMD5(accountAdmin.Password);
        //await _context.SaveChangesAsync();
        // }
        //catch (DbUpdateConcurrencyException)
        //{

        //}

        //return NoContent();
        //}

        [HttpPost]
        [Route("PutAccountAdmin")]
        public async Task<IActionResult> PutAccountAdmin([FromBody] PutAccountAdminModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Message = "Dữ liệu không hợp lệ", Errors = errors });
            }

            var existingAccount = await _context.AccountAdmins
                .Include(a => a.AccountPermissions)
                .FirstOrDefaultAsync(a => a.Id == model.AccountAdmin.Id);
            if (existingAccount == null)
            {
                return NotFound(new { Message = "Tài khoản không tồn tại." });
            }

            // Kiểm tra trùng lặp
            if (_context.AccountAdmins.Any(a => a.Code == model.AccountAdmin.Code && a.Id != model.AccountAdmin.Id))
            {
                return BadRequest(new { Message = "Mã tài khoản đã tồn tại." });
            }
            if (_context.AccountAdmins.Any(a => a.Email == model.AccountAdmin.Email && a.Id != model.AccountAdmin.Id))
            {
                return BadRequest(new { Message = "Email đã tồn tại." });
            }

            // Cập nhật mật khẩu nếu có
            if (!string.IsNullOrEmpty(model.AccountAdmin.Password) && model.AccountAdmin.Password != "null")
            {
                try
                {
                    PasswordHelper.CreatePasswordHash(model.AccountAdmin.Password, out var passwordHash, out var passwordSalt);
                    existingAccount.PasswordHash = passwordHash;
                    existingAccount.PasswordSalt = passwordSalt;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Password Hashing Error: " + ex.Message);
                    return StatusCode(500, new { Message = "Lỗi khi mã hóa mật khẩu." });
                }
            }

            // Cập nhật thông tin tài khoản
            existingAccount.Code = model.AccountAdmin.Code;
            existingAccount.Name = model.AccountAdmin.Name;
            existingAccount.Email = model.AccountAdmin.Email;
            existingAccount.Status = model.AccountAdmin.Status;
            existingAccount.ModifiedDate = DateTime.Now;

            // Cập nhật vai trò
            var existingRoles = _context.UserRoleAdmins.Where(ur => ur.AccountAdminId == model.AccountAdmin.Id).ToList();
            _context.UserRoleAdmins.RemoveRange(existingRoles);
            if (model.RoleIds != null && model.RoleIds.Any())
            {
                foreach (var roleId in model.RoleIds)
                {
                    _context.UserRoleAdmins.Add(new UserRoleAdmin
                    {
                        AccountAdminId = model.AccountAdmin.Id,
                        RolesId = roleId
                    });
                }
            }

            // Cập nhật quyền tài khoản chỉ khi có thay đổi
            if (model.AccountPermissions != null && model.AccountPermissions.Any())
            {
                // Xóa các quyền hiện có chỉ nếu có quyền mới để thay thế
                var existingPermissions = _context.AccountPermissions.Where(ap => ap.AccountAdminId == model.AccountAdmin.Id).ToList();
                _context.AccountPermissions.RemoveRange(existingPermissions);

                foreach (var perm in model.AccountPermissions)
                {
                    _context.AccountPermissions.Add(new AccountPermissions
                    {
                        AccountAdminId = model.AccountAdmin.Id,
                        ModulesId = perm.ModulesId,
                        PermissionValue = perm.PermissionValue
                    });
                }
            }
            // Nếu model.AccountPermissions là null hoặc rỗng, giữ nguyên quyền hiện tại

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.AccountAdmins.Any(e => e.Id == model.AccountAdmin.Id))
                {
                    return NotFound(new { Message = "Tài khoản không tồn tại." });
                }
                Console.WriteLine("Concurrency Error");
                return StatusCode(500, new { Message = "Lỗi đồng bộ dữ liệu." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                return StatusCode(500, new { Message = "Lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }

        public class PutAccountAdminModel
        {
            public AccountAdmin AccountAdmin { get; set; }
            public List<int> RoleIds { get; set; } = new List<int>();
            public List<AccountPermissionModel> AccountPermissions { get; set; } = new List<AccountPermissionModel>();
        }

        // POST: api/AccountAdmin
        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> PostAccountAdmin([FromBody] AccountAdmin accountAdmin)
        //{
        //if (!ModelState.IsValid)
        //{
        //   return BadRequest(ModelState);
        //}
        //accountAdmin.CreatedDate = DateTime.Now;
        //accountAdmin.Password = GenerateMD5(accountAdmin.Password);
        //_context.AccountAdmins.Add(accountAdmin);
        //await _context.SaveChangesAsync();

        //return CreatedAtAction("GetAccountAdmin", new { id = accountAdmin.Id }, accountAdmin);
        // }


        [HttpPost]
        [Route("PostAccountAdmin")]
        [AllowAnonymous]
        public async Task<IActionResult> PostAccountAdmin([FromBody] PostAccountAdminModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.AccountAdmins.Any(a => a.Code == model.AccountAdmin.Code))
            {
                return BadRequest(new { Message = "Mã tài khoản đã tồn tại." });
            }
            if (_context.AccountAdmins.Any(a => a.Email == model.AccountAdmin.Email))
            {
                return BadRequest(new { Message = "Email đã tồn tại." });
            }

            var accountAdmin = model.AccountAdmin;
            accountAdmin.CreatedDate = DateTime.Now;
            accountAdmin.ModifiedDate = DateTime.Now;

            // Tạo hash và salt cho mật khẩu
            PasswordHelper.CreatePasswordHash(accountAdmin.Password, out var passwordHash, out var passwordSalt);
            accountAdmin.PasswordHash = passwordHash;
            accountAdmin.PasswordSalt = passwordSalt;
            accountAdmin.Password = "null";

            _context.AccountAdmins.Add(accountAdmin);
            await _context.SaveChangesAsync();

            // Gán vai trò (nếu có)
            if (model.RoleIds != null && model.RoleIds.Any())
            {
                foreach (var roleId in model.RoleIds)
                {
                    var userRole = new UserRoleAdmin
                    {
                        AccountAdminId = accountAdmin.Id,
                        RolesId = roleId
                    };
                    _context.UserRoleAdmins.Add(userRole);
                }
            }

            // Gán quyền tài khoản (nếu có)
            if (model.AccountPermissions != null && model.AccountPermissions.Any())
            {
                foreach (var perm in model.AccountPermissions)
                {
                    var permission = new AccountPermissions
                    {
                        AccountAdminId = accountAdmin.Id,
                        ModulesId = perm.ModulesId,
                        PermissionValue = perm.PermissionValue
                    };
                    _context.AccountPermissions.Add(permission);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetAccountAdmin", new { id = accountAdmin.Id }, accountAdmin);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                return StatusCode(500, new { Message = "Lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }

        public class PostAccountAdminModel
        {
            public AccountAdmin AccountAdmin { get; set; }
            public List<int> RoleIds { get; set; } = new List<int>();
            public List<AccountPermissionModel> AccountPermissions { get; set; } = new List<AccountPermissionModel>();
        }

        public class AccountPermissionModel
        {
            public int ModulesId { get; set; }
            public int PermissionValue { get; set; }
        }

        // DELETE: api/AccountAdmin/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccountAdmin([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accountAdmin = await _context.AccountAdmins
                .Include(a => a.AccountPermissions)
                .Include(a => a.UserRoleAdmins)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (accountAdmin == null)
            {
                return NotFound();
            }

            // Xóa quyền tài khoản và vai trò
            _context.AccountPermissions.RemoveRange(accountAdmin.AccountPermissions);
            _context.UserRoleAdmins.RemoveRange(accountAdmin.UserRoleAdmins);
            _context.AccountAdmins.Remove(accountAdmin);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(accountAdmin);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                return StatusCode(500, new { Message = "Lỗi khi xóa dữ liệu: " + ex.Message });
            }
        }

        private bool AccountAdminExists(int? id)
        {
            return _context.AccountAdmins.Any(e => e.Id == id);
        }
    }
}