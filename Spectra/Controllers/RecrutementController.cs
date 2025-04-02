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
    public class RecrutementController : ControllerBase
    {
        private readonly AppDBContext _context;

        public RecrutementController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Recrutement
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Recrutement> GetRecrutements()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);
            var data = _context.Recrutements.Select(x => new RecrutementDisplay
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Status = x.Status,
                Image = x.Image,
                TitleSeo = x.TitleSeo,
                MetaKeyWords = x.MetaKeyWords,
                MetaDescription = x.MetaDescription,
                Description = x.Description,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                LinkName = rgx.Replace(x.Name , "-").ToLower()

            }).Where(x => x.Status == true).ToList();

            return data;
        }

        // GET: api/Recrutement/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecrutement([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);
            var recrutement = await _context.Recrutements.Select(x => new RecrutementDisplay
              {
                  Id = x.Id,
                  Code = x.Code,
                  Name = x.Name,
                  Description = x.Description,
                  Status = x.Status,
                  Image = x.Image,
                  TitleSeo = x.TitleSeo,
                  MetaKeyWords = x.MetaKeyWords,
                  MetaDescription = x.MetaDescription,
                  CreatedDate = x.CreatedDate,
                  ModifiedDate = x.ModifiedDate,
                  LinkName = rgx.Replace(x.Name , "-").ToLower()
              }).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (recrutement == null)
            {
                return NotFound();
            }

            return Ok(recrutement);
        }

        // PUT: api/Recrutement/5
        [HttpPost]
        [Route("PutRecrutement")]
        public async Task<IActionResult> PutRecrutement([FromBody] Recrutement recrutement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(recrutement).State = EntityState.Modified;

            try
            {
                recrutement.Status = true;
                recrutement.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Recrutement
        [HttpPost]
        public async Task<IActionResult> PostRecrutement([FromBody] Recrutement recrutement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            recrutement.CreatedDate = DateTime.Now;
            _context.Recrutements.Add(recrutement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecrutement", new { id = recrutement.Id }, recrutement);
        }

        // DELETE: api/Recrutement/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecrutement([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recrutement = await _context.Recrutements.FindAsync(id);
            if (recrutement == null)
            {
                return NotFound();
            }

            _context.Recrutements.Remove(recrutement);
            await _context.SaveChangesAsync();

            return Ok(recrutement);
        }

        private bool RecrutementExists(int? id)
        {
            return _context.Recrutements.Any(e => e.Id == id);
        }
    }
}