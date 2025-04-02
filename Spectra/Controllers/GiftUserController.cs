using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectra.Models;

namespace Spectra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GiftUserController : ControllerBase
    {
        private readonly AppDBContext _context;

        public GiftUserController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/GiftUser
        [HttpGet]
        public IEnumerable<GiftUser> GetGiftUsers()
        {
            return _context.GiftUsers.Where(b => b.Status == true).OrderByDescending(x => x.Id);
        }

        // GET: api/GiftUser/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGiftUser([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var giftUser = await _context.GiftUsers.FindAsync(id);

            if (giftUser == null)
            {
                return NotFound();
            }

            return Ok(giftUser);
        }

        // PUT: api/GiftUser/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGiftUser([FromRoute] int? id, [FromBody] GiftUser giftUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != giftUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(giftUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GiftUserExists(id))
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

        // POST: api/GiftUser
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostGiftUser([FromBody] GiftUser giftUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            giftUser.CreatedDate = DateTime.Now;
            _context.GiftUsers.Add(giftUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGiftUser", new { id = giftUser.Id }, giftUser);
        }

        // DELETE: api/GiftUser/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGiftUser([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var giftUser = await _context.GiftUsers.FindAsync(id);
            if (giftUser == null)
            {
                return NotFound();
            }

            _context.GiftUsers.Remove(giftUser);
            await _context.SaveChangesAsync();

            return Ok(giftUser);
        }

        private bool GiftUserExists(int? id)
        {
            return _context.GiftUsers.Any(e => e.Id == id);
        }
    }
}