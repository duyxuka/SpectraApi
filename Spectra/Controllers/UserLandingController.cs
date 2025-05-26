using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class UserLandingController : ControllerBase
    {
        private readonly AppDBContext _context;

        public UserLandingController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/UserLanding
        [HttpGet]
        [BinaryAuthorize("UserLanding", ActionType.Xem)]
        public IEnumerable<UserLanding> GetUserLandings()
        {
            return _context.UserLandings.Where(b => b.Status == true).OrderByDescending(x=> x.Id);
        }

        // GET: api/UserLanding/5
        [HttpGet("{id}")]
        [BinaryAuthorize("UserLanding", ActionType.Xem)]
        public async Task<IActionResult> GetUserLanding([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userLanding = await _context.UserLandings.FindAsync(id);

            if (userLanding == null)
            {
                return NotFound();
            }

            return Ok(userLanding);
        }

        // PUT: api/UserLanding/5
        [HttpPut("{id}")]
        [BinaryAuthorize("UserLanding", ActionType.Sua)]
        public async Task<IActionResult> PutUserLanding([FromRoute] int? id, [FromBody] UserLanding userLanding)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userLanding.Id)
            {
                return BadRequest();
            }

            _context.Entry(userLanding).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserLandingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserLanding
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostUserLanding([FromBody] UserLanding userLanding)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var random = new Random();
            userLanding.Code = random.Next(100000,999999);
            userLanding.CreatedDate = DateTime.Now;
            userLanding.Status = true;
            _context.UserLandings.Add(userLanding);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserLanding", new { id = userLanding.Id }, userLanding);
        }

        // DELETE: api/UserLanding/5
        [HttpDelete("{id}")]
        [BinaryAuthorize("UserLanding", ActionType.Xoa)]
        public async Task<IActionResult> DeleteUserLanding([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userLanding = await _context.UserLandings.FindAsync(id);
            if (userLanding == null)
            {
                return NotFound();
            }

            _context.UserLandings.Remove(userLanding);
            await _context.SaveChangesAsync();

            return Ok(userLanding);
        }

        private bool UserLandingExists(int? id)
        {
            return _context.UserLandings.Any(e => e.Id == id);
        }
    }
}