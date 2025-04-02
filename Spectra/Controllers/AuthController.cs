using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spectra.Models;

namespace Spectra.Controllers
{
    [Route("api/auth")]
    [EnableCors("AddCors")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDBContext _context;
        public AuthController(AppDBContext context)
        {
            _context = context;
        }
        [HttpPost("signin")]
        public async Task<IActionResult> SignInAsync([FromBody] SignInRequest signInRequest)
        {
            //tkuser: duy17092002@gmail.com
            //mk: 123456789
            AccountUser user;
            user = _context.AccountUsers.Where(acc => acc.Email == signInRequest.Email & acc.Password == signInRequest.Password).FirstOrDefault();
            
            if (user is null)
            {
                return BadRequest(new Response(false, "Invalid credentials."));
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.MobilePhone,user.Phone),
                //new Claim(type: ClaimTypes.Name, value: user.Name)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                });
            //HttpContext.Session.Set("Email", user.Email);
            return Ok(new Response(true, "Signed in successfully"));
        }
        ////[Authorize]
        [HttpGet("user")]
        public IActionResult GetUser()
        {
            var userClaims = User.Claims.FirstOrDefault();
            return Ok(userClaims);
        }
        [Authorize]
        [HttpGet("signout")]
        public async Task SignOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

}