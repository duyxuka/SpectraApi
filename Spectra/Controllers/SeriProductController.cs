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
    public class SeriProductController : ControllerBase
    {
        private readonly AppDBContext _context;

        public SeriProductController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/SeriProduct
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<SeriProduct> GetSeriProducts()
        {
            var seriproduct = _context.SeriProducts
                .AsNoTracking()
                .Join(_context.Locations, ai => ai.LocationId, al => al.Id, (ai, al) => new { ai, al })
                .Join(_context.Cities, ci => ci.ai.CityId, co => co.Id, (ci, co) => new { ci, co })
                .Join(_context.Products, pr => pr.ci.ai.ProductId, se => se.Id, (pr, se) => new { pr, se })
                .Join(_context.Category, ca => ca.se.CategoryId, ct => ct.Id, (ca, ct) => new { ca, ct })
                .Select(x => new SeriProductDisplay
                {
                    Id = x.ca.pr.ci.ai.Id,
                    ProductSeri = x.ca.pr.ci.ai.ProductSeri,
                    ProductId = x.ca.pr.ci.ai.ProductId,
                    Status = x.ca.pr.ci.ai.Status,
                    LocationId = x.ca.pr.ci.ai.LocationId,
                    CityName = x.ca.pr.co.Name,
                    CityId = x.ca.pr.ci.ai.CityId,
                    ProductName = x.ca.se.Name,
                    LocationName = x.ca.pr.ci.al.Name,
                    LocationCode = x.ca.pr.ci.al.Code,
                    CreatedDate = x.ca.pr.ci.ai.CreatedDate,
                    DealerSaleDate = x.ca.pr.ci.ai.DealerSaleDate,
                })
                .OrderByDescending(x => x.Id)
                .ToList();

            return seriproduct;
        }
        [HttpGet]
        [Route("SeriProductPage")]
        public IActionResult SeriProductResult(int? page, int pagesize = 5)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var countDetails = _context.SeriProducts.AsNoTracking().Count();
                var currentPage = page ?? 1;
                using (var context = _context)
                {
                    var SeriQuery = context.SeriProducts
                    .AsNoTracking()
                    .Join(_context.Locations, ai => ai.LocationId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(_context.Cities, ci => ci.ai.CityId, co => co.Id, (ci, co) => new { ci, co })
                    .Join(_context.Products, pr => pr.ci.ai.ProductId, se => se.Id, (pr, se) => new { pr, se })
                    .Join(_context.Category, ca => ca.se.CategoryId, ct => ct.Id, (ca, ct) => new { ca, ct })
                    .Select(x => new SeriProductDisplay
                    {
                        Id = x.ca.pr.ci.ai.Id,
                        ProductSeri = x.ca.pr.ci.ai.ProductSeri,
                        ProductId = x.ca.pr.ci.ai.ProductId,
                        Status = x.ca.pr.ci.ai.Status,
                        LocationId = x.ca.pr.ci.ai.LocationId,
                        CityName = x.ca.pr.co.Name,
                        CityId = x.ca.pr.ci.ai.CityId,
                        ProductName = x.ca.se.Name,
                        LocationName = x.ca.pr.ci.al.Name,
                        LocationCode = x.ca.pr.ci.al.Code,
                        CreatedDate = x.ca.pr.ci.ai.CreatedDate,
                        DealerSaleDate = x.ca.pr.ci.ai.DealerSaleDate,
                    })
                        .OrderByDescending(x => x.Id)
                        .AsNoTracking()
                        .Skip((currentPage - 1) * pagesize)
                        .Take(pagesize);

                    var result = new PageResult<SeriProductDisplay>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = pagesize,
                        Items = SeriQuery.ToList()
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
        // GET: api/SeriProduct/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSeriProduct([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var seriProduct = await _context.SeriProducts.FindAsync(id);

            if (seriProduct == null)
            {
                return NotFound();
            }

            return Ok(seriProduct);
        }


        [HttpGet]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSearch([FromQuery] string code)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productseri = await _context.SeriProducts
                .AsNoTracking()
                .Join(_context.Locations, ai => ai.LocationId, al => al.Id, (ai, al) => new { ai, al })
                .Join(_context.Cities, ci => ci.ai.CityId, co => co.Id, (ci, co) => new { ci, co })
                .Join(_context.Products, pr => pr.ci.ai.ProductId, se => se.Id, (pr, se) => new { pr, se })
                .Join(_context.Category, ca => ca.se.CategoryId, ct => ct.Id, (ca, ct) => new { ca, ct })
                .Where(s => s.ca.pr.ci.ai.ProductSeri == code)
                .Select(x => new SeriProductDisplay
                {
                    Id = x.ca.pr.ci.ai.Id,
                    ProductSeri = x.ca.pr.ci.ai.ProductSeri,
                    ProductId = x.ca.pr.ci.ai.ProductId,
                    Status = x.ca.pr.ci.ai.Status,
                    LocationId = x.ca.pr.ci.ai.LocationId,
                    CityName = x.ca.pr.co.Name,
                    ProductWarranty = x.ca.se.WarrantyMonth,
                    CityId = x.ca.pr.ci.ai.CityId,
                    CategoryId = x.ct.Id,
                    ProductName = x.ca.se.Name,
                    LocationName = x.ca.pr.ci.al.Name,
                    LocationCode = x.ca.pr.ci.al.Code,
                    CreatedDate = x.ca.pr.ci.ai.CreatedDate,
                    DealerSaleDate = x.ca.pr.ci.ai.DealerSaleDate,
                })
                .FirstOrDefaultAsync();

            if (productseri == null)
            {
                return NotFound();
            }

            return Ok(productseri);
        }

        // PUT: api/SeriProduct/5
        [HttpPost]
        [Route("PutSeriProduct")]
        [AllowAnonymous]
        public async Task<IActionResult> PutSeriProduct([FromBody] SeriProduct seriProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(seriProduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
            }

            return NoContent();
        }

        // POST: api/SeriProduct
        [HttpPost]
        public async Task<IActionResult> PostSeriProduct([FromBody] SeriProduct seriProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            seriProduct.CreatedDate = DateTime.Now;
            seriProduct.Status = false;
            _context.SeriProducts.Add(seriProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSeriProduct", new { id = seriProduct.Id }, seriProduct);
        }

        // DELETE: api/SeriProduct/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeriProduct([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var seriProduct = await _context.SeriProducts.FindAsync(id);
            if (seriProduct == null)
            {
                return NotFound();
            }

            _context.SeriProducts.Remove(seriProduct);
            await _context.SaveChangesAsync();

            return Ok(seriProduct);
        }

        private bool SeriProductExists(int? id)
        {
            return _context.SeriProducts.Any(e => e.Id == id);
        }
    }
}