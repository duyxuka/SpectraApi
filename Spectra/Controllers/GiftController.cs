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

namespace Spectra.Controllers
{
    [EnableCors("AddCors")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GiftController : ControllerBase
    {
        private readonly AppDBContext _context;

        public GiftController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Gift
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Gift> GetGift()
        {
            return _context.Gift;
        }

        // GET: api/Gift/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGift([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var gift = await _context.Gift.FindAsync(id);

            if (gift == null)
            {
                return NotFound();
            }

            return Ok(gift);
        }

        // PUT: api/Gift/5
        [HttpPost]
        [Route("PutGift")]
        public async Task<IActionResult> PutGift([FromBody] Gift gift)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(gift).State = EntityState.Modified;

            try
            {
                gift.Status = true;
                gift.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Gift
        [HttpPost]
        public async Task<IActionResult> PostGift([FromBody] Gift gift)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Gift.Add(gift);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGift", new { id = gift.Id }, gift);
        }

        // DELETE: api/Gift/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGift([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var gift = await _context.Gift.FindAsync(id);
            if (gift == null)
            {
                return NotFound();
            }

            _context.Gift.Remove(gift);
            await _context.SaveChangesAsync();

            return Ok(gift);
        }

        private bool GiftExists(int? id)
        {
            return _context.Gift.Any(e => e.Id == id);
        }
    }
}