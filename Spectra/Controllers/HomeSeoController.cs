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
    public class HomeSeoController : ControllerBase
    {
        private readonly AppDBContext _context;

        public HomeSeoController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Home
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Home> GetHomes()
        {
            return _context.Homes;
        }
        [HttpGet]
        [Route("HomeUser")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHomesUser()
        {
            var data = await _context.Homes.Select(x => new HomeDisplay
            {
                Id = x.Id,
                TitleSeo = x.TitleSeo,
                MetaKeyWords = x.MetaKeyWords,
                MetaDescription = x.MetaDescription,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
            }).FirstOrDefaultAsync();
            return Ok(data);
        }

        // GET: api/Home/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHome([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var home = await _context.Homes.FindAsync(id);

            if (home == null)
            {
                return NotFound();
            }

            return Ok(home);
        }

        // PUT: api/Home/5
        [HttpPost] 
        [Route("PutHome")]
        public async Task<IActionResult> PutHome([FromBody] Home home)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(home).State = EntityState.Modified;

            try
            {
                home.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Home
        [HttpPost]
        public async Task<IActionResult> PostHome([FromBody] Home home)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Homes.Add(home);
            home.CreatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHome", new { id = home.Id }, home);
        }

        // DELETE: api/Home/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHome([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var home = await _context.Homes.FindAsync(id);
            if (home == null)
            {
                return NotFound();
            }

            _context.Homes.Remove(home);
            await _context.SaveChangesAsync();

            return Ok(home);
        }

        private bool HomeExists(int? id)
        {
            return _context.Homes.Any(e => e.Id == id);
        }
    }
}