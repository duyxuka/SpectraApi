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
    public class WarrantySeoController : ControllerBase
    {
        private readonly AppDBContext _context;

        public WarrantySeoController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/WarrantySeo
        [HttpGet]
        public IEnumerable<WarrantySeo> GetWarrantySeos()
        {
            return _context.WarrantySeos;
        }
        [HttpGet]
        [Route("WarrantyUser")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWarrantyUser()
        {
            var data = await _context.WarrantySeos.Select(x => new WarrantySeo
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
        // GET: api/WarrantySeo/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWarrantySeo([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var warrantySeo = await _context.WarrantySeos.FindAsync(id);

            if (warrantySeo == null)
            {
                return NotFound();
            }

            return Ok(warrantySeo);
        }

        // PUT: api/WarrantySeo/5
        [HttpPost]
        [Route("PutWarranty")]
        public async Task<IActionResult> PutWarrantySeo([FromBody] WarrantySeo warrantySeo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(warrantySeo).State = EntityState.Modified;

            try
            {
                warrantySeo.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/WarrantySeo
        [HttpPost]
        public async Task<IActionResult> PostWarrantySeo([FromBody] WarrantySeo warrantySeo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.WarrantySeos.Add(warrantySeo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWarrantySeo", new { id = warrantySeo.Id }, warrantySeo);
        }

        // DELETE: api/WarrantySeo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarrantySeo([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var warrantySeo = await _context.WarrantySeos.FindAsync(id);
            if (warrantySeo == null)
            {
                return NotFound();
            }

            _context.WarrantySeos.Remove(warrantySeo);
            await _context.SaveChangesAsync();

            return Ok(warrantySeo);
        }

        private bool WarrantySeoExists(int? id)
        {
            return _context.WarrantySeos.Any(e => e.Id == id);
        }
    }
}