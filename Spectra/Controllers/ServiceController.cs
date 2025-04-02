using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class ServiceController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ServiceController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Service
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Service> GetServices()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);
            var data = _context.Services.Select(x => new ServiceDisplay
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Status = x.Status,
                TitleSeo = x.TitleSeo,
                MetaKeyWords = x.MetaKeyWords,
                MetaDescription = x.MetaDescription,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                LinkName = rgx.Replace(x.Name , "-").ToLower()

            }).Where(x => x.Status == true).ToList();

            return data;
        }

        // GET: api/Service/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetService([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);
            var service = await _context.Services.Select(x => new ServiceDisplay
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Status = x.Status,
                TitleSeo = x.TitleSeo,
                MetaKeyWords = x.MetaKeyWords,
                MetaDescription = x.MetaDescription,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                LinkName = rgx.Replace(x.Name, "-").ToLower()

            }).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (service == null)
            {
                return NotFound();
            }

            return Ok(service);
        }

        // PUT: api/Service/5
        [HttpPost]
        [Route("PutService")]
        public async Task<IActionResult> PutService([FromBody] Service service)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(service).State = EntityState.Modified;

            try
            {
                service.Status = true;
                service.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Service
        [HttpPost]
        public async Task<IActionResult> PostService([FromBody] Service service)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetService", new { id = service.Id }, service);
        }

        // DELETE: api/Service/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return Ok(service);
        }

        private bool ServiceExists(int? id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}