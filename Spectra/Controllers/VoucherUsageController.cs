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
    public class VoucherUsageController : ControllerBase
    {
        private readonly AppDBContext _context;

        public VoucherUsageController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/VoucherUsage
        [HttpGet]
        public IEnumerable<VoucherUsage> GetVoucherUsages()
        {
            return _context.VoucherUsages;
        }

        // GET: api/VoucherUsage/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherUsage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var voucherUsage = await _context.VoucherUsages.FindAsync(id);

            if (voucherUsage == null)
            {
                return NotFound();
            }

            return Ok(voucherUsage);
        }

        // PUT: api/VoucherUsage/5
        [HttpPost]
        [Route("PutVoucherUsage")]
        public async Task<IActionResult> PutVoucherUsage([FromBody] VoucherUsage voucherUsage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(voucherUsage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }

            return NoContent();
        }

        // POST: api/VoucherUsage
        [HttpPost]
        public async Task<IActionResult> PostVoucherUsage([FromBody] VoucherUsage voucherUsage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var voucher = _context.Vouchers.FirstOrDefault(v => v.Id == voucherUsage.VoucherId);
            if (voucher == null || voucher.Quantity <= 0)
            {
                return BadRequest("Voucher không tồn tại hoặc đã hết số lượng");
            }

            // Giảm số lượng voucher
            voucher.Quantity--;

            _context.VoucherUsages.Add(new VoucherUsage
            {
                VoucherId = voucherUsage.VoucherId,
                CustomerId = voucherUsage.CustomerId,
                UsedDate = DateTime.Now
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVoucherUsage", new { id = voucherUsage.Id }, voucherUsage);
        }

        // DELETE: api/VoucherUsage/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoucherUsage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var voucherUsage = await _context.VoucherUsages.FindAsync(id);
            if (voucherUsage == null)
            {
                return NotFound();
            }

            _context.VoucherUsages.Remove(voucherUsage);
            await _context.SaveChangesAsync();

            return Ok(voucherUsage);
        }

        private bool VoucherUsageExists(int id)
        {
            return _context.VoucherUsages.Any(e => e.Id == id);
        }
    }
}