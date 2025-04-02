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
    public class WarrantyGoldLogController : ControllerBase
    {
        private readonly AppDBContext _context;

        public WarrantyGoldLogController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/WarrantyGoldLog
        [HttpGet]
        public IEnumerable<WarrantyGoldLog> GetWarrantyGoldLogs()
        {
            return _context.WarrantyGoldLogs;
        }

        [HttpGet]
        [Route("getwarrantygold/{id}")]
        public async Task<IActionResult> GetWarrantyGold([FromRoute] int? id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
                {
                    var data = await context.WarrantyGoldLogs
                        .Join(context.WarrantyGolds, ai => ai.WarrantyGoldId,
                              al => al.Id, (ai, al) => new
                              {
                                  Id = ai.Id,
                                  WarrantyGoldId = ai.WarrantyGoldId,
                                  ChangedBy = ai.ChangedBy,
                                  WarrantyContent = ai.WarrantyContent,
                                  OldValue = ai.OldValue,
                                  NewValue = ai.NewValue,
                                  CreatedDate = ai.CreatedDate
                              })
                            .Where(x => x.WarrantyGoldId == id)
                            .Select(x => new WarrantyGoldLog
                            {
                                Id = x.Id,
                                WarrantyGoldId = x.WarrantyGoldId,
                                ChangedBy = x.ChangedBy,
                                WarrantyContent = x.WarrantyContent,
                                OldValue = x.OldValue,
                                NewValue = x.NewValue,
                                CreatedDate = x.CreatedDate
                            })
                            .ToListAsync();

                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây, ví dụ log lại hoặc trả về một thông báo lỗi phù hợp
                Console.WriteLine($"Lỗi trong quá trình lấy danh sách chi tiết chào mừng: {ex.Message}");
                throw; // Ném ngoại lệ để lớp điều khiển xử lý tiếp tục xử lý
            }
        }

        // GET: api/WarrantyGoldLog/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWarrantyGoldLog([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var warrantyGoldLog = await _context.WarrantyGoldLogs.FindAsync(id);

            if (warrantyGoldLog == null)
            {
                return NotFound();
            }

            return Ok(warrantyGoldLog);
        }

        // PUT: api/WarrantyGoldLog/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarrantyGoldLog([FromRoute] int? id, [FromBody] WarrantyGoldLog warrantyGoldLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != warrantyGoldLog.Id)
            {
                return BadRequest();
            }

            _context.Entry(warrantyGoldLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarrantyGoldLogExists(id))
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

        // POST: api/WarrantyGoldLog
        [HttpPost]
        public async Task<IActionResult> PostWarrantyGoldLog([FromBody] WarrantyGoldLog warrantyGoldLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            warrantyGoldLog.CreatedDate = DateTime.Now;
            _context.WarrantyGoldLogs.Add(warrantyGoldLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWarrantyGoldLog", new { id = warrantyGoldLog.Id }, warrantyGoldLog);
        }

        // DELETE: api/WarrantyGoldLog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarrantyGoldLog([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var warrantyGoldLog = await _context.WarrantyGoldLogs.FindAsync(id);
            if (warrantyGoldLog == null)
            {
                return NotFound();
            }

            _context.WarrantyGoldLogs.Remove(warrantyGoldLog);
            await _context.SaveChangesAsync();

            return Ok(warrantyGoldLog);
        }

        private bool WarrantyGoldLogExists(int? id)
        {
            return _context.WarrantyGoldLogs.Any(e => e.Id == id);
        }
    }
}