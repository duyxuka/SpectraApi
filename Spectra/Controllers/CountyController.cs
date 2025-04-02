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
    public class CountyController : ControllerBase
    {
        private readonly AppDBContext _context;

        public CountyController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/County
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<County> GetCounties()
        {
            return _context.Counties.Select(x => new County
              {
                  Id = x.Id,
                  Name = x.Name,
                  Status = x.Status,
                  CityId = x.CityId,
                  //Locations = x.Locations.ToList(),
                  CreatedDate = x.CreatedDate,
                  ModifiedDate = x.ModifiedDate,

              }).ToList();
        }
        [HttpGet]
        [Route("getcountryID/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCityId([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var counties = await _context.Counties
                .AsNoTracking()
                .Where(c => c.CityId == id)
                .Select(x => new County
                {
                    Id = x.Id,
                    Name = x.Name,
                    CityId = x.CityId,
                    Status = x.Status
                })
                .ToListAsync();

            if (counties == null || counties.Count == 0)
            {
                return NotFound();
            }

            return Ok(counties);
        }


        // GET: api/County/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCounty([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var county = await _context.Counties.FindAsync(id);

            if (county == null)
            {
                return NotFound();
            }

            return Ok(county);
        }

        // PUT: api/County/5
        [HttpPost]
        [Route("PutCounty")]
        public async Task<IActionResult> PutCounty([FromBody] County county)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(county).State = EntityState.Modified;

            try
            {
                county.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/County
        [HttpPost]
        public async Task<IActionResult> PostCounty([FromBody] County county)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Counties.Add(county);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCounty", new { id = county.Id }, county);
        }

        // DELETE: api/County/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCounty([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var county = await _context.Counties.FindAsync(id);
            if (county == null)
            {
                return NotFound();
            }

            _context.Counties.Remove(county);
            await _context.SaveChangesAsync();

            return Ok(county);
        }

        private bool CountyExists(int? id)
        {
            return _context.Counties.Any(e => e.Id == id);
        }
    }
}