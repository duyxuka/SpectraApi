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
using Spectra.Models;

namespace Spectra.Controllers
{
    [EnableCors("AddCors")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    
    public class AccountAdminController : ControllerBase
    {
        private readonly AppDBContext _context;
        
        public AccountAdminController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/AccountAdmin
        [HttpGet]
        public IEnumerable<AccountAdmin> GetAccountAdmins()
        {
            return _context.AccountAdmins.AsNoTracking().ToList();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginModel user)
        {
            var account = _context.AccountAdmins.FirstOrDefault(x => x.Email == user.Email);
            if (user is null || account is null)
            {
                return BadRequest("Invalid client request");
            }

            if (PasswordHelper.VerifyPassword(user.Password, account.PasswordHash, account.PasswordSalt) && account.Status == true)
            {
                // Lấy user role từ bảng quyền (nếu cần)
                var userRole = _context.UserRoles.FirstOrDefault(x => x.UserId == account.Id);
                var userType = userRole?.UserType.ToString() ?? "User"; // ép kiểu enum sang string

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim(ClaimTypes.Name, account.Name),
                    new Claim(ClaimTypes.Role, userType) // hợp lệ vì là string
                };


                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:50925/",
                    audience: "http://localhost:50925/",
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                return Ok(new AuthenticatedResponse
                {
                    Token = tokenString,
                    User = account.Name
                });
            }

            return Unauthorized();
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

            var accountAdmin = await _context.AccountAdmins.FindAsync(id);

            if (accountAdmin == null)
            {
                return NotFound();
            }

            return Ok(accountAdmin);
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