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
using Attribute = Spectra.Models.Attribute;

namespace Spectra.Controllers
{
    [EnableCors("AddCors")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttributesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public AttributesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Attributes
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Attribute> GetAttributes()
        {
            return _context.Attributes;
        }

        // GET: api/Attributes/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAttribute([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var attribute = await _context.Attributes.FindAsync(id);

            if (attribute == null)
            {
                return NotFound();
            }

            return Ok(attribute);
        }

        // PUT: api/Attributes/5
        [HttpPost]
        [Route("PutAttribute")]
        public async Task<IActionResult> PutValueAttribute([FromBody] Attribute attribute)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(attribute).State = EntityState.Modified;

            try
            {
                attribute.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Attributes
        [HttpPost]
        public async Task<IActionResult> PostAttribute([FromBody] Attribute attribute)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Attributes.Add(attribute);
            attribute.CreatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAttribute", new { id = attribute.Id }, attribute);
        }

        // DELETE: api/Attributes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttribute([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var attribute = await _context.Attributes.FindAsync(id);
            if (attribute == null)
            {
                return NotFound();
            }

            _context.Attributes.Remove(attribute);
            await _context.SaveChangesAsync();

            return Ok(attribute);
        }

        private bool AttributeExists(int? id)
        {
            return _context.Attributes.Any(e => e.Id == id);
        }
    }
}