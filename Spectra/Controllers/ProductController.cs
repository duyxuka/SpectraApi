using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    
    public class ProductController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IServiceManagercs _serviceManagercs;
        public ProductController(AppDBContext context, IServiceManagercs serviceManagercs)
        {
            _serviceManagercs = serviceManagercs;
            _context = context;
        }

        // GET: api/Product
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<ProductDisplay> GetProducts()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var data = _context.Products
                    .AsNoTracking()
                    .Join(_context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(_context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Where(x => x.gt.ai.Status == true)
                    .Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Code = x.gt.ai.Code,
                        Name = x.gt.ai.Name,
                        WarrantyMonth = x.gt.ai.WarrantyMonth,
                        Price = x.gt.ai.Price,
                        SalePrice = x.gt.ai.SalePrice,
                        Images = x.gt.ai.Images,
                        JobId = x.gt.ai.JobId,
                        CategoryId = x.gt.ai.CategoryId,
                        Option = x.gt.ai.Option,
                        GiftId = x.gt.ai.GiftId,
                        Status = x.gt.ai.Status,
                        ScheduleStatus = x.gt.ai.ScheduleStatus,
                        CreatedDate = x.gt.ai.CreatedDate,
                        ModifiedDate = x.gt.ai.ModifiedDate,
                        CategoryName = x.gt.al.Name,
                        GiftName = x.pr.Name,
                        GiftPrice = x.pr.Price,
                        Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                        LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                    })
                    .ToList();

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                // Log the exception or handle it accordingly
                return Enumerable.Empty<ProductDisplay>();
            }
        }
        //[HttpGet]
        //[Route("ProductHome")]
        //[AllowAnonymous]
        //public IEnumerable<Product> GetProductsLaster()
        //{
        //    string pattern = "[ ,+(){}.*+?^$|]";
        //    Regex rgx = new Regex(pattern);
        //    var data = _context.Products
        // .Join(_context.Category, ai => ai.CategoryId,
        //       al => al.Id, (ai, al) => new { ai, al }).Join(_context.Gift, gt => gt.ai.GiftId,
        //      pr => pr.Id, (gt, pr) => new { gt, pr }).Select(x => new ProductDisplay
        //      {
        //          Id = x.gt.ai.Id,
        //          Code = x.gt.ai.Code,
        //          Name = x.gt.ai.Name,
        //          Description = x.gt.ai.Description,
        //          Instruct = x.gt.ai.Instruct,
        //          Price = x.gt.ai.Price,
        //          SalePrice = x.gt.ai.SalePrice,
        //          WarrantyMonth = x.gt.ai.WarrantyMonth,
        //          TitleDescription = x.gt.ai.TitleDescription,
        //          TitleSeo = x.gt.ai.TitleSeo,
        //          MetaKeyWords = x.gt.ai.MetaKeyWords,
        //          MetaDescription = x.gt.ai.MetaDescription,
        //          Images = x.gt.ai.Images,
        //          CategoryId = x.gt.ai.CategoryId,
        //          Option = x.gt.ai.Option,
        //          ScheduleStatus = x.gt.ai.ScheduleStatus,
        //          GiftId = x.gt.ai.GiftId,
        //          JobId = x.gt.ai.JobId,
        //          Status = x.gt.ai.Status,
        //          Information = x.gt.ai.Information,
        //          Start = x.gt.ai.Start,
        //          Ends = x.gt.ai.Ends,
        //          CreatedDate = x.gt.ai.CreatedDate,
        //          ModifiedDate = x.gt.ai.ModifiedDate,
        //          CategoryName = x.gt.al.Name,
        //          GiftName = x.pr.Name,
        //          GiftPrice = x.pr.Price,
        //          Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
        //          LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()

        //      }).OrderBy(x => x.Code).ToList();
        //    return data;
        //}

        [HttpPost]
        [Route("ProductHangfire")]
        [BinaryAuthorize("Product", ActionType.Sua)]
        public async Task<IActionResult> ProductHangfire([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                DateTime endDateTime = Convert.ToDateTime(product.Ends);
                DateTime startDateTime = Convert.ToDateTime(product.Start);
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
                var jobId = BackgroundJob.Schedule<IServiceManagercs>(
                    x => x.UpdateDatabase(product),
                    TimeSpan.FromSeconds(secondsUntilStart));
                var jobId1 = BackgroundJob.Schedule<IServiceManagercs>(
                    x => x.UpdateDatabaseAgain(product),
                    TimeSpan.FromSeconds(secondsUntilStart + secondsBetweenStartAndEnd));
                // Update the database with the jobId1
                await _serviceManagercs.UpdateDatabaseJobIdAsync(product,jobId1);

                return Ok(product);
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
        [Route("ProductHangfireCancel")]
        [BinaryAuthorize("Product", ActionType.Sua)]
        public IActionResult ProductHangfireCancel([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string[] jobss = product.JobId.Split('-');

            foreach (string job in jobss)
            {
                BackgroundJob.Delete(job);
            }
            
            BackgroundJob.Enqueue<IServiceManagercs>(x => x.UpdateDatabaseAgain(product));

            try
            {
                return Ok(product);
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return NoContent();

        }

        [HttpGet]
        [Route("ProductWaranty")]
        [AllowAnonymous]
        public IEnumerable<ProductDisplay> GetProductsWaranty()
        {
            try
            {
                var data = _context.Products
                    .AsNoTracking()
                    .Join(_context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(_context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Where(x => x.gt.al.Option == true) // Assuming Category.Option represents Warranty information
                    .Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Code = x.gt.ai.Code,
                        Name = x.gt.ai.Name,
                        CategoryId = x.gt.ai.CategoryId,
                        GiftId = x.gt.ai.GiftId,
                        WarrantyMonth = x.gt.ai.WarrantyMonth,
                        CategoryName = x.gt.al.Name,
                        CategoryWaranty = x.gt.al.Option
                    })
                    .ToList();

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products with warranty: {ex.Message}");
                // Log the exception or handle it accordingly
                return Enumerable.Empty<ProductDisplay>(); // Return an empty collection or handle the error case
            }
        }

        [HttpGet]
        [Route("ProductMHS")]
        [AllowAnonymous]
        public IActionResult GetProductsMHS(int? page, int pagesize = 4)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var query = _context.Products
                    .AsNoTracking()
                    .Join(_context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(_context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Where(x => x.gt.al.Name.ToLower().Contains("máy hút sữa".ToLower()));
                var countDetails = query.Count();

                var result = new PageResult<ProductDisplay>
                {
                    Count = countDetails,
                    PageIndex = page ?? 1,
                    PageSize = pagesize,
                    Items = query
                        .OrderByDescending(x => x.gt.ai.CreatedDate) // Thay đổi phương thức sắp xếp nếu cần thiết
                        .Skip((page - 1 ?? 0) * pagesize)
                        .Take(pagesize)
                        .Select(x => new ProductDisplay
                        {
                            Id = x.gt.ai.Id,
                            Name = x.gt.ai.Name,
                            TitleDescription = x.gt.ai.TitleDescription.Substring(0, 100),
                            Price = x.gt.ai.Price,
                            SalePrice = x.gt.ai.SalePrice,
                            Images = x.gt.ai.Images,
                            CategoryName = x.gt.al.Name,
                            Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                            LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                        })
                        .ToList()
                };

                return Ok(result); // Trả về kết quả thành công với mã 200
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                // Sử dụng Problem để trả về JSON hợp lệ cho lỗi
                return StatusCode(500, "An error occurred while processing the request."); // Trả về mã lỗi 500 nếu có sự cố
            }
        }



        [HttpGet]
        [Route("ProductMTT")]
        [AllowAnonymous]
        public IActionResult GetProductsMTT(int? page, int pagesize = 4)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var query = _context.Products
                    .AsNoTracking()
                    .Join(_context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(_context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Where(x => x.gt.al.Name.ToLower().Contains("tiệt trùng"));
                var countDetails = query.Count();

                var result = new PageResult<ProductDisplay>
                {
                    Count = countDetails,
                    PageIndex = page ?? 1,
                    PageSize = pagesize,
                    Items = query
                        .OrderByDescending(x => x.gt.ai.CreatedDate)
                        .Skip((page - 1 ?? 0) * pagesize)
                        .Take(pagesize)
                        .Select(x => new ProductDisplay
                        {
                            Id = x.gt.ai.Id,
                            Name = x.gt.ai.Name,
                            TitleDescription = x.gt.ai.TitleDescription.Substring(0, 100),
                            Price = x.gt.ai.Price,
                            SalePrice = x.gt.ai.SalePrice,
                            Images = x.gt.ai.Images,
                            CategoryName = x.gt.al.Name,
                            Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                            LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                        })
                        .ToList()
                };

                return Ok(result); // Trả về kết quả thành công với mã 200
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request."); // Trả về mã lỗi 500 nếu có sự cố
            }
        }


        [HttpGet]
        [Route("ProductTS")]
        [AllowAnonymous]
        public IActionResult GetProductsTS(int? page, int pagesize = 4)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var query = _context.Products
                    .AsNoTracking()
                    .Join(_context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(_context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Where(x => x.gt.al.Name.ToLower().Contains("trữ sữa"));

                var countDetails = query.Count();

                var result = new PageResult<ProductDisplay>
                {
                    Count = countDetails,
                    PageIndex = page ?? 1,
                    PageSize = pagesize,
                    Items = query
                        .OrderByDescending(x => x.gt.ai.CreatedDate) // Adjust the sorting method as needed
                        .Skip((page - 1 ?? 0) * pagesize)
                        .Take(pagesize)
                        .Select(x => new ProductDisplay
                        {
                            Id = x.gt.ai.Id,
                            Name = x.gt.ai.Name,
                            TitleDescription = x.gt.ai.TitleDescription.Substring(0, 100),
                            Price = x.gt.ai.Price,
                            SalePrice = x.gt.ai.SalePrice,
                            Images = x.gt.ai.Images,
                            CategoryName = x.gt.al.Name,
                            Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                            LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                        })
                        
                        .ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (this can be to a file, database, etc.)
                Console.WriteLine($"Error retrieving products: {ex.Message}");

                // Return the error as a proper JSON response
                return StatusCode(500, new { message = "An error occurred while processing the request.", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("ProductBS")]
        [AllowAnonymous]
        public IActionResult GetProductsBS(int? page, int pagesize = 4)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var query = _context.Products
                    .AsNoTracking()
                    .Join(_context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(_context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Where(x => x.gt.al.Name.ToLower().Contains("bình sữa"));
                var countDetails = query.Count();

                var result = new PageResult<ProductDisplay>
                {
                    Count = countDetails,
                    PageIndex = page ?? 1,
                    PageSize = pagesize,
                    Items = query
                        .OrderByDescending(x => x.gt.ai.CreatedDate) // Thay đổi phương thức sắp xếp nếu cần thiết
                        .Skip((page - 1 ?? 0) * pagesize)
                        .Take(pagesize)
                        .Select(x => new ProductDisplay
                        {
                            Id = x.gt.ai.Id,
                            Name = x.gt.ai.Name,
                            TitleDescription = x.gt.ai.TitleDescription.Substring(0, 100),
                            Price = x.gt.ai.Price,
                            SalePrice = x.gt.ai.SalePrice,
                            Images = x.gt.ai.Images,
                            CategoryName = x.gt.al.Name,
                            Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                            LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                        })
                        .ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Error retrieving products: {ex.Message}");

                // Return a proper JSON error response
                return StatusCode(500, new { message = "An error occurred while processing the request.", error = ex.Message });
            }
        }



        [HttpGet]
        [Route("ProductPK")]
        [AllowAnonymous]
        public IActionResult GetProductsPK()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var data = _context.Products
                    .AsNoTracking()
                    .Join(_context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(_context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Where(x => x.gt.al.Name.ToLower().Contains("hâm sữa") || x.gt.al.Name.ToLower().Contains("phụ kiện"))
                    .Take(3)
                    .Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Name = x.gt.ai.Name.Length > 30 ? x.gt.ai.Name.Substring(0, 30) + "..." : x.gt.ai.Name,
                        Price = x.gt.ai.Price,
                        SalePrice = x.gt.ai.SalePrice,
                        Images = x.gt.ai.Images,
                        CategoryId = x.gt.ai.CategoryId,
                        Option = x.gt.ai.Option,
                        GiftId = x.gt.ai.GiftId,
                        Status = x.gt.ai.Status,
                        CategoryName = x.gt.al.Name,
                        TitleDescription = x.gt.ai.TitleDescription.Substring(0, 120),
                        CategoryCode = x.gt.al.Code,
                        GiftName = x.pr.Name,
                        GiftPrice = x.pr.Price,
                        Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                        LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                    })
                    .ToList();

                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        [Route("ProductGift")]
        [AllowAnonymous]
        public IEnumerable<Product> GetProductsGift()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);
            var data = _context.Products
        .Join(_context.Category, ai => ai.CategoryId,
              al => al.Id, (ai, al) => new { ai, al }).Join(_context.Gift, gt => gt.ai.GiftId,
              pr => pr.Id, (gt, pr) => new { gt, pr }).Select(x => new ProductDisplay
              {
                  Id = x.gt.ai.Id,
                  Code = x.gt.ai.Code,
                  Name = x.gt.ai.Name,
                  Price = x.gt.ai.Price,
                  SalePrice = x.gt.ai.SalePrice,
                  Images = x.gt.ai.Images,
                  CategoryId = x.gt.ai.CategoryId,
                  Option = x.gt.ai.Option,
                  GiftId = x.gt.ai.GiftId,
                  Status = x.gt.ai.Status,
                  CreatedDate = x.gt.ai.CreatedDate,
                  ModifiedDate = x.gt.ai.ModifiedDate,
                  CategoryName = x.gt.al.Name,
                  CategoryCode = x.gt.al.Code,
                  GiftName = x.pr.Name,
                  GiftPrice = x.pr.Price,
                  Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                  LinkName = x.gt.ai.Name.Replace(" ", "-").ToLower()

              }).Where(x => x.GiftPrice > 0).ToList();
            return data;
        }

        [HttpGet]
        [Route("getcategoryID/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryID([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var data = await _context.Products
                    .AsNoTracking()
                    .Join(_context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(_context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Where(x => x.gt.ai.CategoryId == id)
                    .Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Name = x.gt.ai.Name.Length > 30 ? x.gt.ai.Name.Substring(0, 30) + "..." : x.gt.ai.Name,
                        Price = x.gt.ai.Price,
                        SalePrice = x.gt.ai.SalePrice,
                        TitleSeo = x.gt.ai.TitleSeo,
                        Images = x.gt.ai.Images,
                        CategoryId = x.gt.ai.CategoryId,
                        GiftId = x.gt.ai.GiftId,
                        Status = x.gt.ai.Status,
                        CategoryName = x.gt.al.Name,
                        TitleDescription = x.gt.ai.TitleDescription.Substring(0, 120),
                        GiftName = x.pr.Name,
                        GiftPrice = x.pr.Price,
                        Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                        LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                    })
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }


        [HttpGet]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSearch([FromQuery(Name = "code")] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var products = await _context.Products
                    .AsNoTracking()
                    .Join(_context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(_context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Where(x => x.gt.ai.Name.ToLower().Contains(name.Trim().ToLower()))
                    .Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Name = x.gt.ai.Name.Length > 30 ? x.gt.ai.Name.Substring(0, 30) + "..." : x.gt.ai.Name,
                        Price = x.gt.ai.Price,
                        SalePrice = x.gt.ai.SalePrice,
                        Option = x.gt.ai.Option,
                        Images = x.gt.ai.Images,
                        CategoryId = x.gt.ai.CategoryId,
                        GiftId = x.gt.ai.GiftId,
                        TitleDescription = x.gt.ai.TitleDescription.Substring(0, 120),
                        Status = x.gt.ai.Status,
                        CategoryName = x.gt.al.Name,
                        GiftName = x.pr.Name,
                        GiftPrice = x.pr.Price,
                        Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                        LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                    })
                    .ToListAsync();

                if (products == null || products.Count == 0)
                {
                    return NotFound();
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet("detail/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductDetailAll(int id)
        {
            try
            {
                var product = await _context.Products
                    .Where(p => p.Id == id)
                    .Include(p => p.Category)
                    .Include(p => p.Gift)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(v => v.ProductVariantAttributes)
                            .ThenInclude(va => va.ValueAttribute)
                                .ThenInclude(a => a.Attribute)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (product == null)
                    return NotFound();

                // Tính % giảm giá
                float giaphantram = product.Price > 0 ? 100 - ((product.SalePrice * 100) / product.Price) : 0;

                var result = new
                {
                    Id = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    Description = product.Description,
                    TitleDescription = product.TitleDescription,
                    Instruct = product.Instruct,
                    TitleSeo = product.TitleSeo,
                    MetaKeyWords = product.MetaKeyWords,
                    MetaDescription = product.MetaDescription,
                    Images = product.Images,
                    Price = product.Price,
                    SalePrice = product.SalePrice,
                    Option = product.Option,
                    Start = product.Start,
                    Ends = product.Ends,
                    CreatedDate = product.CreatedDate,
                    ModifiedDate = product.ModifiedDate,
                    JobId = product.JobId,
                    WarrantyMonth = product.WarrantyMonth,
                    ScheduleStatus = product.ScheduleStatus,
                    Status = product.Status,
                    Information = product.Information,

                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name,
                    CategoryCode = product.Category?.Code,

                    GiftId = product.GiftId,
                    GiftName = product.Gift?.Name,
                    GiftPrice = product.Gift?.Price,

                    Giaphantram = giaphantram,
                    LinkName = product.Name?.Replace(" ", "-").ToLower(),

                    Variants = product.ProductVariants.Select(v => new VariantDto
                    {
                        VariantId = v.Id,
                        Price = v.Price,
                        SalePrice = v.SalePrice,
                        Attributes = v.ProductVariantAttributes.Select(va => new AttributeDto
                        {
                            AttributeName = va.ValueAttribute.Attribute.Name,
                            ValueName = va.ValueAttribute.Name
                        }).ToList()
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin sản phẩm: {ex.Message}");
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu.");
            }
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var product = await _context.Products
                    .AsNoTracking()
                    .Join(_context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                    .Join(_context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                    .Where(x => x.gt.ai.Id == id)
                    .Select(x => new ProductDisplay
                    {
                        Id = x.gt.ai.Id,
                        Code = x.gt.ai.Code,
                        Name = x.gt.ai.Name,
                        Description = x.gt.ai.Description,
                        TitleDescription = x.gt.ai.TitleDescription,
                        Instruct = x.gt.ai.Instruct,
                        TitleSeo = x.gt.ai.TitleSeo,
                        MetaKeyWords = x.gt.ai.MetaKeyWords,
                        MetaDescription = x.gt.ai.MetaDescription,
                        Price = x.gt.ai.Price,
                        SalePrice = x.gt.ai.SalePrice,
                        Option = x.gt.ai.Option,
                        Images = x.gt.ai.Images,
                        Start = x.gt.ai.Start,
                        Ends = x.gt.ai.Ends,
                        CategoryId = x.gt.ai.CategoryId,
                        GiftId = x.gt.ai.GiftId,
                        Status = x.gt.ai.Status,
                        JobId = x.gt.ai.JobId,
                        ScheduleStatus = x.gt.ai.ScheduleStatus,
                        WarrantyMonth = x.gt.ai.WarrantyMonth,
                        Information = x.gt.ai.Information,
                        CreatedDate = x.gt.ai.CreatedDate,
                        ModifiedDate = x.gt.ai.ModifiedDate,
                        CategoryName = x.gt.al.Name,
                        CategoryCode = x.gt.al.Code,
                        GiftName = x.pr.Name,
                        GiftPrice = x.pr.Price,
                        Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                        LinkName = x.gt.ai.Name.Replace(" ", "-").ToLower()
                    })
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    return NotFound();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving product details: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }


        [HttpGet]
        [Route("ProductNewCate")]
        [AllowAnonymous]
        public IEnumerable<ProductDisplay> GetProductsNew()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            var data = _context.Products
                .AsNoTracking()
                .Join(_context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                .Join(_context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                .OrderByDescending(x => x.gt.ai.Code)
                .Take(3)
                .Select(x => new ProductDisplay
                {
                    Id = x.gt.ai.Id,
                    Code = x.gt.ai.Code,
                    Name = x.gt.ai.Name,
                    Price = x.gt.ai.Price,
                    SalePrice = x.gt.ai.SalePrice,
                    Option = x.gt.ai.Option,
                    Images = x.gt.ai.Images,
                    CategoryId = x.gt.ai.CategoryId,
                    GiftId = x.gt.ai.GiftId,
                    TitleDescription = x.gt.ai.TitleDescription.Substring(0, 60),
                    Status = x.gt.ai.Status,
                    CreatedDate = x.gt.ai.CreatedDate,
                    ModifiedDate = x.gt.ai.ModifiedDate,
                    CategoryName = x.gt.al.Name,
                    GiftName = x.pr.Name,
                    GiftPrice = x.pr.Price,
                    Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                    LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                })
                .ToList();

            return data;
        }

        [HttpGet]
        [Route("ProductNews")]
        [AllowAnonymous]
        public IEnumerable<ProductDisplay> GetProductsNews()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            var data = _context.Products
                .AsNoTracking()
                .Join(_context.Category, ai => ai.CategoryId, al => al.Id, (ai, al) => new { ai, al })
                .Join(_context.Gift, gt => gt.ai.GiftId, pr => pr.Id, (gt, pr) => new { gt, pr })
                .OrderByDescending(x => x.gt.ai.CreatedDate)
                .Select(x => new ProductDisplay
                {
                    Id = x.gt.ai.Id,
                    Code = x.gt.ai.Code,
                    Name = x.gt.ai.Name,
                    Price = x.gt.ai.Price,
                    TitleDescription = x.gt.ai.TitleDescription,
                    SalePrice = x.gt.ai.SalePrice,
                    Option = x.gt.ai.Option,
                    Images = x.gt.ai.Images,
                    CategoryId = x.gt.ai.CategoryId,
                    GiftId = x.gt.ai.GiftId,
                    Status = x.gt.ai.Status,
                    CreatedDate = x.gt.ai.CreatedDate,
                    ModifiedDate = x.gt.ai.ModifiedDate,
                    CategoryName = x.gt.al.Name,
                    GiftName = x.pr.Name,
                    GiftPrice = x.pr.Price,
                    Giaphantram = 100 - ((x.gt.ai.SalePrice * 100) / x.gt.ai.Price),
                    LinkName = rgx.Replace(x.gt.ai.Name, "-").ToLower()
                })
                .ToList();

            return data;
        }


        [HttpGet]
        [Route("TrashProduct")]
        [BinaryAuthorize("Product", ActionType.Xoa)]
        public IEnumerable<Product> GetTrashProduct()
        {
            var data = _context.Products
         .Join(_context.Category, ai => ai.CategoryId,
               al => al.Id, (ai, al) => new { ai, al }).Select(x => new ProductDisplay
              {
                  Id = x.ai.Id,
                  Code = x.ai.Code,
                  Name = x.ai.Name,
                  Price = x.ai.Price,
                  Images = x.ai.Images,
                  CategoryId = x.ai.CategoryId,
                  Status = x.ai.Status,
                  CreatedDate = x.ai.CreatedDate,
                  ModifiedDate = x.ai.ModifiedDate,
                  CategoryName = x.al.Name,
                  LinkName = x.ai.Name.Replace(" ", "-").ToLower()
              }).Where(x => x.Status == false).ToList();
            return data;
        }

        [HttpPost]
        [Route("RepeatProduct")]
        [BinaryAuthorize("Product", ActionType.Xoa)]
        public async Task<IActionResult> RepeatProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                product.Status = true;
                await _context.SaveChangesAsync();
                return Ok(product);
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        [HttpPost]
        [Route("TemporaryDelete")]
        [BinaryAuthorize("Product", ActionType.Xoa)]
        public async Task<IActionResult> TemporaryDelete([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                product.Status = false;
                //categoryProduct.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok(product);
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }


        // PUT: api/Product/5
        [HttpPost]
        [Route("PutProduct")]
        [BinaryAuthorize("Product", ActionType.Sua)]
        public async Task<IActionResult> PutProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                
                product.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok(product);
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Product
        [HttpPost]
        [BinaryAuthorize("Product", ActionType.Them)]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            product.ScheduleStatus = false;
            product.CreatedDate = DateTime.Now;
            product.Start = new DateTime(0001, 01, 01, 00, 00, 00); 
            product.Ends = new DateTime(0001, 01, 01, 00, 00, 00); 
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        [BinaryAuthorize("Product", ActionType.Xoa)]
        public async Task<IActionResult> DeleteProduct([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        private bool ProductExists(int? id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}