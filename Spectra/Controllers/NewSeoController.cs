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
    public class NewSeoController : ControllerBase
    {
        private readonly AppDBContext _context;

        public NewSeoController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/NewSeo
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<NewSeo> GetNewSeos()
        {
            return _context.NewSeos;
        }
        [HttpGet]
        [Route("NewUser")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewsUser()
        {
            var data = await _context.NewSeos.Select(x => new NewSeoDisplay
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
        // GET: api/NewSeo/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewSeo([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newSeo = await _context.NewSeos.FindAsync(id);

            if (newSeo == null)
            {
                return NotFound();
            }

            return Ok(newSeo);
        }

        // PUT: api/NewSeo/5
        [HttpPost]
        [Route("PutNewSeo")]
        public async Task<IActionResult> PutNewSeo([FromBody] NewSeo newSeo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(newSeo).State = EntityState.Modified;

            try
            {
                newSeo.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/NewSeo
        [HttpPost]
        public async Task<IActionResult> PostNewSeo([FromBody] NewSeo newSeo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.NewSeos.Add(newSeo);
            newSeo.CreatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNewSeo", new { id = newSeo.Id }, newSeo);
        }

        // DELETE: api/NewSeo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewSeo([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newSeo = await _context.NewSeos.FindAsync(id);
            if (newSeo == null)
            {
                return NotFound();
            }

            _context.NewSeos.Remove(newSeo);
            await _context.SaveChangesAsync();

            return Ok(newSeo);
        }

        private bool NewSeoExists(int? id)
        {
            return _context.NewSeos.Any(e => e.Id == id);
        }
    }
}