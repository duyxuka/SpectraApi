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
    public class ApplicationController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ApplicationController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Application
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<ApplicationDisplay> GetApplications()
        {
            using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
            {
                var data = context.Applications
                .AsNoTracking()
                .Join(context.Category,
                      ai => ai.CategoryId,
                      al => al.Id,
                      (ai, al) => new
                      {
                          Id = ai.Id,
                          Name = ai.Name,
                          Code = ai.Code,
                          CategoryId = ai.CategoryId,
                          Description = ai.Description,
                          Status = ai.Status,
                          Image = ai.Image,
                          CateName = al.Name,
                          LinkName = ai.Name.Replace(" ", "-").ToLower()
                      })
                .Where(x => x.Status)
                .Select(x => new ApplicationDisplay
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    CategoryId = x.CategoryId,
                    Description = x.Description,
                    Status = x.Status,
                    Image = x.Image,
                    CateName = x.CateName,
                    LinkName = x.LinkName
                })
                .ToList();

                return data;
            }
        }


        // GET: api/Application/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetApplication([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var application = await _context.Applications.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            return Ok(application);
        }

        [HttpGet]
        [Route("getcategory/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var context = _context) // Thay YourDbContext bằng tên thực của DbContext của bạn
            {
                var data = await context.Applications
                .AsNoTracking()
                .Join(context.Category,
                      ai => ai.CategoryId,
                      al => al.Id,
                      (ai, al) => new ApplicationDisplay
                      {
                          Id = ai.Id,
                          Name = ai.Name,
                          Code = ai.Code,
                          Image = ai.Image,
                          CategoryId = ai.CategoryId,
                          Description = ai.Description,
                          Status = ai.Status,
                          LinkName = ai.Name.Replace(" ", "-").ToLower()
                      })
                .FirstOrDefaultAsync(x => x.CategoryId == id);

                if (data == null)
                {
                    return NotFound();
                }

                return Ok(data);
            }
        }

        // PUT: api/Application/5
        [HttpPost]
        [Route("PutApplication")]
        public async Task<IActionResult> PutApplication([FromBody] Application application)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(application).State = EntityState.Modified;

            try
            {
                application.Status = true;
                application.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Application
        [HttpPost]
        public async Task<IActionResult> PostApplication([FromBody] Application application)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            application.CreatedDate = DateTime.Now;
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            application = _context.Applications.Join(_context.Category, ca => ca.CategoryId,
              ne => ne.Id, (ca, ne) => new { ca, ne }).Where(x => x.ca.Id == application.Id).Select(x => new ApplicationDisplay
              {
                  Id = x.ca.Id,
                  Code = x.ca.Code,
                  Name = x.ca.Name,
                  Description = x.ca.Description,
                  Image = x.ca.Image,
                  CategoryId = x.ca.CategoryId,
                  Status = x.ca.Status,
                  CreatedDate = x.ca.CreatedDate,
                  ModifiedDate = x.ca.ModifiedDate,
                  CateName = x.ne.Name
              }).FirstOrDefault();

            return CreatedAtAction("GetApplication", new { id = application.Id }, application);
        }

        // DELETE: api/Application/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var application = await _context.Applications.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            return Ok(application);
        }

        private bool ApplicationExists(int? id)
        {
            return _context.Applications.Any(e => e.Id == id);
        }
    }
}