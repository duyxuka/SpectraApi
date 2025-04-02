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
    public class ServiceDetailController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ServiceDetailController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/ServiceDetail
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<ServiceDetail> GetServiceDetails()
        {
            try
            {
                string pattern = "[ ,+(){}.*+?^$|]";
                Regex rgx = new Regex(pattern);

                var data = _context.ServiceDetails
                    .AsNoTracking()
                    .Join(_context.Services, ai => ai.ServiceId, al => al.Id, (ai, al) => new
                    {
                        Id = ai.Id,
                        Code = ai.Code,
                        Name = ai.Name,
                        Status = ai.Status,
                        Description = ai.Description,
                        Image = ai.Image,
                        ServiceId = ai.ServiceId,
                        TitleSeo = ai.TitleSeo,
                        MetaKeyWords = ai.MetaKeyWords,
                        MetaDescription = ai.MetaDescription,
                        CreatedDate = ai.CreatedDate,
                        ModifiedDate = ai.ModifiedDate,
                        ServiceName = al.Name,
                        LinkName = rgx.Replace(ai.Name, "-").ToLower()
                    })
                    .Select(x => new ServiceDetailDisplay()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        Status = x.Status,
                        Description = x.Description,
                        Image = x.Image,
                        ServiceId = x.ServiceId,
                        TitleSeo = x.TitleSeo,
                        MetaKeyWords = x.MetaKeyWords,
                        MetaDescription = x.MetaDescription,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        ServiceName = x.ServiceName,
                        LinkName = rgx.Replace(x.Name, "-").ToLower()
                    })
                    .ToList();

                return data;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây, ví dụ log lại hoặc trả về lỗi phù hợp
                Console.WriteLine($"Lỗi trong quá trình lấy dữ liệu dịch vụ: {ex.Message}");
                throw; // Ném ngoại lệ để lớp điều khiển xử lý tiếp tục xử lý
            }
        }


        [HttpGet]
        [Route("getServiceId/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetServiceId([FromRoute] int? id)
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
                    var data = await context.ServiceDetails
                        .Join(context.Services, ai => ai.ServiceId, al => al.Id, (ai, al) => new
                        {
                            Id = ai.Id,
                            Name = ai.Name,
                            Code = ai.Code,
                            Image = ai.Image,
                            ServiceId = ai.ServiceId,
                            ServiceName = al.Name,
                            TitleSeo = al.TitleSeo,
                            MetaKeyWords = al.MetaKeyWords,
                            MetaDescription = al.MetaDescription,
                            Description = ai.Description,
                            Status = ai.Status,
                            LinkName = rgx.Replace(ai.Name, "-").ToLower()
                        })
                        .Where(x => x.ServiceId == id)
                        .Select(x => new ServiceDetailDisplay
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            Image = x.Image,
                            ServiceId = x.ServiceId,
                            ServiceName = x.ServiceName,
                            Description = x.Description,
                            TitleSeo = x.TitleSeo,
                            MetaKeyWords = x.MetaKeyWords,
                            MetaDescription = x.MetaDescription,
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
                // Xử lý ngoại lệ ở đây, ví dụ log lại hoặc trả về lỗi phù hợp
                Console.WriteLine($"Lỗi trong quá trình lấy dữ liệu dịch vụ theo ID: {ex.Message}");
                throw; // Ném ngoại lệ để lớp điều khiển xử lý tiếp tục xử lý
            }
        }


        // GET: api/ServiceDetail/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetServiceDetail([FromRoute] int? id)
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
                    var serviceDetail = await context.ServiceDetails
                        .Select(x => new ServiceDetailDisplay
                        {
                            Id = x.Id,
                            Code = x.Code,
                            Name = x.Name,
                            Description = x.Description,
                            Status = x.Status,
                            Image = x.Image,
                            TitleSeo = x.TitleSeo,
                            ServiceId = x.ServiceId,
                            MetaKeyWords = x.MetaKeyWords,
                            MetaDescription = x.MetaDescription,
                            CreatedDate = x.CreatedDate,
                            ModifiedDate = x.ModifiedDate,
                            LinkName = rgx.Replace(x.Name, "-").ToLower()
                        })
                        .Where(x => x.Id == id)
                        .FirstOrDefaultAsync();

                    if (serviceDetail == null)
                    {
                        return NotFound();
                    }

                    return Ok(serviceDetail);
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ ở đây, ví dụ log lại hoặc trả về lỗi phù hợp
                Console.WriteLine($"Lỗi trong quá trình lấy dữ liệu chi tiết dịch vụ theo ID: {ex.Message}");
                throw; // Ném ngoại lệ để lớp điều khiển xử lý tiếp tục xử lý
            }
        }


        // PUT: api/ServiceDetail/5
        [HttpPost]
        [Route("PutServiceDetail")]
        public async Task<IActionResult> PutServiceDetail([FromBody] ServiceDetail serviceDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(serviceDetail).State = EntityState.Modified;

            try
            {
                serviceDetail.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/ServiceDetail
        [HttpPost]
        public async Task<IActionResult> PostServiceDetail([FromBody] ServiceDetail serviceDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ServiceDetails.Add(serviceDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServiceDetail", new { id = serviceDetail.Id }, serviceDetail);
        }

        // DELETE: api/ServiceDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceDetail([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var serviceDetail = await _context.ServiceDetails.FindAsync(id);
            if (serviceDetail == null)
            {
                return NotFound();
            }

            _context.ServiceDetails.Remove(serviceDetail);
            await _context.SaveChangesAsync();

            return Ok(serviceDetail);
        }

        private bool ServiceDetailExists(int? id)
        {
            return _context.ServiceDetails.Any(e => e.Id == id);
        }
    }
}