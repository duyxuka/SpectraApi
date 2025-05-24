using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
    public class BannerController : ControllerBase
    {
        AppDBContext _context;

        public BannerController(AppDBContext context)
        {
           this._context = context;
        }

        // GET: api/Banner
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetBanners()
        {
            var banners = await _context.Banners.ToListAsync();

            return Ok(banners);
        }


        // GET: api/Banner/5
        [HttpGet("{id}")]
        [BinaryAuthorize("Banner", ActionType.Xem)]
        public async Task<IActionResult> GetBanner([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var banner = await _context.Banners.FindAsync(id);

            if (banner == null)
            {
                return NotFound();
            }

            return Ok(banner);
        }
        

        // PUT: api/Banner/5
        [HttpPost]
        [Route("PutBanner")]
        [BinaryAuthorize("Banner", ActionType.Sua)]
        public async Task<IActionResult> PutBanner([FromBody] Banner banner)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(banner).State = EntityState.Modified;

            try
            {
                
                banner.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok(banner);
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return NoContent();
            
        }

        // POST: api/Banner
        [HttpPost]
        [BinaryAuthorize("Banner", ActionType.Them)]
        public async Task<IActionResult> PostBanner([FromBody] Banner banner)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            banner.CreatedDate = DateTime.Now;
            
            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBanner", new { id = banner.Id }, banner);
        }

        // DELETE: api/Banner/5
        [HttpDelete("{id}")]
        [BinaryAuthorize("Banner", ActionType.Xoa)]
        public async Task<IActionResult> DeleteBanner([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound();
            }

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();

            return Ok(banner);
        }

        private bool BannerExists(int? id)
        {
            return _context.Banners.Any(e => e.Id == id);
        }
    }
}