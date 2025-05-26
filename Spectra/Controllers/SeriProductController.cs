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
using Spectra.Models.Authorize;

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
        [BinaryAuthorize("SeriProduct", ActionType.Xem)]
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
        [Route("ExportedProductsLast6Months")]
        [BinaryAuthorize("Dashboard", ActionType.Xem)]
        public async Task<IActionResult> GetExportedProductsLast6Months()
        {
            try
            {
                var now = DateTime.Now;
                // Lấy ngày đầu tiên của tháng hiện tại trừ đi 5 tháng để có điểm bắt đầu 6 tháng trước
                var sixMonthsAgo = new DateTime(now.Year, now.Month, 1).AddMonths(-5);

                // Truy vấn các SeriProduct đã xuất kho (Status == true) trong 6 tháng qua
                // và nhóm theo tháng, năm của DealerSaleDate
                var monthlyExportedProducts = await _context.SeriProducts
                    .AsNoTracking()
                    .Where(sp => sp.Status == true && sp.DealerSaleDate.HasValue && sp.DealerSaleDate.Value >= sixMonthsAgo && sp.DealerSaleDate.Value <= now)
                    .GroupBy(sp => new { sp.DealerSaleDate.Value.Year, sp.DealerSaleDate.Value.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalExported = g.Count() // Đếm số lượng sản phẩm đã xuất kho trong nhóm này
                    })
                    .OrderBy(g => g.Year)
                    .ThenBy(g => g.Month)
                    .ToListAsync();

                // Tạo danh sách kết quả cho 6 tháng, bao gồm cả những tháng không có sản phẩm xuất kho
                var result = new List<object>();
                for (int i = 0; i < 6; i++)
                {
                    var currentMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
                    var monthData = monthlyExportedProducts.FirstOrDefault(m => m.Year == currentMonth.Year && m.Month == currentMonth.Month);

                    result.Add(new
                    {
                        Month = currentMonth.Month,
                        Year = currentMonth.Year,
                        MonthLabel = $"{currentMonth.Month}/{currentMonth.Year}",
                        TotalExported = monthData?.TotalExported ?? 0 // Nếu không có dữ liệu, coi như 0 sản phẩm xuất kho
                    });
                }

                // Sắp xếp lại danh sách theo thứ tự thời gian tăng dần
                result = result.OrderBy(x => ((dynamic)x).Year).ThenBy(x => ((dynamic)x).Month).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết để dễ dàng gỡ lỗi
                Console.WriteLine($"Error retrieving exported products data: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An error occurred while processing the request for exported products data.", error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }


        [HttpGet]
        [Route("SeriProductPage")]
        [BinaryAuthorize("SeriProduct", ActionType.Xem)]
        public IActionResult SeriProductResult(int? page, int pagesize = 5)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);
            var currentPage = page ?? 1;
            try
            {
                var query = _context.SeriProducts.AsNoTracking();
                var countDetails = query.Count();

                using (var context = _context)
                {
                    var SeriQuery = query
                    .OrderByDescending(x => x.Id)
                    .Skip((currentPage - 1) * pagesize)
                    .Take(pagesize)
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
                    });

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
        [BinaryAuthorize("SeriProduct", ActionType.Xem)]
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
        [BinaryAuthorize("SeriProduct", ActionType.Sua)]
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

        [HttpPost("UpdateStatusSeriProduct")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateStatusSeriProduct([FromBody] SeriProduct updateData)
        {
            var product = await _context.SeriProducts.FirstOrDefaultAsync(p => p.ProductSeri == updateData.ProductSeri);
            if (product == null)
                return NotFound();

            product.Status = true;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST: api/SeriProduct
        [HttpPost]
        [BinaryAuthorize("SeriProduct", ActionType.Them)]
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
        [BinaryAuthorize("SeriProduct", ActionType.Xoa)]
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