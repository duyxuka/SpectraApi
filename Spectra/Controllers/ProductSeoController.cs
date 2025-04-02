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
    public class ProductSeoController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ProductSeoController(AppDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("ProductUser")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsUser()
        {
            var data = await _context.ProductSeos.Select(x => new ProductSeoDisplay
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
        // GET: api/ProductSeo
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<ProductSeo> GetProductSeos()
        {
            return _context.ProductSeos;
        }

        // GET: api/ProductSeo/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductSeo([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productSeo = await _context.ProductSeos.FindAsync(id);

            if (productSeo == null)
            {
                return NotFound();
            }

            return Ok(productSeo);
        }

        // PUT: api/ProductSeo/5
        [HttpPost]
        [Route("PutProductSeo")]
        public async Task<IActionResult> PutProductSeo([FromBody] ProductSeo productSeo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(productSeo).State = EntityState.Modified;

            try
            {
                productSeo.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/ProductSeo
        [HttpPost]
        public async Task<IActionResult> PostProductSeo([FromBody] ProductSeo productSeo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductSeos.Add(productSeo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductSeo", new { id = productSeo.Id }, productSeo);
        }

        // DELETE: api/ProductSeo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductSeo([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productSeo = await _context.ProductSeos.FindAsync(id);
            if (productSeo == null)
            {
                return NotFound();
            }

            _context.ProductSeos.Remove(productSeo);
            productSeo.CreatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(productSeo);
        }

        private bool ProductSeoExists(int? id)
        {
            return _context.ProductSeos.Any(e => e.Id == id);
        }
    }
}