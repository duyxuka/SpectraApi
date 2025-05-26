using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectra.Models;
using Spectra.Models.Authorize;
using Spectra.Services;

namespace Spectra.Controllers
{
    [EnableCors("AddCors")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductVariantController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IServiceProductVariant _serviceProductVariant;

        public ProductVariantController(AppDBContext context, IServiceProductVariant serviceProductVariant)
        {
            _serviceProductVariant = serviceProductVariant;
            _context = context;
        }

        // GET: api/ProductVariant
        [HttpGet]
        [BinaryAuthorize("Product", ActionType.Xem)]
        public async Task<IActionResult> GetProductVariants()
        {
            try
            {
                var variantDtos = await _context.ProductVariants
                    .Include(pv => pv.Product)
                    .Include(pv => pv.ProductVariantAttributes)
                        .ThenInclude(pva => pva.ValueAttribute)
                            .ThenInclude(va => va.Attribute)
                    .Select(pv => new VariantDto
                    {
                        VariantId = pv.Id,
                        SKU = pv.JobId,
                        Price = pv.Price,
                        SalePrice = pv.SalePrice,
                        Status = pv.Status,
                        ProductName = pv.Product != null ? pv.Product.Name : "N/A",
                        Attributes = pv.ProductVariantAttributes.Select(pva => new AttributeDto
                        {
                            AttributeName = pva.ValueAttribute != null ? pva.ValueAttribute.Attribute.Name : "N/A",
                            ValueName = pva.ValueAttribute != null ? pva.ValueAttribute.Name : "N/A"
                        }).ToList(),
                        ValueName = pv.ProductVariantAttributes.FirstOrDefault().ValueAttribute != null ? pv.ProductVariantAttributes.FirstOrDefault().ValueAttribute.Name : "N/A"
                    })
                    .ToListAsync();

                return Ok(variantDtos);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in GetProductVariants: {ex.Message}, StackTrace: {ex.StackTrace}");
                return StatusCode(500, "Internal Server Error");
            }
        }


        // GET: api/ProductVariant/5
        [HttpGet("{id}")]
        [BinaryAuthorize("Product", ActionType.Xem)]
        public async Task<IActionResult> GetProductVariant(int id)
        {
            try
            {
                var variant = await _context.ProductVariants
                    .Include(v => v.Product)
                    .Include(v => v.ProductVariantAttributes)
                        .ThenInclude(a => a.ValueAttribute)
                            .ThenInclude(va => va.Attribute)
                    .Select(pv => new VariantDto
                    {
                        VariantId = pv.Id,
                        ProductId = pv.ProductId,
                        ProductName = pv.Product != null ? pv.Product.Name : "N/A",
                        Price = pv.Price,
                        SalePrice = pv.SalePrice,
                        Status = pv.Status,
                        SKU = pv.JobId,
                        ValueAttributeId = pv.ProductVariantAttributes.FirstOrDefault() != null
                            ? pv.ProductVariantAttributes.FirstOrDefault().ValueAttributeId
                            : 0,
                        AttributeId = pv.ProductVariantAttributes.FirstOrDefault() != null && pv.ProductVariantAttributes.FirstOrDefault().ValueAttribute != null
                            ? pv.ProductVariantAttributes.FirstOrDefault().ValueAttribute.AttributeId
                            : 0,
                        Attributes = pv.ProductVariantAttributes.Select(pva => new AttributeDto
                        {
                            AttributeName = pva.ValueAttribute != null && pva.ValueAttribute.Attribute != null
                                ? pva.ValueAttribute.Attribute.Name
                                : "N/A",
                            ValueName = pva.ValueAttribute != null ? pva.ValueAttribute.Name : "N/A"
                        }).ToList()
                    })
                    .FirstOrDefaultAsync(v => v.VariantId == id);

                if (variant == null)
                {
                    return NotFound();
                }

                return Ok(variant);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProductVariant: {ex.Message}, StackTrace: {ex.StackTrace}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("variantsschedule")]
        [BinaryAuthorize("Product", ActionType.Sua)]
        public async Task<IActionResult> ScheduleVariant([FromBody] ProductVariant productVariant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Sử dụng CreatedDate và ModifiedDate làm start và end
                DateTime endDateTime = Convert.ToDateTime(productVariant.ModifiedDate);
                DateTime startDateTime = Convert.ToDateTime(productVariant.CreatedDate);
                DateTime currentDateTime = DateTime.Now;

                TimeSpan timeUntilStart = startDateTime.Subtract(currentDateTime);
                TimeSpan durationBetweenStartAndEnd = endDateTime.Subtract(startDateTime);

                double secondsUntilStart = timeUntilStart.TotalSeconds;
                double secondsBetweenStartAndEnd = durationBetweenStartAndEnd.TotalSeconds;

                if (secondsUntilStart < 0 || secondsBetweenStartAndEnd < 0)
                {
                    return BadRequest("Phạm vi thời gian không hợp lệ.");
                }

                // Lên lịch hai công việc Hangfire
                var jobId = BackgroundJob.Schedule<IServiceProductVariant>(
                    x => x.UpdateDatabase(productVariant),
                    TimeSpan.FromSeconds(secondsUntilStart));
                var jobId1 = BackgroundJob.Schedule<IServiceProductVariant>(
                    x => x.UpdateDatabaseAgain(productVariant),
                    TimeSpan.FromSeconds(secondsUntilStart + secondsBetweenStartAndEnd));

                // Update the database with the jobId1
                await _serviceProductVariant.UpdateDatabaseJobIdAsync(productVariant, jobId1);

                return Ok(productVariant);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Lỗi đồng bộ hóa dữ liệu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đặt lịch biến thể: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("ProductVariantHangfireCancel")]
        [BinaryAuthorize("Product", ActionType.Sua)]
        public IActionResult ItemHangfireCancel([FromBody] ProductVariant productVariant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string[] jobss = productVariant.JobId.Split('-');

            foreach (string job in jobss)
            {
                BackgroundJob.Delete(job);
            }

            BackgroundJob.Enqueue<IServiceProductVariant>(x => x.UpdateDatabaseAgain(productVariant));

            try
            {
                return Ok(productVariant);
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return NoContent();

        }

        // PUT: api/ProductVariant/5
        [HttpPost("PutItem")]
        [BinaryAuthorize("Product", ActionType.Sua)]
        public async Task<IActionResult> UpdateProductVariant([FromBody] ProductVariant variant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingVariant = await _context.ProductVariants
                .Include(v => v.ProductVariantAttributes)
                .FirstOrDefaultAsync(v => v.Id == variant.Id);

            if (existingVariant == null)
            {
                return NotFound();
            }

            // Update scalar properties
            existingVariant.ProductId = variant.ProductId;
            existingVariant.Price = variant.Price;
            existingVariant.SalePrice = variant.SalePrice;
            existingVariant.Status = variant.Status;

            // Update ProductVariantAttributes
            existingVariant.ProductVariantAttributes.Clear();
            foreach (var attr in variant.ProductVariantAttributes)
            {
                existingVariant.ProductVariantAttributes.Add(new ProductVariantAttributes
                {
                    ProductVariantId = variant.Id,
                    ValueAttributeId = attr.ValueAttributeId
                });
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST: api/ProductVariant
        [HttpPost]
        [BinaryAuthorize("Product", ActionType.Them)]
        public async Task<IActionResult> PostProductVariant([FromBody] ProductVariant productVariant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductVariants.Add(productVariant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductVariant", new { id = productVariant.Id }, productVariant);
        }

        // DELETE: api/ProductVariant/5
        [HttpDelete("{id}")]
        [BinaryAuthorize("Product", ActionType.Xoa)]
        public async Task<IActionResult> DeleteProductVariant([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productVariant = await _context.ProductVariants.FindAsync(id);
            if (productVariant == null)
            {
                return NotFound();
            }

            _context.ProductVariants.Remove(productVariant);
            await _context.SaveChangesAsync();

            return Ok(productVariant);
        }

        private bool ProductVariantExists(int id)
        {
            return _context.ProductVariants.Any(e => e.Id == id);
        }
    }
}