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
    
    public class CategoryController : ControllerBase
    {
        private readonly AppDBContext _context;

        public CategoryController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Category> GetCategory()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var data = _context.Category
                    .AsNoTracking() // Tối ưu hóa để không theo dõi các đối tượng
                    .Select(x => new CategoryProductDisplay
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        Status = x.Status,
                        Image = x.Image,
                        Title = x.Title,
                        TitleSeo = x.TitleSeo,
                        Option = x.Option,
                        MetaKeyWords = x.MetaKeyWords,
                        MetaDescription = x.MetaDescription,
                        Description = x.Description,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        LinkName = rgx.Replace(x.Name, "-").ToLower()
                    })
                    .Where(x => x.Status == true)
                    .ToList();

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving categories: {ex.Message}");
                // Log the exception or handle it accordingly
                return Enumerable.Empty<Category>();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("CategoryBH")]
        public IEnumerable<Category> GetCategoryBH()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var data = _context.Category
                    .AsNoTracking() // Tối ưu hóa để không theo dõi các đối tượng
                    .Where(x => x.Option == true)
                    .Select(x => new CategoryProductDisplay
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        Status = x.Status,
                        Image = x.Image,
                        Title = x.Title,
                        TitleSeo = x.TitleSeo,
                        Option = x.Option,
                        MetaKeyWords = x.MetaKeyWords,
                        MetaDescription = x.MetaDescription,
                        Description = x.Description,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        LinkName = rgx.Replace(x.Name, "-").ToLower()
                    })
                    .ToList();

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving categories: {ex.Message}");
                // Log the exception or handle it accordingly
                return Enumerable.Empty<Category>();
            }
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            try
            {
                var category = await _context.Category
                    .AsNoTracking() // Tối ưu hóa để không theo dõi các đối tượng
                    .Where(x => x.Id == id)
                    .Select(x => new CategoryProductDisplay
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        Status = x.Status,
                        Image = x.Image,
                        Title = x.Title,
                        TitleSeo = x.TitleSeo,
                        Option = x.Option,
                        MetaKeyWords = x.MetaKeyWords,
                        MetaDescription = x.MetaDescription,
                        Description = x.Description,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        LinkName = rgx.Replace(x.Name, "-").ToLower()
                    })
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    return NotFound();
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving category: {ex.Message}");
                // Log the exception or handle it accordingly
                return BadRequest("Error retrieving category");
            }
        }


        [HttpGet]
        [Route("TrashCategory")]
        public IEnumerable<Category> GetTrashCategoryProducts()
        {
            return _context.Category.Where(b => b.Status == false);
        }

        [HttpPost]
        [Route("RepeatCategory")]
        public async Task<IActionResult> RepeatCategoryProduct([FromBody] Category categoryProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(categoryProduct).State = EntityState.Modified;

            try
            {
                categoryProduct.Status = true;
                categoryProduct.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }
        [HttpPost]
        [Route("TemporaryDelete")]
        public async Task<IActionResult> TemporaryDelete([FromBody] Category categoryProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(categoryProduct).State = EntityState.Modified;

            try
            {
                categoryProduct.Status = false;
                //categoryProduct.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // PUT: api/Category/5
        [HttpPost]
        [Route("PutCategory")]
        public async Task<IActionResult> PutCategoryProduct([FromBody] Category categoryProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(categoryProduct).State = EntityState.Modified;

            try
            {
                categoryProduct.Status = true;
                categoryProduct.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> PostCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            category.CreatedDate = DateTime.Now;
            _context.Category.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryProduct = await _context.Category.FindAsync(id);
            if (categoryProduct == null)
            {
                return NotFound();
            }

            _context.Category.Remove(categoryProduct);
            await _context.SaveChangesAsync();

            return Ok(categoryProduct);
        }

        private bool CategoryExists(int? id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}