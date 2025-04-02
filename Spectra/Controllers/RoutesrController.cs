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
    public class RoutesrController : ControllerBase
    {
        private readonly AppDBContext _context;

        public RoutesrController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Routesr
        [HttpGet]
        public IEnumerable<Routesr> GetRoutesrs()
        {
            return _context.Routesrs;
        }

        [HttpGet]
        [Route("RoutesrUser")]
        [AllowAnonymous]
        public IEnumerable<Routesr> GetRoutesrsUser()
        {
            return _context.Routesrs.Where(x => x.IsActive == true);
        }

        // GET: api/Routesr/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoutesr([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var routesr = await _context.Routesrs.FindAsync(id);

            if (routesr == null)
            {
                return NotFound();
            }

            return Ok(routesr);
        }

        // PUT: api/Routesr/5
        [HttpPost]
        [Route("PutRoutesr")]
        public async Task<IActionResult> PutRoutesr([FromBody] Routesr routesr)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(routesr).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
            }

            return NoContent();
        }

        // POST: api/Routesr
        [HttpPost]
        public async Task<IActionResult> PostRoutesr([FromBody] Routesr routesr)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            routesr.CreatedDate = DateTime.Now;
            _context.Routesrs.Add(routesr);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoutesr", new { id = routesr.Id }, routesr);
        }

        // DELETE: api/Routesr/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoutesr([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var routesr = await _context.Routesrs.FindAsync(id);
            if (routesr == null)
            {
                return NotFound();
            }

            _context.Routesrs.Remove(routesr);
            await _context.SaveChangesAsync();

            return Ok(routesr);
        }

        private bool RoutesrExists(int? id)
        {
            return _context.Routesrs.Any(e => e.Id == id);
        }
    }
}