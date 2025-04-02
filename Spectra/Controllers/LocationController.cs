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
    public class LocationController : ControllerBase
    {
        private readonly AppDBContext _context;

        public LocationController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Location
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<LocationDisplay> GetLocations()
        {
            var locations = _context.Locations
                .AsNoTracking()
                .Join(_context.Cities,
                      ci => ci.CityId,
                      co => co.Id,
                      (ci, co) => new LocationDisplay
                      {
                          Id = ci.Id,
                          Name = ci.Name,
                          Description = ci.Description,
                          Code = ci.Code,
                          Status = ci.Status,
                          CityName = co.Name,
                          CityId = ci.CityId,
                          CreatedDate = ci.CreatedDate,
                          ModifiedDate = ci.ModifiedDate
                      })
                .ToList();

            return locations;
        }


        // GET: api/Location/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLocation([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var location = await _context.Locations.FindAsync(id);

            if (location == null)
            {
                return NotFound();
            }

            return Ok(location);
        }

        [HttpGet]
        [Route("GetbyCity/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLocationCity([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var locations = await _context.Locations
                .AsNoTracking()
                .Join(_context.Cities,
                      ci => ci.CityId,
                      co => co.Id,
                      (ci, co) => new LocationDisplay
                      {
                          Id = ci.Id,
                          Name = ci.Name,
                          Code = ci.Code,
                          Status = ci.Status,
                          CityName = co.Name,
                          Description = ci.Description,
                          CityId = ci.CityId,
                          CreatedDate = ci.CreatedDate,
                          ModifiedDate = ci.ModifiedDate
                      })
                .Where(x => x.CityId == id && x.Status)
                .ToListAsync();

            return Ok(locations);
        }


        [HttpGet]
        [Route("GetbyCityAdmin/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLocationCityAdmin([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var locations = await _context.Locations
                .AsNoTracking()
                .Join(_context.Cities,
                      ci => ci.CityId,
                      co => co.Id,
                      (ci, co) => new LocationDisplay
                      {
                          Id = ci.Id,
                          Name = ci.Name,
                          Code = ci.Code,
                          Status = ci.Status,
                          CityName = co.Name,
                          Description = ci.Description,
                          CityId = ci.CityId,
                          CreatedDate = ci.CreatedDate,
                          ModifiedDate = ci.ModifiedDate
                      })
                .Where(x => x.CityId == id)
                .ToListAsync();

            return Ok(locations);
        }

        [HttpGet]
        [Route("GetbyLocation/{name}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLocationName([FromRoute] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var location = await _context.Locations
                .AsNoTracking()
                .Join(_context.Cities,
                      ci => ci.CityId,
                      co => co.Id,
                      (ci, co) => new LocationDisplay
                      {
                          Id = ci.Id,
                          Name = ci.Name,
                          Code = ci.Code,
                          Status = ci.Status,
                          CityName = co.Name,
                          CityId = ci.CityId,
                          CreatedDate = ci.CreatedDate,
                          ModifiedDate = ci.ModifiedDate
                      })
                .Where(x => x.Code == name)
                .ToListAsync();

            return Ok(location);
        }


        // PUT: api/Location/5
        [HttpPost]
        [Route("PutLocation")]
        public async Task<IActionResult> PutLocation([FromBody] Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(location).State = EntityState.Modified;

            try
            {
                location.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Location
        [HttpPost]
        public async Task<IActionResult> PostLocation([FromBody] Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            location.CreatedDate = DateTime.Now;
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocation", new { id = location.Id }, location);
        }

        // DELETE: api/Location/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return Ok(location);
        }

        private bool LocationExists(int? id)
        {
            return _context.Locations.Any(e => e.Id == id);
        }
    }
}