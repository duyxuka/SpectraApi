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
    public class OrderCusController : ControllerBase
    {
        private readonly AppDBContext _context;

        public OrderCusController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/OrderCus
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<OrderCus> GetOrderCus()
        {
            return _context.OrderCus.OrderByDescending(x => x.Id).ToList();
        }

        [HttpGet]
        [Route("orderSuccess")]
        [AllowAnonymous]
        public IEnumerable<OrderCus> GetOrderCusSuccess()
        {
            return _context.OrderCus.OrderByDescending(x => x.Id).Where(x => x.Status == 3);
        }
        [HttpGet]
        [Route("OrderHistory/{name}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderCusSuccess(string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ordercus = await _context.OrderCus.Where(x => x.Status == 3 && x.Name.Equals(name)).ToListAsync();
            if (ordercus == null)
            {
                return NotFound();
            }
            return Ok(ordercus);
        }
        [HttpGet]
        [Route("GetAllOrders/{name}")]
        [AllowAnonymous]
        public IEnumerable<OrderCus> GetOrderCus(string name)
        {
            var ordercus = _context.OrderCus.Where(x => x.Name.Equals(name)).ToList();
            return ordercus;
        }

        // GET: api/OrderCus/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderCus([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderCus = await _context.OrderCus.FindAsync(id);

            if (orderCus == null)
            {
                return NotFound();
            }

            return Ok(orderCus);
        }

        // PUT: api/OrderCus/5
        [HttpPost]
        [Route("PutOrderCus")]
        public async Task<IActionResult> PutOrderCus([FromBody] OrderCus orderCus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            orderCus.ModifiedDate = DateTime.Now;
            _context.Entry(orderCus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(orderCus);
            }
            catch (DbUpdateConcurrencyException)
            {
            }

            return NoContent();
        }

        // POST: api/OrderCus
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostOrderCus([FromBody] OrderCus orderCus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var random = new Random();
            orderCus.CreatedDate = DateTime.Now;
            orderCus.AccountCusId = random.Next(100000, 999999);
            _context.OrderCus.Add(orderCus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderCus", new { id = orderCus.Id }, orderCus);
        }

        // DELETE: api/OrderCus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderCus([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderCus = await _context.OrderCus.FindAsync(id);
            if (orderCus == null)
            {
                return NotFound();
            }

            _context.OrderCus.Remove(orderCus);
            await _context.SaveChangesAsync();

            return Ok(orderCus);
        }

        private bool OrderCusExists(int? id)
        {
            return _context.OrderCus.Any(e => e.Id == id);
        }
    }
}