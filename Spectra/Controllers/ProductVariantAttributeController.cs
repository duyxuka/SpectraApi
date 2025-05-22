using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectra.Models;

namespace Spectra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantAttributeController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ProductVariantAttributeController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/ProductVariantAttribute
        [HttpGet]
        public IEnumerable<ProductVariantAttributes> GetProductVariantAttributes()
        {
            return _context.ProductVariantAttributes;
        }

        // GET: api/ProductVariantAttribute/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductVariantAttributes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productVariantAttributes = await _context.ProductVariantAttributes.FindAsync(id);

            if (productVariantAttributes == null)
            {
                return NotFound();
            }

            return Ok(productVariantAttributes);
        }

        // PUT: api/ProductVariantAttribute/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductVariantAttributes([FromRoute] int id, [FromBody] ProductVariantAttributes productVariantAttributes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productVariantAttributes.Id)
            {
                return BadRequest();
            }

            _context.Entry(productVariantAttributes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductVariantAttributesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProductVariantAttribute
        [HttpPost]
        public async Task<IActionResult> PostProductVariantAttributes([FromBody] ProductVariantAttributes productVariantAttributes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductVariantAttributes.Add(productVariantAttributes);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductVariantAttributes", new { id = productVariantAttributes.Id }, productVariantAttributes);
        }

        // DELETE: api/ProductVariantAttribute/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductVariantAttributes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productVariantAttributes = await _context.ProductVariantAttributes.FindAsync(id);
            if (productVariantAttributes == null)
            {
                return NotFound();
            }

            _context.ProductVariantAttributes.Remove(productVariantAttributes);
            await _context.SaveChangesAsync();

            return Ok(productVariantAttributes);
        }

        private bool ProductVariantAttributesExists(int id)
        {
            return _context.ProductVariantAttributes.Any(e => e.Id == id);
        }
    }
}