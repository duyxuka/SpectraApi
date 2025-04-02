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
    
    public class CategoryNewsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public CategoryNewsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/CategoryNews
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<CategoryNewDisplay> GetCategoryNews()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            var data = _context.CategoryNews
                .AsNoTracking()
                .Where(x => x.Status)
                .Select(x => new CategoryNewDisplay
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Status = x.Status,
                    TitleSeo = x.TitleSeo,
                    MetaDescription = x.MetaDescription,
                    MetaKeyWords = x.MetaKeyWords,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    LinkName = rgx.Replace(x.Name, "-").ToLower()
                })
                .ToList();

            return data;
        }


        // GET: api/CategoryNews/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryNew([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryNew = await _context.CategoryNews
                .AsNoTracking()
                .Where(x => x.Id == id && x.Status)
                .Select(x => new CategoryNewDisplay
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Status = x.Status,
                    TitleSeo = x.TitleSeo,
                    MetaDescription = x.MetaDescription,
                    MetaKeyWords = x.MetaKeyWords,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    LinkName = Regex.Replace(x.Name, "[ ,+(){}.*+?^$|]", "-").ToLower()
                })
                .FirstOrDefaultAsync();

            if (categoryNew == null)
            {
                return NotFound();
            }

            return Ok(categoryNew);
        }


        // PUT: api/CategoryNews/5
        [HttpPost]
        [Route("PutCategoryNews")]
        public async Task<IActionResult> PutCategoryNews([FromBody] CategoryNew categoryNew)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(categoryNew).State = EntityState.Modified;

            try
            {
                categoryNew.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/CategoryNews
        [HttpPost]
        public async Task<IActionResult> PostCategoryNew([FromBody] CategoryNew categoryNew)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            categoryNew.CreatedDate = DateTime.Now;
            _context.CategoryNews.Add(categoryNew);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategoryNew", new { id = categoryNew.Id }, categoryNew);
        }

        // DELETE: api/CategoryNews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryNew([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryNew = await _context.CategoryNews.FindAsync(id);
            if (categoryNew == null)
            {
                return NotFound();
            }

            _context.CategoryNews.Remove(categoryNew);
            await _context.SaveChangesAsync();

            return Ok(categoryNew);
        }

        private bool CategoryNewExists(int? id)
        {
            return _context.CategoryNews.Any(e => e.Id == id);
        }
    }
}