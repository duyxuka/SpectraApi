using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class PaymentInformationModelsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public PaymentInformationModelsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/PaymentInformationModels
        [HttpGet]
        public IEnumerable<PaymentInformationModel> GetPaymentInformationModels()
        {
            return _context.PaymentInformationModels.OrderByDescending(x => x.Id).ToList();
        }

        // GET: api/PaymentInformationModels/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentInformationModel([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paymentInformationModel = await _context.PaymentInformationModels.FindAsync(id);

            if (paymentInformationModel == null)
            {
                return NotFound();
            }

            return Ok(paymentInformationModel);
        }

        // PUT: api/PaymentInformationModels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaymentInformationModel([FromRoute] int? id, [FromBody] PaymentInformationModel paymentInformationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != paymentInformationModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(paymentInformationModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentInformationModelExists(id))
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

        // POST: api/PaymentInformationModels
        [HttpPost]
        public async Task<IActionResult> PostPaymentInformationModel([FromBody] PaymentInformationModel paymentInformationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PaymentInformationModels.Add(paymentInformationModel);
            paymentInformationModel.CreatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaymentInformationModel", new { id = paymentInformationModel.Id }, paymentInformationModel);
        }


        // DELETE: api/PaymentInformationModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentInformationModel([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paymentInformationModel = await _context.PaymentInformationModels.FindAsync(id);
            if (paymentInformationModel == null)
            {
                return NotFound();
            }

            _context.PaymentInformationModels.Remove(paymentInformationModel);
            await _context.SaveChangesAsync();

            return Ok(paymentInformationModel);
        }

        private bool PaymentInformationModelExists(int? id)
        {
            return _context.PaymentInformationModels.Any(e => e.Id == id);
        }
    }
}