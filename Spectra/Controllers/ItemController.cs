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
using Spectra.Services;

namespace Spectra.Controllers
{
    [EnableCors("AddCors")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IServiceItem _serviceItem;
        public ItemController(AppDBContext context, IServiceItem serviceItem)
        {
            _serviceItem = serviceItem;
            _context = context;
        }

        // GET: api/Item
        [HttpGet]
        [AllowAnonymous]
        public  IEnumerable<Item> GetItems()
        {
            var data = _context.Items.Join(_context.Products, ai => ai.ProductId,
               al => al.Id, (ai, al) => new { ai, al }).Join(_context.Attributes, gt => gt.ai.AttributeId,
              pr => pr.Id, (gt, pr) => new { gt, pr }).Join(_context.ValueAttributes, sz => sz.gt.ai.ValueAttributeId,
              ss => ss.Id, (sz, ss) => new { sz, ss }).Join(_context.Gift, gf => gf.sz.gt.ai.GiftId,
              it => it.Id, (gf, it) => new { gf, it }).Select(x => new ItemDisplay
              {
                  Id = x.gf.sz.gt.ai.Id,
                  ProductId = x.gf.sz.gt.ai.ProductId,
                  AttributeId = x.gf.sz.gt.ai.AttributeId,
                  ValueAttributeId = x.gf.sz.gt.ai.ValueAttributeId,
                  ProductName = x.gf.sz.gt.al.Name,
                  AttributeName = x.gf.sz.pr.Name,
                  GiftId = x.gf.sz.gt.ai.GiftId,
                  JobId = x.gf.sz.gt.ai.JobId,
                  GifeName = x.it.Name,
                  StatusColor = x.gf.sz.pr.Status,
                  ValueAttributeName = x.gf.ss.Name,
                  StatusSize = x.gf.ss.Status,
                  Price = x.gf.sz.gt.ai.Price,
                  Status = x.gf.sz.gt.ai.Status

            }).ToList();
            return data;
        }

        [HttpPost]
        [Route("ItemHangfire")]
        public async Task<IActionResult> ItemHangfire([FromBody] Item item, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                DateTime endDateTime = Convert.ToDateTime(end);
                DateTime startDateTime = Convert.ToDateTime(start);
                DateTime currentDateTime = DateTime.Now;

                TimeSpan timeUntilStart = startDateTime.Subtract(currentDateTime);
                TimeSpan durationBetweenStartAndEnd = endDateTime.Subtract(startDateTime);

                double secondsUntilStart = timeUntilStart.TotalSeconds;
                double secondsBetweenStartAndEnd = durationBetweenStartAndEnd.TotalSeconds;

                if (secondsUntilStart < 0 || secondsBetweenStartAndEnd < 0)
                {
                    return BadRequest("Invalid time range.");
                }
                // Schedule the first job
                var jobId = BackgroundJob.Schedule<IServiceItem>(
                    x => x.UpdateDatabase(item),
                    TimeSpan.FromSeconds(secondsUntilStart));
                var jobId1 = BackgroundJob.Schedule<IServiceItem>(
                    x => x.UpdateDatabaseAgain(item),
                    TimeSpan.FromSeconds(secondsUntilStart + secondsBetweenStartAndEnd));
                // Update the database with the jobId1
                await _serviceItem.UpdateDatabaseJobIdAsync(item, jobId1);

                return Ok(item);
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("ConcurrencyError", "A concurrency error occurred while updating the database.");
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ItemHangfireCancel")]
        public IActionResult ItemHangfireCancel([FromBody] Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string[] jobss = item.JobId.Split('-');

            foreach (string job in jobss)
            {
                BackgroundJob.Delete(job);
            }

            BackgroundJob.Enqueue<IServiceItem>(x => x.UpdateDatabaseAgain(item));

            try
            {
                return Ok(item);
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return NoContent();

        }

        [HttpGet]
        [Route("getcolor/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetItemColor([FromRoute] int? id, string vl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var data = await _context.Items.Join(_context.Products, ai => ai.ProductId,
               al => al.Id, (ai, al) => new { ai, al }).Join(_context.Attributes, gt => gt.ai.AttributeId,
              pr => pr.Id, (gt, pr) => new { gt, pr }).Join(_context.ValueAttributes, sz => sz.gt.ai.ValueAttributeId,
              ss => ss.Id, (sz, ss) => new { sz, ss }).Join(_context.Gift, gf => gf.sz.gt.ai.GiftId,
              it => it.Id, (gf, it) => new { gf, it }).Select(x => new ItemDisplay
              {
                  Id = x.gf.sz.gt.ai.Id,
                  ProductId = x.gf.sz.gt.ai.ProductId,
                  AttributeId = x.gf.sz.gt.ai.AttributeId,
                  ValueAttributeId = x.gf.sz.gt.ai.ValueAttributeId,
                  ProductName = x.gf.sz.gt.al.Name,
                  AttributeName = x.gf.sz.pr.Name,
                  GiftId = x.gf.sz.gt.ai.GiftId,
                  JobId = x.gf.sz.gt.ai.JobId,
                  GifeName = x.it.Name,
                  StatusColor = x.gf.sz.pr.Status,
                  ValueAttributeName = x.gf.ss.Name,
                  StatusSize = x.gf.ss.Status,
                  Price = x.gf.sz.gt.ai.Price,
                  Status = x.gf.sz.gt.ai.Status,
              }).Where(x => x.ProductId == id && x.Status == true && x.AttributeName.ToLower().Contains(("màu sắc").Trim().ToLower())).ToListAsync();

            return Ok(data);
        }

        [HttpGet]
        [Route("getItemPro/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> getItemPro([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = await _context.Items.Join(_context.Products, ai => ai.ProductId,
               al => al.Id, (ai, al) => new { ai, al }).Join(_context.Attributes, gt => gt.ai.AttributeId,
              pr => pr.Id, (gt, pr) => new { gt, pr }).Join(_context.ValueAttributes, sz => sz.gt.ai.ValueAttributeId,
              ss => ss.Id, (sz, ss) => new { sz, ss }).Join(_context.Gift, gf => gf.sz.gt.ai.GiftId,
              it => it.Id, (gf, it) => new { gf, it }).Select(x => new ItemDisplay
              {
                  Id = x.gf.sz.gt.ai.Id,
                  ProductId = x.gf.sz.gt.ai.ProductId,
                  AttributeId = x.gf.sz.gt.ai.AttributeId,
                  ValueAttributeId = x.gf.sz.gt.ai.ValueAttributeId,
                  ProductName = x.gf.sz.gt.al.Name,
                  AttributeName = x.gf.sz.pr.Name,
                  GiftId = x.gf.sz.gt.ai.GiftId,
                  JobId = x.gf.sz.gt.ai.JobId,
                  GifeName = x.it.Name,
                  StatusColor = x.gf.sz.pr.Status,
                  ValueAttributeName = x.gf.ss.Name,
                  StatusSize = x.gf.ss.Status,
                  Price = x.gf.sz.gt.ai.Price,
                  Status = x.gf.sz.gt.ai.Status,
                  PriceAgain = x.gf.sz.gt.ai.PriceAgain

              }).Where(x => x.ProductId == id && x.Status == true && x.AttributeName.ToLower().Contains(("màu sắc").Trim().ToLower()) || x.ProductId == id && x.Status == true && x.AttributeName.ToLower().Contains(("kích thước").Trim().ToLower())).ToListAsync();

            return Ok(data);
        }

        [HttpGet]
        [Route("getsize/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetItemSize([FromRoute] int? id, string vl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = await _context.Items.Join(_context.Products, ai => ai.ProductId,
               al => al.Id, (ai, al) => new { ai, al }).Join(_context.Attributes, gt => gt.ai.AttributeId,
              pr => pr.Id, (gt, pr) => new { gt, pr }).Join(_context.ValueAttributes, sz => sz.gt.ai.ValueAttributeId,
              ss => ss.Id, (sz, ss) => new { sz, ss }).Join(_context.Gift, gf => gf.sz.gt.ai.GiftId,
              it => it.Id, (gf, it) => new { gf, it }).Select(x => new ItemDisplay
              {
                  Id = x.gf.sz.gt.ai.Id,
                  ProductId = x.gf.sz.gt.ai.ProductId,
                  AttributeId = x.gf.sz.gt.ai.AttributeId,
                  ValueAttributeId = x.gf.sz.gt.ai.ValueAttributeId,
                  ProductName = x.gf.sz.gt.al.Name,
                  AttributeName = x.gf.sz.pr.Name,
                  GiftId = x.gf.sz.gt.ai.GiftId,
                  JobId = x.gf.sz.gt.ai.JobId,
                  GifeName = x.it.Name,
                  StatusColor = x.gf.sz.pr.Status,
                  ValueAttributeName = x.gf.ss.Name,
                  StatusSize = x.gf.ss.Status,
                  Price = x.gf.sz.gt.ai.Price,
                  Status = x.gf.sz.gt.ai.Status,

              }).Where(x => x.ProductId == id && x.Status == true && x.AttributeName.ToLower().Contains(("kích thước").Trim().ToLower())).ToListAsync();

            if (data != null)
            {
                return Ok(data);
            }
            else
            {
                return null;
            }
            
        }

        [HttpGet]
        [Route("getsize1/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetItemSize1([FromRoute] int? id, string vl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var data = await _context.Items.Join(_context.Products, ai => ai.ProductId,
               al => al.Id, (ai, al) => new { ai, al }).Join(_context.Attributes, gt => gt.ai.AttributeId,
              pr => pr.Id, (gt, pr) => new { gt, pr }).Join(_context.ValueAttributes, sz => sz.gt.ai.ValueAttributeId,
              ss => ss.Id, (sz, ss) => new { sz, ss }).Join(_context.Gift, gf => gf.sz.gt.ai.GiftId,
              it => it.Id, (gf, it) => new { gf, it }).Select(x => new ItemDisplay
              {
                  Id = x.gf.sz.gt.ai.Id,
                  ProductId = x.gf.sz.gt.ai.ProductId,
                  AttributeId = x.gf.sz.gt.ai.AttributeId,
                  ValueAttributeId = x.gf.sz.gt.ai.ValueAttributeId,
                  ProductName = x.gf.sz.gt.al.Name,
                  AttributeName = x.gf.sz.pr.Name,
                  GiftId = x.gf.sz.gt.ai.GiftId,
                  JobId = x.gf.sz.gt.ai.JobId,
                  GifeName = x.it.Name,
                  StatusColor = x.gf.sz.pr.Status,
                  ValueAttributeName = x.gf.ss.Name,
                  StatusSize = x.gf.ss.Status,
                  Price = x.gf.sz.gt.ai.Price,
                  Status = x.gf.sz.gt.ai.Status,

              }).Where(x => x.ProductId == id && x.Status == true && x.AttributeName.ToLower().Contains(("kích thước").Trim().ToLower())).ToListAsync();
            if (data != null)
            {
                return Ok(data);
            }
            else
            {
                return null;
            }

        }
        [HttpGet]
        [Route("getproduct/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetItempro([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = await _context.Items.Join(_context.Products, ai => ai.ProductId,
               al => al.Id, (ai, al) => new { ai, al }).Join(_context.Attributes, gt => gt.ai.AttributeId,
              pr => pr.Id, (gt, pr) => new { gt, pr }).Join(_context.ValueAttributes, sz => sz.gt.ai.ValueAttributeId,
              ss => ss.Id, (sz, ss) => new { sz, ss }).Join(_context.Gift, gf => gf.sz.gt.ai.GiftId,
              it => it.Id, (gf, it) => new { gf, it }).Select(x => new ItemDisplay
              {
                  Id = x.gf.sz.gt.ai.Id,
                  ProductId = x.gf.sz.gt.ai.ProductId,
                  AttributeId = x.gf.sz.gt.ai.AttributeId,
                  ValueAttributeId = x.gf.sz.gt.ai.ValueAttributeId,
                  ProductName = x.gf.sz.gt.al.Name,
                  AttributeName = x.gf.sz.pr.Name,
                  GiftId = x.gf.sz.gt.ai.GiftId,
                  JobId = x.gf.sz.gt.ai.JobId,
                  GifeName = x.it.Name,
                  StatusColor = x.gf.sz.pr.Status,
                  ValueAttributeName = x.gf.ss.Name,
                  StatusSize = x.gf.ss.Status,
                  Price = x.gf.sz.gt.ai.Price,
                  Status = x.gf.sz.gt.ai.Status,

              }).Where(x => x.ProductId == id).ToListAsync();

            return Ok(data);
        }
        // GET: api/Item/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetItem([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _context.Items.Join(_context.Products, ai => ai.ProductId,
               al => al.Id, (ai, al) => new { ai, al }).Join(_context.Attributes, gt => gt.ai.AttributeId,
              pr => pr.Id, (gt, pr) => new { gt, pr }).Join(_context.ValueAttributes, sz => sz.gt.ai.ValueAttributeId,
              ss => ss.Id, (sz, ss) => new { sz, ss }).Join(_context.Gift, gf => gf.sz.gt.ai.GiftId,
              it => it.Id, (gf, it) => new { gf, it }).Select(x => new ItemDisplay
              {
                  Id = x.gf.sz.gt.ai.Id,
                  ProductId = x.gf.sz.gt.ai.ProductId,
                  AttributeId = x.gf.sz.gt.ai.AttributeId,
                  ValueAttributeId = x.gf.sz.gt.ai.ValueAttributeId,
                  ProductName = x.gf.sz.gt.al.Name,
                  AttributeName = x.gf.sz.pr.Name,
                  GiftId = x.gf.sz.gt.ai.GiftId,
                  JobId = x.gf.sz.gt.ai.JobId,
                  GifeName = x.it.Name,
                  GifePrice = x.it.Price,
                  StatusColor = x.gf.sz.pr.Status,
                  ValueAttributeName = x.gf.ss.Name,
                  StatusSize = x.gf.ss.Status,
                  Price = x.gf.sz.gt.ai.Price,
                  Status = x.gf.sz.gt.ai.Status,

              }).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // PUT: api/Item/5
        [HttpPost]
        [Route("PutItem")]
        public async Task<IActionResult> PutItem([FromBody] Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                item.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Item
        [HttpPost]
        public async Task<IActionResult> PostItem([FromBody] Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Items.Add(item);
            item.CreatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItem", new { id = item.Id }, item);
        }

        // DELETE: api/Item/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        private bool ItemExists(int? id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}