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
    public class VoucherController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IServiceVoucher _serviceVoucher;

        public VoucherController(AppDBContext context, IServiceVoucher serviceVoucher)
        {
            _serviceVoucher = serviceVoucher;
            _context = context;
        }

        // GET: api/Voucher
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Voucher> GetVouchers()
        {
            return _context.Vouchers.AsNoTracking()
                    .Join(_context.Products, ai => ai.ProductId, al => al.Id, (ai, al) => new { ai, al })
                    .Select(x => new VoucherDisplay
                    {
                        Id = x.ai.Id,
                        VoucherCode = x.ai.VoucherCode,
                        Discount = x.ai.Discount,
                        JobId = x.ai.JobId,
                        DiscountType = x.ai.DiscountType,
                        ScheduleStatus = x.ai.ScheduleStatus,
                        StartDate = x.ai.StartDate,
                        EndDate = x.ai.EndDate,
                        ProductId = x.ai.ProductId,
                        ProductName = x.al.Name,
                        Quantity = x.ai.Quantity,
                        Status = x.ai.Status

                    })
                    .ToList(); ;
        }

        [HttpGet]
        [Route("GetVoucherPro")]
        [AllowAnonymous]
        public IEnumerable<Voucher> GetVouchersByProduct([FromQuery] int id)
        {
            return _context.Vouchers.AsNoTracking()
                    .Join(_context.Products, ai => ai.ProductId, al => al.Id, (ai, al) => new { ai, al })
                    .Where(x => x.ai.ProductId == id && x.ai.Status == false)
                    .Select(x => new VoucherDisplay
                    {
                        Id = x.ai.Id,
                        VoucherCode = x.ai.VoucherCode,
                        Discount = x.ai.Discount,
                        JobId = x.ai.JobId,
                        DiscountType = x.ai.DiscountType,
                        ScheduleStatus = x.ai.ScheduleStatus,
                        StartDate = x.ai.StartDate,
                        EndDate = x.ai.EndDate,
                        ProductId = x.ai.ProductId,
                        Quantity = x.ai.Quantity,
                        ProductName = x.al.Name,
                        Status = x.ai.Status

                    })
                    .ToList(); ;
        }
        [HttpGet]
        [Route("GetVoucherProActive")]
        [AllowAnonymous]
        public IEnumerable<Voucher> GetVouchersByProductActive([FromQuery] int id)
        {
            return _context.Vouchers.AsNoTracking()
                    .Join(_context.Products, ai => ai.ProductId, al => al.Id, (ai, al) => new { ai, al })
                    .Where(x => x.ai.ProductId == id && x.ai.Status == true)
                    .Select(x => new VoucherDisplay
                    {
                        Id = x.ai.Id,
                        VoucherCode = x.ai.VoucherCode,
                        Discount = x.ai.Discount,
                        JobId = x.ai.JobId,
                        ScheduleStatus = x.ai.ScheduleStatus,
                        DiscountType = x.ai.DiscountType,
                        StartDate = x.ai.StartDate,
                        EndDate = x.ai.EndDate,
                        ProductId = x.ai.ProductId,
                        Quantity = x.ai.Quantity,
                        ProductName = x.al.Name,
                        Status = x.ai.Status

                    }).ToList();
        }

        [HttpGet]
        [Route("GetVoucherCheck")]
        [AllowAnonymous]
        public IActionResult GetVouchersByProductCheck([FromQuery] string code)
        {
            var voucher = _context.Vouchers.AsNoTracking()
                        .Where(x => x.VoucherCode == code && x.Status == true)
                        .Select(x => new VoucherDisplay
                        {
                            Id = x.Id,
                            VoucherCode = x.VoucherCode,
                            Discount = x.Discount,
                            JobId = x.JobId,
                            ScheduleStatus = x.ScheduleStatus,
                            DiscountType = x.DiscountType,
                            StartDate = x.StartDate,
                            EndDate = x.EndDate,
                            Quantity = x.Quantity,
                            ProductId = x.ProductId,
                            Status = x.Status

                        }).FirstOrDefault();

            if (voucher == null)
            {
                return NotFound();
            }

            return Ok(voucher);
        }

        [HttpPost]
        [Route("VoucherHangfire")]
        public async Task<IActionResult> VoucherHangfire([FromBody] Voucher voucher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                DateTime endDateTime = Convert.ToDateTime(voucher.EndDate);
                DateTime startDateTime = Convert.ToDateTime(voucher.StartDate);
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
                var jobId = BackgroundJob.Schedule<IServiceVoucher>(
                    x => x.UpdateDatabase(voucher),
                    TimeSpan.FromSeconds(secondsUntilStart));

                // Schedule the delay job as a continuation of the first job
                //var delayJobId = BackgroundJob.ContinueJobWith(
                //    jobId,
                //    () => Task.Delay(TimeSpan.FromSeconds(secondsBetweenStartAndEnd)),
                //    JobContinuationOptions.OnlyOnSucceededState);

                // Schedule the second job as a continuation of the delay job
                //var jobId1 = BackgroundJob.ContinueJobWith<IServiceVoucher>(
                //    delayJobId,
                //    x => x.UpdateDatabaseAgain(voucher),
                //    JobContinuationOptions.OnlyOnSucceededState);
                var jobId1 = BackgroundJob.Schedule<IServiceVoucher>(
                    x => x.UpdateDatabaseAgain(voucher),
                    TimeSpan.FromSeconds(secondsUntilStart + secondsBetweenStartAndEnd));
                // Update the database with the jobId1
                await _serviceVoucher.UpdateDatabaseJobIdAsync(voucher,jobId1);

                return Ok(voucher);
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
        [Route("VoucherHangfireCancel")]
        public IActionResult VoucherHangfireCancel([FromBody] Voucher voucher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string[] jobss = voucher.JobId.Split('-');

            foreach (string job in jobss)
            {
                BackgroundJob.Delete(job);
            }
            BackgroundJob.Enqueue<IServiceVoucher>(x => x.UpdateDatabaseAgain(voucher));

            try
            {
                return Ok(voucher);
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return NoContent();

        }

        // GET: api/Voucher/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucher([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var voucher = await _context.Vouchers.FindAsync(id);

            if (voucher == null)
            {
                return NotFound();
            }

            return Ok(voucher);
        }

        // PUT: api/Voucher/5
        [HttpPost]
        [Route("PutVoucher")]
        public async Task<IActionResult> PutVoucher([FromBody] Voucher voucher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(voucher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Voucher
        [HttpPost]
        public async Task<IActionResult> PostVoucher([FromBody] Voucher voucher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            voucher.JobId = "0";
            voucher.Status = false;
            voucher.ScheduleStatus = false;
            voucher.StartDate = new DateTime(0001, 01, 01, 00, 00, 00);
            voucher.EndDate = new DateTime(0001, 01, 01, 00, 00, 00);
            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVoucher", new { id = voucher.Id }, voucher);
        }

        // DELETE: api/Voucher/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoucher([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }

            _context.Vouchers.Remove(voucher);
            await _context.SaveChangesAsync();

            return Ok(voucher);
        }

        private bool VoucherExists(int id)
        {
            return _context.Vouchers.Any(e => e.Id == id);
        }
    }
}