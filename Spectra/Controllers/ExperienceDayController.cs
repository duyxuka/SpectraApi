using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Hangfire;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectra.Models;
using Spectra.Models.Authorize;
using System.Globalization;

namespace Spectra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AddCors")]
    [Authorize]
    public class ExperienceDayController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ExperienceDayController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/ExperienceDay
        [HttpGet]
        //[BinaryAuthorize("ExperienceDay", ActionType.Xem)]
        public IEnumerable<ExperienceDay> GetExperienceDays()
        {
            return _context.ExperienceDays.ToList();
        }

        [HttpGet]
        [Route("ExperienceDaysWebsite")]
        //[BinaryAuthorize("ExperienceDay", ActionType.Xem)]
        public IEnumerable<ExperienceDay> GetExperienceDaysHCM(int website)
        {
            return _context.ExperienceDays.AsNoTracking().Where(x => x.Website == website).OrderByDescending(x => x.CreateDate);

        }

        [HttpGet]
        [Route("ExperienceDaysWebsiteonPage")]
        //[BinaryAuthorize("ExperienceDay", ActionType.Xem)]
        public async Task<IActionResult> GetExperienceDaysOnPage(int website, int? page, int pagesize = 5)
        {
           try
            {
                int currentPage = page ?? 1;

                var query = _context.ExperienceDays.AsNoTracking().Where(x => x.Website == website);

                int countDetails = await query.CountAsync();

                var experday = await query
                    .OrderByDescending(x => x.CreateDate)
                    .Skip((currentPage - 1) * pagesize)
                    .Take(pagesize)
                    .ToListAsync();

                var result = new PageResult<ExperienceDay>
                {
                    Count = countDetails,
                    PageIndex = currentPage,
                    PageSize = pagesize,
                    Items = experday
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving paged orders: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        // GET: api/ExperienceDay/5
        [HttpGet("{id}")]
        //[BinaryAuthorize("ExperienceDay", ActionType.Xem)]
        public async Task<IActionResult> GetExperienceDay([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var experienceDay = await _context.ExperienceDays.FindAsync(id);

            if (experienceDay == null)
            {
                return NotFound();
            }

            return Ok(experienceDay);
        }

        [HttpGet]
        [Route("excel")]
        //[BinaryAuthorize("ExperienceDay", ActionType.XuatFile)]
        public async Task<FileResult> ExportExcel(int website, string query = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var data = await _context.ExperienceDays
                .Where(x => x.Website == website)
                .ToListAsync();

            // Lọc theo query
            if (!string.IsNullOrEmpty(query))
            {
                var normalizedQuery = RemoveVietnameseTones(query).ToLower();
                data = data.Where(e =>
                    (e.Name != null && RemoveVietnameseTones(e.Name).ToLower().Contains(normalizedQuery)) ||
                    (e.Email != null && RemoveVietnameseTones(e.Email).ToLower().Contains(normalizedQuery)) ||
                    (e.Phone != null && RemoveVietnameseTones(e.Phone).ToLower().Contains(normalizedQuery))
                ).ToList();
            }

            // Lọc theo ngày
            if (startDate.HasValue && endDate.HasValue)
            {
                data = data.Where(e => e.CreateDate >= startDate && e.CreateDate <= endDate).ToList();
            }
            else if (startDate.HasValue)
            {
                data = data.Where(e => e.CreateDate >= startDate).ToList();
            }
            else if (endDate.HasValue)
            {
                data = data.Where(e => e.CreateDate <= endDate).ToList();
            }

            Console.WriteLine($"Số bản ghi /experiencedays/excel: {data.Count}");
            var fileName = "danh-sach-dky-trai-nghiem.xlsx";
            return GenrateExcel(fileName, data);
        }
        private string RemoveVietnameseTones(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var normalized = text.Normalize(NormalizationForm.FormD);
            var result = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    result.Append(c);
                }
            }

            return result.ToString()
                .Replace("đ", "d")
                .Replace("Đ", "D")
                .Normalize(NormalizationForm.FormC);
        }
        private FileResult GenrateExcel(string filename, IEnumerable<ExperienceDay> experienceDays)
        {
            DataTable dataTable = new DataTable("dbo.Spectra_Warranty");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Tên"),
                new DataColumn("Email"),
                new DataColumn("Số điện thoại"),
                new DataColumn("Mẹ bầu hay mẹ bỉm"),
                new DataColumn("Số tuổi của bé"),
                new DataColumn("Máy hút sữa và mong muốn"),
                new DataColumn("Không mang theo người thân"),
                new DataColumn("Khung giờ đăng ký"),
                new DataColumn("Ngày đăng ký")
            });

            foreach (var experience in experienceDays)
            {
                dataTable.Rows.Add(experience.Name, experience.Email, experience.Phone, experience.Mom,
                                    experience.Old, experience.Breastpump, experience.Private, experience.Time, experience.CreateDate.ToString("dd/MM/yyyy - hh:mm:ss tt"));
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        , filename);
                }
            }
        }

        // PUT: api/ExperienceDay/5
        [HttpPut("{id}")]
        //[BinaryAuthorize("ExperienceDay", ActionType.Sua)]
        public async Task<IActionResult> PutExperienceDay([FromRoute] int? id, [FromBody] ExperienceDay experienceDay)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != experienceDay.Id)
            {
                return BadRequest();
            }

            _context.Entry(experienceDay).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExperienceDayExists(id))
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

        // POST: api/ExperienceDay
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostExperienceDay([FromBody] ExperienceDay experienceDay)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            experienceDay.CreateDate = DateTime.Now;
            _context.ExperienceDays.Add(experienceDay);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExperienceDay", new { id = experienceDay.Id }, experienceDay);
        }


        // DELETE: api/ExperienceDay/5
        [HttpDelete("{id}")]
        //[BinaryAuthorize("ExperienceDay", ActionType.Xoa)]
        public async Task<IActionResult> DeleteExperienceDay([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var experienceDay = await _context.ExperienceDays.FindAsync(id);
            if (experienceDay == null)
            {
                return NotFound();
            }

            _context.ExperienceDays.Remove(experienceDay);
            await _context.SaveChangesAsync();

            return Ok(experienceDay);
        }

        private bool ExperienceDayExists(int? id)
        {
            return _context.ExperienceDays.Any(e => e.Id == id);
        }
    }
}