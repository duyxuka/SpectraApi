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
        [BinaryAuthorize("Admin", ActionType.Xem)]
        public async Task<IEnumerable<AdminDTO>> GetAccountAdmins()
        {
            var accounts = await (from ac in _context.AccountAdmins
                                  join ur in _context.UserRoleAdmins on ac.Id equals ur.AccountAdminId
                                  join ro in _context.Roles on ur.RolesId equals ro.Id
                                  select new
                                  {
                                      ac.Id,
                                      ac.Code,
                                      ac.Name,
                                      ac.Status,
                                      ac.Email,
                                      RoleName = ro.Name
                                  })
                                 .AsNoTracking()
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
                    RoleNames = g.Select(x => x.RoleName).Distinct().ToList()
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
                .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (!PasswordHelper.VerifyPassword(user.Password, account.PasswordHash, account.PasswordSalt) || !account.Status)
                return BadRequest("Sai mật khẩu hoặc tài khoản đã bị khóa");

            // Lấy quyền từ tất cả role → tổng hợp từng module bằng bitwise OR
            var permissions = await (from ur in _context.UserRoleAdmins
                                     join rp in _context.Permissions on ur.RolesId equals rp.RolesId
                                     join m in _context.Modules on rp.ModulesId equals m.Id
                                     where ur.AccountAdminId == account.Id
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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Name),
                new Claim("UserId", account.Id.ToString()),
                new Claim("Permissions", JsonConvert.SerializeObject(permissionDict)) // serialize permissions
            };

            var token = GenerateJwtToken(claims); // bạn tự code phần này

            return Ok(new { token });
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
        [BinaryAuthorize("Admin", ActionType.Xem)]
        public async Task<IActionResult> GetAccountAdmin([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accountAdmin = await _context.AccountAdmins
                .Where(ac => ac.Id == id)
                .FirstOrDefaultAsync();

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

            var result = new
            {
                accountAdmin.Id,
                accountAdmin.Code,
                accountAdmin.Name,
                accountAdmin.Status,
                accountAdmin.Email,
                Roles = roles
            };

            return Ok(result);
        }


        [HttpGet]
        [Route("GetPermissionsByRoles")]
        public async Task<IActionResult> GetPermissionsByRoles([FromQuery] int[] roleIds)
        {
            if (roleIds == null || roleIds.Length == 0)
            {
                return BadRequest("RoleIds are required.");
            }

            try
            {
                // Lấy tất cả các module từ bảng Spectra_Modules
                var modules = await _context.Modules
                    .Select(m => new
                    {
                        m.Id,
                        m.Name
                    })
                    .ToListAsync();

                // Lấy danh sách quyền từ bảng Spectra_Permissions cho các roleIds
                var permissionsRaw = await _context.Permissions
                    .Where(p => roleIds.Contains(p.RolesId))
                    .Select(p => new
                    {
                        p.ModulesId,
                        p.PermissionValue
                    })
                    .ToListAsync();

                // Gộp quyền trong bộ nhớ (LINQ-to-Objects)
                var permissions = permissionsRaw
                    .GroupBy(p => p.ModulesId)
                    .Select(g => new
                    {
                        ModulesId = g.Key,
                        PermissionValue = g.Aggregate(0, (acc, p) => acc | p.PermissionValue) // Gộp quyền ở phía client
            })
                    .ToList();

                // Ánh xạ tất cả module với quyền (nếu không có quyền thì PermissionValue = 0)
                var result = modules.Select(m => new
                {
                    ModulesId = m.Id,
                    ModuleName = m.Name,
                    PermissionValue = permissions.FirstOrDefault(p => p.ModulesId == m.Id)?.PermissionValue ?? 0
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
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
        [BinaryAuthorize("Admin", ActionType.Sua)]
        public async Task<IActionResult> PutAccountAdmin([FromBody] AccountAdmin accountAdmin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAccount = await _context.AccountAdmins.FindAsync(accountAdmin.Id);
            if (existingAccount == null)
            {
                return NotFound();
            }

            // Chỉ cập nhật nếu có mật khẩu mới
            if (!string.IsNullOrEmpty(accountAdmin.Password))
            {
                PasswordHelper.CreatePasswordHash(accountAdmin.Password, out var passwordHash, out var passwordSalt);
                existingAccount.PasswordHash = passwordHash;
                existingAccount.PasswordSalt = passwordSalt;
            }

            existingAccount.Name = accountAdmin.Name;
            existingAccount.Email = accountAdmin.Email;
            existingAccount.Status = accountAdmin.Status;
            existingAccount.ModifiedDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.AccountAdmins.Any(e => e.Id == accountAdmin.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
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
        [BinaryAuthorize("Admin", ActionType.Them)]
        public async Task<IActionResult> PostAccountAdmin([FromBody] AccountAdmin accountAdmin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            accountAdmin.CreatedDate = DateTime.Now;
            accountAdmin.ModifiedDate = DateTime.Now;

            // Tạo hash và salt cho mật khẩu
            PasswordHelper.CreatePasswordHash(accountAdmin.Password, out var passwordHash, out var passwordSalt);
            accountAdmin.PasswordHash = passwordHash;
            accountAdmin.PasswordSalt = passwordSalt;
            accountAdmin.Password = "null"; // Không lưu trữ mật khẩu gốc

            _context.AccountAdmins.Add(accountAdmin);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccountAdmin", new { id = accountAdmin.Id }, accountAdmin);
        }

        // DELETE: api/AccountAdmin/5
        [HttpDelete("{id}")]
        [BinaryAuthorize("Admin", ActionType.Xoa)]
        public async Task<IActionResult> DeleteAccountAdmin([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accountAdmin = await _context.AccountAdmins.FindAsync(id);
            if (accountAdmin == null)
            {
                return NotFound();
            }

            _context.AccountAdmins.Remove(accountAdmin);
            await _context.SaveChangesAsync();

            return Ok(accountAdmin);
        }

        private bool AccountAdminExists(int? id)
        {
            return _context.AccountAdmins.Any(e => e.Id == id);
        }
    }
}