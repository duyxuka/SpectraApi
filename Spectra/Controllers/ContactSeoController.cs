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
    public class ContactSeoController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ContactSeoController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/ContactSeo
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<ContactSeo> GetContactSeos()
        {
            return _context.ContactSeos;
        }
        [HttpGet]
        [Route("ContactUser")]
        [AllowAnonymous]
        public async Task<IActionResult> GetContactsUser()
        {
            var data = await _context.ContactSeos.Select(x => new ContactSeoDisplay
            {
                Id = x.Id,
                TitleSeo = x.TitleSeo,
                MetaKeyWords = x.MetaKeyWords,
                MetaDescription = x.MetaDescription,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
            }).FirstOrDefaultAsync();
            return Ok(data);
        }
        // GET: api/ContactSeo/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetContactSeo([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactSeo = await _context.ContactSeos.FindAsync(id);

            if (contactSeo == null)
            {
                return NotFound();
            }

            return Ok(contactSeo);
        }

        // PUT: api/ContactSeo/5
        [HttpPost]
        [Route("PutContactSeo")]
        public async Task<IActionResult> PutContactSeo([FromBody] ContactSeo contactSeo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(contactSeo).State = EntityState.Modified;

            try
            {
                contactSeo.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/ContactSeo
        [HttpPost]
        public async Task<IActionResult> PostContactSeo([FromBody] ContactSeo contactSeo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ContactSeos.Add(contactSeo);
            contactSeo.CreatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContactSeo", new { id = contactSeo.Id }, contactSeo);
        }

        // DELETE: api/ContactSeo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactSeo([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactSeo = await _context.ContactSeos.FindAsync(id);
            if (contactSeo == null)
            {
                return NotFound();
            }

            _context.ContactSeos.Remove(contactSeo);
            await _context.SaveChangesAsync();

            return Ok(contactSeo);
        }

        private bool ContactSeoExists(int? id)
        {
            return _context.ContactSeos.Any(e => e.Id == id);
        }
    }
}