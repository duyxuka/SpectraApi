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
    public class CitiesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public CitiesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<City> GetCities()
        {
            var cities = _context.Cities
                .AsNoTracking()
                .Select(x => new City
                {
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status,
                    Counties = x.Counties.ToList(),
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate
                })
                .ToList();

            return cities;
        }


        // GET: api/Cities/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCity([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city);
        }
        
        // PUT: api/Cities/5
        [HttpPost]
        [Route("PutCities")]
        public async Task<IActionResult> PutCity([FromBody] City city)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(city).State = EntityState.Modified;

            try
            {
                city.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Cities
        [HttpPost]
        public async Task<IActionResult> PostCity([FromBody] City city)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = city.Id }, city);
        }

        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return Ok(city);
        }

        private bool CityExists(int? id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}