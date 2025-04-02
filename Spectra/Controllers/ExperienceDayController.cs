using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Hangfire;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectra.Models;

namespace Spectra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AddCors")]
    public class ExperienceDayController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ExperienceDayController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/ExperienceDay
        [HttpGet]
        public IEnumerable<ExperienceDay> GetExperienceDays()
        {
            return _context.ExperienceDays.Where(x => x.Website == 1).OrderByDescending(x => x.CreateDate);
        }

        [HttpGet]
        [Route("HCM")]
        public IEnumerable<ExperienceDay> GetExperienceDaysHCM()
        {
            return _context.ExperienceDays.Where(x => x.Website == 2).OrderByDescending(x => x.CreateDate);
        }

        // GET: api/ExperienceDay/5
        [HttpGet("{id}")]
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
        public async Task<FileResult> ExportExcel(string query = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Lấy danh sách ban đầu
            var data = _context.ExperienceDays.Where(x => x.Website == 1).AsQueryable();

            // Lọc theo tên, email hoặc số điện thoại nếu có query
            if (!string.IsNullOrEmpty(query))
            {
                data = data.Where(e =>
                    e.Name.Contains(query) ||
                    e.Email.Contains(query) ||
                    e.Phone.Contains(query));
            }

            // Lọc theo ngày nếu có startDate và endDate
            if (startDate.HasValue && endDate.HasValue)
            {
                data = data.Where(e => e.CreateDate >= startDate && e.CreateDate <= endDate);
            }
            else if (startDate.HasValue)
            {
                data = data.Where(e => e.CreateDate >= startDate);
            }
            else if (endDate.HasValue)
            {
                data = data.Where(e => e.CreateDate <= endDate);
            }

            var result = await data.ToListAsync();
            var fileName = "danh-sach-dky-trai-nghiem.xlsx";

            return GenrateExcel(fileName, result);

        }

        [HttpGet]
        [Route("excelHCM")]
        public async Task<FileResult> ExportExcelHCM(string query = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Lấy danh sách ban đầu
            var data = _context.ExperienceDays.Where(x => x.Website == 2).AsQueryable();

            // Lọc theo tên, email hoặc số điện thoại nếu có query
            if (!string.IsNullOrEmpty(query))
            {
                data = data.Where(e =>
                    e.Name.Contains(query) ||
                    e.Email.Contains(query) ||
                    e.Phone.Contains(query));
            }

            // Lọc theo ngày nếu có startDate và endDate
            if (startDate.HasValue && endDate.HasValue)
            {
                data = data.Where(e => e.CreateDate >= startDate && e.CreateDate <= endDate);
            }
            else if (startDate.HasValue)
            {
                data = data.Where(e => e.CreateDate >= startDate);
            }
            else if (endDate.HasValue)
            {
                data = data.Where(e => e.CreateDate <= endDate);
            }

            var result = await data.ToListAsync();
            var fileName = "danh-sach-dky-trai-nghiem.xlsx";

            return GenrateExcel(fileName, result);

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