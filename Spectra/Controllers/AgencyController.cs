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
    public class AgencyController : ControllerBase
    {
        private readonly AppDBContext _context;

        public AgencyController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Agency
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Agency> GetAgencies()
        {
            return _context.Agencies.AsNoTracking().ToList(); 
        }
        [HttpGet]
        [Route("AgencyUser")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAgencyUser()
        {
            var data = await _context.Agencies
                .AsNoTracking()
                .Select(x => new AgencyDisplay
                {
                    Id = x.Id,
                    TitleSeo = x.TitleSeo,
                    MetaKeyWords = x.MetaKeyWords,
                    MetaDescription = x.MetaDescription,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        // GET: api/Agency/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAgency([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agency = await _context.Agencies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (agency == null)
            {
                return NotFound();
            }

            return Ok(agency);
        }


        // PUT: api/Agency/5
        [HttpPost]
        [Route("PutAgencySeo")]
        public async Task<IActionResult> PutAgency([FromBody] Agency agency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(agency).State = EntityState.Modified;

            try
            {
                agency.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Agency
        [HttpPost]
        public async Task<IActionResult> PostAgency([FromBody] Agency agency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Agencies.Add(agency);
            agency.CreatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAgency", new { id = agency.Id }, agency);
        }

        // DELETE: api/Agency/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgency([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agency = await _context.Agencies.FindAsync(id);
            if (agency == null)
            {
                return NotFound();
            }

            _context.Agencies.Remove(agency);
            await _context.SaveChangesAsync();

            return Ok(agency);
        }

        private bool AgencyExists(int? id)
        {
            return _context.Agencies.Any(e => e.Id == id);
        }
    }
}