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
            if (user is null)
            {
                return BadRequest("Invalid client request");
            }
            user.AccountName = account.Name;
            user.Role = account.Role;
            var pass_md5 = GenerateMD5(user.Password);
            if (user.Email == account.Email && pass_md5 == account.Password && account.Status == true)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:50925/",
                    audience: "http://localhost:50925/",
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: signinCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new AuthenticatedResponse { Token = tokenString, User = user });
            }
            return Unauthorized();
        }

        private string GenerateMD5(string password)
        {
                StringBuilder hash = new StringBuilder();
                MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
                byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(password));

                for (int i = 0; i < bytes.Length; i++)
                {
                    hash.Append(bytes[i].ToString("x2"));
                }
                return hash.ToString();
        }


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
        [HttpPost]
        [Route("PutAccountAdmin")]
        public async Task<IActionResult> PutAccountAdmin([FromBody] AccountAdmin accountAdmin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(accountAdmin).State = EntityState.Modified;

            try
            {
                accountAdmin.ModifiedDate = DateTime.Now;
                accountAdmin.Password = GenerateMD5(accountAdmin.Password);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/AccountAdmin
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostAccountAdmin([FromBody] AccountAdmin accountAdmin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            accountAdmin.CreatedDate = DateTime.Now;
            accountAdmin.Password = GenerateMD5(accountAdmin.Password);
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