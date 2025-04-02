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
    public class WelcomeDetailController : ControllerBase
    {
        private readonly AppDBContext _context;

        public WelcomeDetailController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/WelcomeDetail
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<WelcomeDetail> GetWelcomeDetails()
        {
            try
            {
                string pattern = "[ ,+(){}.*+?^$|]";
                Regex rgx = new Regex(pattern);

                using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
                {
                    var data = context.WelcomeDetails
                        .Join(context.Welcomes, ai => ai.WelcomeId, al => al.Id, (ai, al) => new
                        {
                            Id = ai.Id,
                            Name = ai.Name,
                            Code = ai.Code,
                            Image = ai.Image,
                            WelcomeId = ai.WelcomeId,
                            Description = ai.Description,
                            Status = ai.Status,
                            CateWelName = al.Name,
                            TitleSeo = ai.TitleSeo,
                            MetaKeyWords = ai.MetaKeyWords,
                            MetaDescription = ai.MetaDescription,
                            CreatedDate = ai.CreatedDate,
                            LinkName = rgx.Replace(ai.Name, "-").ToLower()
                        })
                        .Where(x => x.Status == true)
                        .Select(x => new WelcomeDetailDisplay
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            Image = x.Image,
                            WelcomeId = x.WelcomeId,
                            CateWelName = x.CateWelName,
                            TitleSeo = x.TitleSeo,
                            MetaKeyWords = x.MetaKeyWords,
                            MetaDescription = x.MetaDescription,
                            CreatedDate = x.CreatedDate,
                            Description = x.Description,
                            Status = x.Status,
                            LinkName = rgx.Replace(x.Name, "-").ToLower()
                        })
                        .ToList();

                    return data;
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây, ví dụ log lại hoặc trả về một thông báo lỗi phù hợp
                Console.WriteLine($"Lỗi trong quá trình lấy danh sách chi tiết chào mừng: {ex.Message}");
                throw; // Ném ngoại lệ để lớp điều khiển xử lý tiếp tục xử lý
            }
        }

        [HttpGet]
        [Route("WelcomeHome")]
        [AllowAnonymous]
        public IEnumerable<WelcomeDetail> GetWelcomeHome()
        {
            try
            {
                string pattern = "[ ,+(){}.*+?^$|]";
                Regex rgx = new Regex(pattern);

                using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
                {
                    var data = context.WelcomeDetails
                        .Join(context.Welcomes, ai => ai.WelcomeId, al => al.Id, (ai, al) => new { ai, al })
                        .Where(x => x.al.Name.ToLower().Contains("tin tức thị trường".Trim().ToLower()) && x.ai.Status == true)
                        .Select(x => new WelcomeDetailDisplay
                        {
                            Id = x.ai.Id,
                            Name = x.ai.Name,
                            WelcomeId = x.ai.WelcomeId,
                            Image = x.ai.Image,
                            CateWelName = x.al.Name,
                            Status = x.ai.Status,
                            CreatedDate = x.ai.CreatedDate,
                            LinkName = rgx.Replace(x.ai.Name, "-").ToLower()
                        })
                        .OrderByDescending(x => x.CreatedDate)
                        .Take(3)
                        .ToList();

                    return data;
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây, ví dụ log lại hoặc trả về một thông báo lỗi phù hợp
                Console.WriteLine($"Lỗi trong quá trình lấy danh sách chào mừng trang chủ: {ex.Message}");
                throw; // Ném ngoại lệ để lớp điều khiển xử lý tiếp tục xử lý
            }
        }

        // GET: api/WelcomeDetail/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWelcomeDetail([FromRoute] int? id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string pattern = "[ ,+(){}.*+?^$|]";
                Regex rgx = new Regex(pattern);

                using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
                {
                    var welcomeDetail = await context.WelcomeDetails
                        .Select(x => new WelcomeDetailDisplay
                        {
                            Id = x.Id,
                            Code = x.Code,
                            Name = x.Name,
                            Description = x.Description,
                            Status = x.Status,
                            Image = x.Image,
                            WelcomeId = x.WelcomeId,
                            TitleSeo = x.TitleSeo,
                            MetaKeyWords = x.MetaKeyWords,
                            MetaDescription = x.MetaDescription,
                            CreatedDate = x.CreatedDate,
                            ModifiedDate = x.ModifiedDate,
                            LinkName = rgx.Replace(x.Name, "-").ToLower()
                        })
                        .Where(x => x.Id == id)
                        .FirstOrDefaultAsync();

                    if (welcomeDetail == null)
                    {
                        return NotFound();
                    }

                    return Ok(welcomeDetail);
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây, ví dụ log lại hoặc trả về một thông báo lỗi phù hợp
                Console.WriteLine($"Lỗi trong quá trình lấy chi tiết chào mừng: {ex.Message}");
                throw; // Ném ngoại lệ để lớp điều khiển xử lý tiếp tục xử lý
            }
        }

        [HttpGet]
        [Route("getcategory/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory([FromRoute] int? id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string pattern = "[ ,+(){}.*+?^$|]";
                Regex rgx = new Regex(pattern);

                using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
                {
                    var data = await context.WelcomeDetails
                        .Join(context.Welcomes, ai => ai.WelcomeId,
                              al => al.Id, (ai, al) => new
                              {
                                  Id = ai.Id,
                                  Name = ai.Name,
                                  Code = ai.Code,
                                  Image = ai.Image,
                                  WelcomeId = ai.WelcomeId,
                                  Description = ai.Description,
                                  Status = ai.Status,
                                  TitleSeo = ai.TitleSeo,
                                  MetaKeyWords = ai.MetaKeyWords,
                                  MetaDescription = ai.MetaDescription,
                                  CreatedDate = ai.CreatedDate,
                                  LinkName = rgx.Replace(ai.Name, "-").ToLower()
                              })
                        .Where(x => x.WelcomeId == id)
                        .Select(x => new WelcomeDetailDisplay
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            Image = x.Image,
                            WelcomeId = x.WelcomeId,
                            TitleSeo = x.TitleSeo,
                            MetaKeyWords = x.MetaKeyWords,
                            MetaDescription = x.MetaDescription,
                            CreatedDate = x.CreatedDate,
                            Description = x.Description,
                            Status = x.Status,
                            LinkName = rgx.Replace(x.Name, "-").ToLower()
                        })
                        .Where(x => x.Status == true)
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

        // PUT: api/WelcomeDetail/5
        [HttpPost]
        [Route("PutWelcomeDetail")]
        public async Task<IActionResult> PutWelcomeDetail([FromBody] WelcomeDetail welcomeDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(welcomeDetail).State = EntityState.Modified;

            try
            {
                welcomeDetail.Status = true;
                welcomeDetail.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/WelcomeDetail
        [HttpPost]
        public async Task<IActionResult> PostWelcomeDetail([FromBody] WelcomeDetail welcomeDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            welcomeDetail.Status = true;
            welcomeDetail.CreatedDate = DateTime.Now;
            _context.WelcomeDetails.Add(welcomeDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWelcomeDetail", new { id = welcomeDetail.Id }, welcomeDetail);
        }

        // DELETE: api/WelcomeDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWelcomeDetail([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var welcomeDetail = await _context.WelcomeDetails.FindAsync(id);
            if (welcomeDetail == null)
            {
                return NotFound();
            }

            _context.WelcomeDetails.Remove(welcomeDetail);
            await _context.SaveChangesAsync();

            return Ok(welcomeDetail);
        }

        private bool WelcomeDetailExists(int? id)
        {
            return _context.WelcomeDetails.Any(e => e.Id == id);
        }
    }
}