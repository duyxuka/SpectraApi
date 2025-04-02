using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

    public class NewsDetailsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public NewsDetailsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/NewsDetails
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<NewsDetailDisplay> GetNewsDetails()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
                {
                    var data = context.NewsDetails
                    .AsNoTracking()
                    .Join(context.CategoryNews,
                          nd => nd.CategoryNewId,
                          cn => cn.Id,
                          (nd, cn) => new { nd, cn })
                    .Join(context.Category,
                          combined => combined.nd.CategoryId,
                          c => c.Id,
                          (combined, c) => new NewsDetailDisplay
                          {
                              Id = combined.nd.Id,
                              Code = combined.nd.Code,
                              Name = combined.nd.Name,
                              Status = combined.nd.Status,
                              Image = combined.nd.Image,
                              CategoryNewId = combined.nd.CategoryNewId,
                              CategoryId = combined.nd.CategoryId,
                              TitleSeo = combined.nd.TitleSeo,
                              MetaKeyWords = combined.nd.MetaKeyWords,
                              MetaDescription = combined.nd.MetaDescription,
                              CreatedDate = combined.nd.CreatedDate,
                              ModifiedDate = combined.nd.ModifiedDate,
                              CateNewName = combined.cn.Name,
                              CateName = c.Name,
                              LinkName = rgx.Replace(combined.nd.Name, "-").ToLower()
                          })
                    .Where(x => x.Status == true)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();

                    return data;
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây, ví dụ: logging, trả về lỗi phù hợp với yêu cầu
                // Ví dụ đơn giản, có thể thêm logging:
                Console.WriteLine($"Lỗi xảy ra trong GetNewsDetails: {ex.Message}");
                throw; // Ném ngoại lệ để báo hiệu rằng đã xử lý ngoại lệ và không thể tiếp tục thực thi
            }
        }


        // GET: api/NewsDetails
        [HttpGet]
        [Route("NewHome")]
        [AllowAnonymous]
        public IEnumerable<NewsDetailDisplay> GetNewsDetailsHome()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
                {
                    var data = context.NewsDetails
                    .AsNoTracking()
                    .Join(context.CategoryNews,
                          nd => nd.CategoryNewId,
                          cn => cn.Id,
                          (nd, cn) => new { nd, cn })
                    .Join(context.Category,
                          combined => combined.nd.CategoryId,
                          c => c.Id,
                          (combined, c) => new NewsDetailDisplay
                          {
                              Id = combined.nd.Id,
                              Name = combined.nd.Name,
                              Status = combined.nd.Status,
                              Description = combined.nd.Description.Substring(0, 200),
                              Image = combined.nd.Image,
                              CategoryNewId = combined.nd.CategoryNewId,
                              CategoryId = combined.nd.CategoryId,
                              CateNewName = combined.cn.Name,
                              CateName = c.Name,
                              LinkName = rgx.Replace(combined.nd.Name, "-").ToLower()
                          })
                    .Where(x => x.Status == true)
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(3)
                    .ToList();

                    return data;
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây, ví dụ: logging, trả về lỗi phù hợp với yêu cầu
                // Ví dụ đơn giản, có thể thêm logging:
                Console.WriteLine($"Lỗi xảy ra trong GetNewsDetailsHome: {ex.Message}");
                throw; // Ném ngoại lệ để báo hiệu rằng đã xử lý ngoại lệ và không thể tiếp tục thực thi
            }
        }


        [HttpGet]
        [Route("NewAdmin")]
        [AllowAnonymous]
        public IEnumerable<NewsDetailDisplay> GetNewsDetailsAdmin()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
                {
                    var data = context.NewsDetails
                    .AsNoTracking()
                    .Join(context.CategoryNews,
                          nd => nd.CategoryNewId,
                          cn => cn.Id,
                          (nd, cn) => new { nd, cn })
                    .Join(context.Category,
                          combined => combined.nd.CategoryId,
                          c => c.Id,
                          (combined, c) => new NewsDetailDisplay
                          {
                              Id = combined.nd.Id,
                              Code = combined.nd.Code,
                              Name = combined.nd.Name,
                              Status = combined.nd.Status,
                              Image = combined.nd.Image,
                              CategoryNewId = combined.nd.CategoryNewId,
                              CategoryId = combined.nd.CategoryId,
                              TitleSeo = combined.nd.TitleSeo,
                              MetaKeyWords = combined.nd.MetaKeyWords,
                              MetaDescription = combined.nd.MetaDescription,
                              CreatedDate = combined.nd.CreatedDate,
                              ModifiedDate = combined.nd.ModifiedDate,
                              CateNewName = combined.cn.Name,
                              CateName = c.Name,
                              LinkName = rgx.Replace(combined.nd.Name, "-").ToLower()
                          })
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();

                    return data;
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây, ví dụ: logging, trả về lỗi phù hợp với yêu cầu
                // Ví dụ đơn giản, có thể thêm logging:
                Console.WriteLine($"Lỗi xảy ra trong GetNewsDetailsAdmin: {ex.Message}");
                throw; // Ném ngoại lệ để báo hiệu rằng đã xử lý ngoại lệ và không thể tiếp tục thực thi
            }
        }


        // GET: api/NewsDetails/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewsDetail([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
                {
                    string pattern = "[ ,+(){}.*+?^$|]";
                    Regex rgx = new Regex(pattern);

                    var newsDetail = await context.NewsDetails
                        .AsNoTracking()
                        .Where(x => x.Id == id)
                        .Select(x => new NewsDetailDisplay
                        {
                            Id = x.Id,
                            Code = x.Code,
                            Name = x.Name,
                            Description = x.Description,
                            Status = x.Status,
                            Image = x.Image,
                            CategoryId = x.CategoryId,
                            CategoryNewId = x.CategoryNewId,
                            TitleSeo = x.TitleSeo,
                            MetaKeyWords = x.MetaKeyWords,
                            MetaDescription = x.MetaDescription,
                            CreatedDate = x.CreatedDate,
                            ModifiedDate = x.ModifiedDate,
                            LinkName = rgx.Replace(x.Name, "-").ToLower()
                        })
                        .FirstOrDefaultAsync();

                    if (newsDetail == null)
                    {
                        return NotFound();
                    }

                    return Ok(newsDetail);
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây, ví dụ: logging, trả về lỗi phù hợp với yêu cầu
                Console.WriteLine($"Lỗi xảy ra trong GetNewsDetail: {ex.Message}");
                return StatusCode(500, "Lỗi server, vui lòng thử lại sau.");
            }
        }


        [HttpGet]
        [Route("getcategorynew/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryNew([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
                {
                    string pattern = "[ ,+(){}.*+?^$|]";
                    Regex rgx = new Regex(pattern);

                    var data = await context.NewsDetails
                        .AsNoTracking()
                        .Where(x => x.CategoryNewId == id && x.Status == true)
                        .Select(x => new NewsDetailDisplay
                        {
                            Id = x.Id,
                            Name = x.Name,
                            CategoryNewId = x.CategoryNewId,
                            Description = x.Description.Substring(0, 300),
                            Image = x.Image,
                            TitleSeo = x.TitleSeo,
                            MetaKeyWords = x.MetaKeyWords,
                            MetaDescription = x.MetaDescription,
                            CreatedDate = x.CreatedDate,
                            Status = x.Status,
                            LinkName = rgx.Replace(x.Name, "-").ToLower()
                        })
                        .ToListAsync();

                    if (data == null || data.Count == 0)
                    {
                        return NotFound();
                    }

                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây, ví dụ: logging, trả về lỗi phù hợp với yêu cầu
                Console.WriteLine($"Lỗi xảy ra trong GetCategoryNew: {ex.Message}");
                return StatusCode(500, "Lỗi server, vui lòng thử lại sau.");
            }
        }

        [HttpGet]
        [Route("getCategoryId/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryId([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
                {
                    string pattern = "[ ,+(){}.*+?^$|]";
                    Regex rgx = new Regex(pattern);

                    var data = await context.NewsDetails
                        .AsNoTracking()
                        .Where(x => x.CategoryId == id && x.Status == true)
                        .Select(x => new NewsDetailDisplay
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            Image = x.Image,
                            CategoryId = x.CategoryId,
                            Description = x.Description,
                            Status = x.Status,
                            TitleSeo = x.TitleSeo,
                            MetaKeyWords = x.MetaKeyWords,
                            MetaDescription = x.MetaDescription,
                            CreatedDate = x.CreatedDate,
                            LinkName = rgx.Replace(x.Name, "-").ToLower()
                        })
                        .ToListAsync();

                    if (data == null || data.Count == 0)
                    {
                        return NotFound();
                    }

                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây, ví dụ: logging, trả về lỗi phù hợp với yêu cầu
                Console.WriteLine($"Lỗi xảy ra trong GetCategoryId: {ex.Message}");
                return StatusCode(500, "Lỗi server, vui lòng thử lại sau.");
            }
        }


        // PUT: api/NewsDetails/5
        [HttpPost]
        [Route("PutNewsDetails")]
        public async Task<IActionResult> PutNewsDetail([FromBody] NewsDetail newsDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(newsDetail).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return Ok(newsDetail);
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/NewsDetails
        [HttpPost]
        public async Task<IActionResult> PostNewsDetail([FromBody] NewsDetail newsDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.NewsDetails.Add(newsDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNewsDetail", new { id = newsDetail.Id }, newsDetail);

        }

        // DELETE: api/NewsDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsDetail([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newsDetail = await _context.NewsDetails.FindAsync(id);
            if (newsDetail == null)
            {
                return NotFound();
            }

            _context.NewsDetails.Remove(newsDetail);
            await _context.SaveChangesAsync();

            return Ok(newsDetail);
        }

        private bool NewsDetailExists(int? id)
        {
            return _context.NewsDetails.Any(e => e.Id == id);
        }
    }
}