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
    public class ValueAttributeController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ValueAttributeController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/ValueAttribute
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<ValueAttribute> GetValueAttributes()
        {
            var data = _context.ValueAttributes.Join(_context.Attributes, ai => ai.AttributeId,
               al => al.Id, (ai, al) => new { ai, al }).Select(x => new ValueDisplay
               {
                  Id = x.ai.Id,
                  Name = x.ai.Name,
                  AttributeId = x.ai.AttributeId,
                  AttributeName = x.al.Name,
                  Status = x.ai.Status

            }).Where(x => x.Status == true).ToList();

            return data;
        }

        // GET: api/ValueAttribute/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetValueAttribute([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var valueAttribute = await _context.ValueAttributes.FindAsync(id);

            if (valueAttribute == null)
            {
                return NotFound();
            }

            return Ok(valueAttribute);
        }

        // PUT: api/ValueAttribute/5
        [HttpPost]
        [Route("PutValueAttribute")]
        public async Task<IActionResult> PutValueAttribute([FromBody] ValueAttribute valueAttribute)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(valueAttribute).State = EntityState.Modified;

            try
            {
                valueAttribute.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/ValueAttribute
        [HttpPost]
        public async Task<IActionResult> PostValueAttribute([FromBody] ValueAttribute valueAttribute)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ValueAttributes.Add(valueAttribute);
            valueAttribute.CreatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetValueAttribute", new { id = valueAttribute.Id }, valueAttribute);
        }

        // DELETE: api/ValueAttribute/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteValueAttribute([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var valueAttribute = await _context.ValueAttributes.FindAsync(id);
            if (valueAttribute == null)
            {
                return NotFound();
            }

            _context.ValueAttributes.Remove(valueAttribute);
            await _context.SaveChangesAsync();

            return Ok(valueAttribute);
        }

        private bool ValueAttributeExists(int? id)
        {
            return _context.ValueAttributes.Any(e => e.Id == id);
        }
    }
}