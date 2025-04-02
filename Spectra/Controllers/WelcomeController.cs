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
    public class WelcomeController : ControllerBase
    {
        private readonly AppDBContext _context;

        public WelcomeController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Welcome
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Welcome> GetWelcomes()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);

            var data = _context.Welcomes.Select(x => new WelcomeDisplay
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Status = x.Status,
                TitleSeo = x.TitleSeo,
                MetaKeyWords = x.MetaKeyWords,
                MetaDescription = x.MetaDescription,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                LinkName = rgx.Replace(x.Name , "-").ToLower()

            }).Where(x => x.Status == true).ToList();

            return data;
        }

        // GET: api/Welcome/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWelcome([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);
            var welcome = await _context.Welcomes.Select(x => new WelcomeDisplay
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Status = x.Status,
                TitleSeo = x.TitleSeo,
                MetaKeyWords = x.MetaKeyWords,
                MetaDescription = x.MetaDescription,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                LinkName = rgx.Replace(x.Name, "-").ToLower()

            }).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (welcome == null)
            {
                return NotFound();
            }

            return Ok(welcome);
        }

        // PUT: api/Welcome/5
        [HttpPost]
        [Route("PutWelcome")]
        public async Task<IActionResult> PutWelcome( [FromBody] Welcome welcome)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(welcome).State = EntityState.Modified;

            try
            {
                welcome.Status = true;
                welcome.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Welcome
        [HttpPost]
        public async Task<IActionResult> PostWelcome([FromBody] Welcome welcome)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Welcomes.Add(welcome);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWelcome", new { id = welcome.Id }, welcome);
        }

        // DELETE: api/Welcome/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWelcome([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var welcome = await _context.Welcomes.FindAsync(id);
            if (welcome == null)
            {
                return NotFound();
            }

            _context.Welcomes.Remove(welcome);
            await _context.SaveChangesAsync();

            return Ok(welcome);
        }

        private bool WelcomeExists(int? id)
        {
            return _context.Welcomes.Any(e => e.Id == id);
        }
    }
}