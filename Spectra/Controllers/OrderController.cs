using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Spectra.Models.OrderDetail;
using Spectra.Models;
using Spectra.Services;
using Microsoft.AspNetCore.Cors;
using System.Text.RegularExpressions;

namespace Spectra.Controllers
{
    [EnableCors("AddCors")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly AppDBContext _context;
        private static Random random = new Random();
        public OrderController(AppDBContext context)
        {
            _context = context;
        }


        // GET: api/Order
        [HttpGet]
        public IEnumerable<Order> GetOrder()
        {
            return _context.Order
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        [HttpGet]
        [Route("OrderPage")]
        public IActionResult OrderResult(int? page, int pagesize = 5)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var countDetails = _context.Order.AsNoTracking().Where(x => x.Website == 2).Count();
                var currentPage = page ?? 1;
                using (var context = _context)
                {
                    var OrderQuery = context.Order
                    .AsNoTracking()
                    .Where(x => x.Website == 2)
                    .OrderByDescending(x => x.Id)
                    .Skip((currentPage - 1) * pagesize)
                    .Take(pagesize);

                    var result = new PageResult<Order>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = pagesize,
                        Items = OrderQuery.ToList()
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

        [HttpGet]
        [Route("OrderPageBrand")]
        public IActionResult OrderResultBrand(int? page, int pagesize = 5)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var countDetails = _context.Order.AsNoTracking().Where(x => x.Website == 1).Count();
                var currentPage = page ?? 1;
                using (var context = _context)
                {
                    var OrderQuery = context.Order
                    .AsNoTracking()
                    .Where(x => x.Website == 1)
                    .OrderByDescending(x => x.Id)
                    .Skip((currentPage - 1) * pagesize)
                    .Take(pagesize);

                    var result = new PageResult<Order>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = pagesize,
                        Items = OrderQuery.ToList()
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

        [HttpGet]
        [Route("OrderPageCT")]
        public IActionResult OrderResultCT(int? page, int pagesize = 5)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var countDetails = _context.Order.AsNoTracking().Where(x => x.Website == 3).Count();
                var currentPage = page ?? 1;
                using (var context = _context)
                {
                    var OrderQuery = context.Order
                    .AsNoTracking()
                    .Where(x => x.Website == 3)
                    .OrderByDescending(x => x.Id)
                    .Skip((currentPage - 1) * pagesize)
                    .Take(pagesize);

                    var result = new PageResult<Order>
                    {
                        Count = countDetails,
                        PageIndex = currentPage,
                        PageSize = pagesize,
                        Items = OrderQuery.ToList()
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

        // GET: api/Order/5
        [HttpGet]
        [Route("orderReturn")]
        [AllowAnonymous]
        public IEnumerable<Order> GetOrdersReturn()
        {
            return _context.Order.OrderByDescending(x => x.Id).Where(x => x.Status == 6);
        }
        [HttpGet]
        [Route("orderSuccess")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderAccountSS([FromQuery] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = await _context.Order
                .AsNoTracking()
                .Join(_context.AccountUsers, ac => ac.AccountUserId, ae => ae.Id, (ac, ae) => new { ac, ae })
                .Where(x => x.ac.AccountUserId == id && x.ac.Status == 3)
                .Select(x => new Order
                {
                    Id = x.ac.Id,
                    AccountUserId = x.ac.AccountUserId,
                    TotalAmount = x.ac.TotalAmount,
                    TotalQuantity = x.ac.TotalQuantity,
                    Status = x.ac.Status,
                    CreatedDate = x.ac.CreatedDate,
                    ModifiedDate = x.ac.ModifiedDate,
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet]
        [Route("OrderHistory/{name}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrdersSuccess(string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var order = await _context.Order.Where(x => x.Status == 3 && x.Name.Equals(name)).ToListAsync();
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
        [HttpGet]
        [Route("GetAllOrders/{name}")]
        [AllowAnonymous]
        public IEnumerable<Order> GetOrders(string name)
        {
            var order = _context.Order.Where(x => x.Name.Equals(name)).ToList();
            return order;
        }
        [HttpGet]
        [Route("OrderAcc")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderAccount([FromQuery] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = await _context.Order.Join(_context.AccountUsers, ac => ac.AccountUserId,
              ae => ae.Id, (ac, ae) => new { ac, ae }).Where(x => x.ac.AccountUserId == id).Select(x => new Order
              {
                  Id = x.ac.Id,
                  Code = x.ac.Code,
                  AccountUserId = x.ac.AccountUserId,
                  TotalAmount = x.ac.TotalAmount,
                  TotalQuantity = x.ac.TotalQuantity,
                  Status = x.ac.Status,
                  PaymentMethod = x.ac.PaymentMethod,
                  CreatedDate = x.ac.CreatedDate,
                  ModifiedDate = x.ac.ModifiedDate,
              }).OrderByDescending(x => x.Id).ToListAsync();
            return Ok(data);
        }
        // GET: api/Orders/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrder([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Order.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/Orders/
        [HttpPost]
        [Route("PutOrder")]
        [AllowAnonymous]
        public async Task<IActionResult> PutOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            order.ModifiedDate = DateTime.Now;
            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(order);
            }
            catch (DbUpdateConcurrencyException)
            {
            }

            return NoContent();
        }

        // POST: api/Orders
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            order.CreatedDate = DateTime.Now;
            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }
        [HttpPost]
        [Route("ProductQuantity")]
        public async Task<IActionResult> ProductQuantity([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var orderDetails = _context.OrderDetail
         .Join(_context.Products, od => od.ProductId,
               p => p.Id, (od, p) => new
               {
                   Id = od.Id,
                   ProductId = od.ProductId,
                   Quantity = od.Quantity,
                   Price = od.Price,
                   Status = od.Status,
                   OrderId = od.OrderId,
                   CreatedDate = od.CreatedDate,
                   ModifiedDate = od.ModifiedDate,
                   ProductCode = p.Code,
                   ProductName = p.Name
               }).Select(x => new DisplayOrderDetail()
               {
                   Id = x.Id,
                   ProductId = x.ProductId,
                   Quantity = x.Quantity,
                   Price = x.Price,
                   Status = x.Status,
                   OrderId = x.OrderId,
                   CreatedDate = x.CreatedDate,
                   ModifiedDate = x.ModifiedDate,
                   ProductCode = x.ProductCode,
                   ProductName = x.ProductName
               }).Where(x => x.OrderId == order.Id).ToList();
            for (int i = 0; i < orderDetails.Count(); i++)
            {
                var product = _context.Products.Where(x => x.Id == orderDetails[i].ProductId).FirstOrDefault();
                //product.Quantity = product.Quantity + orderDetails[i].Quantity;
                _context.Entry(product).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        private bool OrderExists(int? id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}