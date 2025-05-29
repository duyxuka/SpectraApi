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
using Spectra.Models.Authorize;
using Newtonsoft.Json;

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
        [BinaryAuthorize("OrderManager", ActionType.Xem)]
        public IEnumerable<Order> GetOrder(int website)
        {
            return _context.Order
                .AsNoTracking()
                .Where(x => x.Website == website)
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        // New Dashboard API
        [HttpGet]
        [Route("Dashboard")]
        [BinaryAuthorize("Dashboard", ActionType.Xem)]
        public async Task<IActionResult> GetOrderDashboard()
        {
            try
            {
                var now = DateTime.Now;
                var startOfDay = new DateTime(now.Year, now.Month, now.Day);
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var startOfYear = new DateTime(now.Year, 1, 1);

                // 1. Total Revenue (Today, Month, Year)
                var revenueToday = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.CreatedDate >= startOfDay && o.Status == 3) // Status 3 = Success
                    .SumAsync(o => o.TotalAmount);

                var revenueMonth = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.CreatedDate >= startOfMonth && o.Status == 3)
                    .SumAsync(o => o.TotalAmount);

                var revenueYear = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.CreatedDate >= startOfYear && o.Status == 3)
                    .SumAsync(o => o.TotalAmount);

                // 2. Total Orders Sold
                var totalOrdersSold = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.Status == 3)
                    .CountAsync();

                // 3. Revenue by Payment Method
                var paymentMethodRevenue = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.Status == 3)
                    .GroupBy(o => o.PaymentMethod)
                    .Select(g => new
                    {
                        PaymentMethod = g.Key,
                        TotalRevenue = g.Sum(o => o.TotalAmount)
                    })
                    .ToListAsync();

                // 4. Order Status Breakdown
                var newOrders = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.Status == 0) // Assuming 0 = New
                    .CountAsync();

                var processingOrders = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.Status == 1) // Assuming 1 = Processing
                    .CountAsync();

                var shippingOrders = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.Status == 2) // Assuming 2 = Shipping
                    .CountAsync();

                var completedOrders = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.Status == 3) // Assuming 3 = Completed
                    .CountAsync();

                var cancelledOrders = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.Status == 4) // Assuming 4 = Cancelled
                    .CountAsync();

                var failedOrders = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.Status == 5) // Assuming 5 = Return
                    .CountAsync();

                var returnOrders = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.Status == 7) // Assuming 6 = False
                    .CountAsync();

                // 5. Chart Data (Monthly Revenue for the Current Year)
                var monthlyRevenue = await _context.Order
                    .AsNoTracking()
                    .Where(o => o.CreatedDate >= startOfYear && o.Status == 3)
                    .GroupBy(o => new { o.CreatedDate.Year, o.CreatedDate.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalRevenue = g.Sum(o => o.TotalAmount)
                    })
                    .OrderBy(g => g.Year).ThenBy(g => g.Month)
                    .ToListAsync();

                // Format chart data
                var chartData = new
                {
                    Labels = monthlyRevenue.Select(m => $"{m.Month}/{m.Year}").ToList(),
                    Data = monthlyRevenue.Select(m => m.TotalRevenue).ToList()
                };

                // Combine all data into response
                var dashboardData = new
                {
                    Revenue = new
                    {
                        Today = revenueToday,
                        ThisMonth = revenueMonth,
                        ThisYear = revenueYear
                    },
                    TotalOrdersSold = totalOrdersSold,
                    PaymentMethodRevenue = paymentMethodRevenue,
                    OrderStatus = new
                    {
                        New = newOrders,
                        Processing = processingOrders,
                        Shipping = shippingOrders,
                        Completed = completedOrders,
                        Cancelled = cancelledOrders,
                        Failed = failedOrders,
                        Returns = returnOrders
                    },
                    ChartData = new
                    {
                        Type = "line", // Can be "line" or "bar" for frontend rendering
                        Data = chartData
                    }
                };

                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving dashboard data: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the dashboard request.");
            }
        }


        [HttpGet]
        [Route("GetOrderWithWebsite")]
        [BinaryAuthorize("OrderManager", ActionType.Xem)]
        public async Task<IActionResult> GetPagedOrdersByWebsiteAsync(int website, int? page, int pagesize = 5)
        {
            try
            {
                int currentPage = page ?? 1;

                var query = _context.Order
                    .AsNoTracking()
                    .Where(x => x.Website == website);

                int countDetails = await query.CountAsync();

                var orders = await query
                    .OrderByDescending(x => x.Id)
                    .Skip((currentPage - 1) * pagesize)
                    .Take(pagesize)
                    .ToListAsync();

                var result = new PageResult<Order>
                {
                    Count = countDetails,
                    PageIndex = currentPage,
                    PageSize = pagesize,
                    Items = orders
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving paged orders: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }


        // GET: api/Order/OrderWithDetails/5
        [HttpGet]
        [Route("OrderWithDetails/{id}")]
        [BinaryAuthorize("OrderManager", ActionType.Xem)]
        public async Task<IActionResult> GetOrderWithDetails([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == null)
            {
                return BadRequest("Order ID is required.");
            }

            try
            {
                // Fetch the Order
                var order = await _context.Order
                    .AsNoTracking()
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                // Fetch the OrderDetails with Product information
                var orderDetails = await _context.OrderDetail
                    .Join(_context.Products, od => od.ProductId, p => p.Id, (od, p) => new { od, p })
                    .Where(x => x.od.OrderId == id)
                    .Select(x => new DisplayOrderDetail
                    {
                        Id = x.od.Id,
                        ProductId = x.od.ProductId,
                        ProductCode = x.p.Code,
                        ProductName = x.p.Name,
                        Quantity = x.od.Quantity,
                        Price = x.od.Price,
                        Status = x.od.Status,
                        OrderId = x.od.OrderId,
                        DiscountVoucher = x.od.DiscountVoucher,
                        Gift = x.od.Gift,
                        Brand = x.od.Brand,
                        CreatedDate = x.od.CreatedDate,
                        ModifiedDate = x.od.ModifiedDate
                    })
                    .ToListAsync();

                // Combine Order and OrderDetails into a response object
                var result = new
                {
                    Order = order,
                    OrderDetails = orderDetails
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving order with details: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("orderSuccess")]
        [BinaryAuthorize("Order", ActionType.Xem)]
        public async Task<IActionResult> GetOrderAccountSS([FromQuery] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == null)
            {
                return BadRequest("AccountUserId is required.");
            }

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            // Kiểm tra nếu là Admin thì bỏ qua kiểm tra AccountUserId
            bool isAdmin = HasPermission("OrderManager", 1); // 1 = View
            if (!isAdmin && userId != id)
            {
                return Forbid("Bạn chỉ có thể xem đơn hàng của chính mình.");
            }

            var orders = await _context.Order
                .AsNoTracking()
                .Where(o => o.AccountUserId == id && o.Status == 3)
                .ToListAsync();

            var totalAmount = orders.Sum(o => o.TotalAmount);

            return Ok(new
            {
                orders,
                totalAmount
            });
        }


        [HttpGet]
        [Route("OrderAcc")]
        [BinaryAuthorize("Order", ActionType.Xem)]
        public async Task<IActionResult> GetOrderAccount([FromQuery] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            bool isAdmin = HasPermission("OrderManager", 1); // 1 = View
            if (!isAdmin && userId != id)
            {
                return Forbid("Bạn chỉ có thể xem đơn hàng của chính mình.");
            }

            var data = await _context.Order
                .Join(_context.AccountUsers, ac => ac.AccountUserId, ae => ae.Id, (ac, ae) => new { ac, ae })
                .Where(x => x.ac.AccountUserId == id)
                .Select(x => new Order
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
                })
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(data);
        }
        // GET: api/Orders/5
        [HttpGet("{id}")]
        [BinaryAuthorize("Order", ActionType.Xem)]
        public async Task<IActionResult> GetOrder([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var order = await _context.Order.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }

            bool isAdmin = HasPermission("OrderManager", 1); // 1 = View
            if (!isAdmin && order.AccountUserId != userId)
            {
                return Forbid("Bạn chỉ có thể xem đơn hàng của chính mình.");
            }

            return Ok(order);
        }

        [HttpPost]
        [Route("CancelOrder/{id}")]
        [BinaryAuthorize("Order", ActionType.Sua)] // Yêu cầu quyền chỉnh sửa
        public async Task<IActionResult> CancelOrder([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == null)
            {
                return BadRequest("Order ID is required.");
            }

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var order = await _context.Order.FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            bool isAdmin = HasPermission("OrderManager", 1); // Kiểm tra quyền admin (xem)
            if (!isAdmin && order.AccountUserId != userId)
            {
                return Forbid("Bạn chỉ có thể hủy đơn hàng của chính mình.");
            }

            if (order.Status == 3) // Giả sử Status = 3 là trạng thái "Đã hoàn thành"
            {
                return BadRequest("Đơn hàng đã hoàn thành, không thể hủy.");
            }
            if (order.Status == 4) // Giả sử Status = 4 là trạng thái "Đã hủy"
            {
                return BadRequest("Đơn hàng đã được hủy trước đó.");
            }

            // Cập nhật trạng thái đơn hàng thành "Đã hủy" (Status = 4)
            order.Status = 4; 
            order.ModifiedDate = DateTime.Now;

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Đơn hàng đã được hủy thành công", order });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật trạng thái đơn hàng" });
            }
        }

        // PUT: api/Orders/
        [HttpPost]
        [Route("PutOrder")]
        [BinaryAuthorize("OrderManager", ActionType.Sua)]
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
        [BinaryAuthorize("Order", ActionType.Them)]
        public async Task<IActionResult> PostOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Lấy userId từ token JWT
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            // Kiểm tra quyền admin
            bool isAdmin = HasPermission("OrderManager", (int)ActionType.Them); // Admin có quyền Thêm OrderManager
            if (!isAdmin && order.AccountUserId != userId)
            {
                return Forbid("Bạn chỉ có thể tạo đơn hàng cho chính mình.");
            }
            order.CreatedDate = DateTime.Now;
            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }
        private bool HasPermission(string module, int requiredPermission)
        {
            var permissionsClaim = User.FindFirst("Permissions")?.Value;
            if (string.IsNullOrEmpty(permissionsClaim))
                return false;

            var permissions = JsonConvert.DeserializeObject<Dictionary<string, int>>(permissionsClaim);
            return permissions.ContainsKey(module) && (permissions[module] & requiredPermission) == requiredPermission;
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        [BinaryAuthorize("OrderManager", ActionType.Xoa)]
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
        
        private bool OrderExists(int? id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}