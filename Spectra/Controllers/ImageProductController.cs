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
    public class ImageProductController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ImageProductController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/ImageProduct
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<ImageProduct> GetImageProducts()
        {
            var data = _context.ImageProducts.Join(_context.Products, ai => ai.ProductId,
              al => al.Id, (ai, al) => new { ai, al }).Select(x => new ImageProductDisplay
              {
                  Id = x.ai.Id,
                  ImageName = x.ai.ImageName,
                  ProductId = x.ai.ProductId,
                  Status = x.ai.Status,
                  ProductName = x.al.Name,
                  ProductsId = x.al.Id,
                  CreatedDate = x.ai.CreatedDate,
                  ModifiedDate = x.ai.ModifiedDate  
              }).Where(x => x.Status == true).ToList();
            return data;
        }
        [HttpGet]
        [Route("Images")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHomesUser()
        {
            var data = await _context.ImageProducts.Join(_context.Products, ai => ai.ProductId,
              al => al.Id, (ai, al) => new { ai, al }).Select(x => new ImageProductDisplay
              {
                  Id = x.ai.Id,
                  ImageName = x.ai.ImageName,
                  ProductId = x.ai.ProductId,
                  Status = x.ai.Status,
                  ProductName = x.al.Name,
                  ProductsId = x.al.Id,
                  CreatedDate = x.ai.CreatedDate,
                  ModifiedDate = x.ai.ModifiedDate
              }).Where(x => x.Status == true & x.ProductId == x.ProductsId).ToListAsync();
            return Ok(data);
        }
        // GET: api/ImageProduct/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImageProduct([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = await _context.ImageProducts
        .Join(_context.Products, ai => ai.ProductId,
              al => al.Id, (ai, al) => new
              {
                  Id = ai.Id,
                  ImageName = ai.ImageName,
                  ProductId = ai.ProductId,
                  Status = ai.Status

              }).Where(x => x.ProductId == id).Select(x => new ImageProduct
              {
                  Id = x.Id,
                  ImageName = x.ImageName,
                  ProductId = x.ProductId,
                  Status = x.Status

              }).ToListAsync();

            return Ok(data);
        }

        // PUT: api/ImageProduct/5
        [HttpPost]
        [Route("PutImageProduct")]
        public async Task<IActionResult> PutImageProduct([FromBody] ImageProduct imageProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(imageProduct).State = EntityState.Modified;

            try
            {
                imageProduct.Status = true;
                imageProduct.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/ImageProduct
        [HttpPost]
        public async Task<IActionResult> PostImageProduct([FromBody] ImageProduct imageProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ImageProducts.Add(imageProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetImageProduct", new { id = imageProduct.Id }, imageProduct);
        }

        // DELETE: api/ImageProduct/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImageProduct([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageProduct = await _context.ImageProducts.FindAsync(id);
            if (imageProduct == null)
            {
                return NotFound();
            }

            _context.ImageProducts.Remove(imageProduct);
            await _context.SaveChangesAsync();

            return Ok(imageProduct);
        }

        private bool ImageProductExists(int? id)
        {
            return _context.ImageProducts.Any(e => e.Id == id);
        }
    }
}