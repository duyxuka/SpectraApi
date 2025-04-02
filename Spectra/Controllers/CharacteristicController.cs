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
    public class CharacteristicController : ControllerBase
    {
        private readonly AppDBContext _context;

        public CharacteristicController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Characteristic
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Characteristic> GetCharacteristics()
        {
            return _context.Characteristics.Join(_context.Category, ai => ai.CategoryId,
              al => al.Id, (ai, al) => new
              {
                  Id = ai.Id,
                  Name = ai.Name,
                  Code = ai.Code,
                  CategoryId = ai.CategoryId,
                  Description = ai.Description,
                  Status = ai.Status,
                  CateName = al.Name,
                  LinkName = ai.Name.Replace(" ", "-").ToLower()

              }).Select(x => new CharacteristicDisplay
              {
                  Id = x.Id,
                  Name = x.Name,
                  Code = x.Code,
                  CategoryId = x.CategoryId,
                  Description = x.Description,
                  Status = x.Status,
                  CateName = x.CateName,
                  LinkName = x.Name.Replace(" ", "-").ToLower()
              }).Where(x => x.Status == true).ToList();
        }

        // GET: api/Characteristic/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCharacteristic([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var characteristic = await _context.Characteristics.FindAsync(id);

            if (characteristic == null)
            {
                return NotFound();
            }

            return Ok(characteristic);
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
            var data = await _context.Characteristics
        .Join(_context.Category, ai => ai.CategoryId,
              al => al.Id, (ai, al) => new
              {
                  Id = ai.Id,
                  Name = ai.Name,
                  Code = ai.Code,
                  CategoryId = ai.CategoryId,
                  Description = ai.Description,
                  Status = ai.Status,
                  LinkName = ai.Name.Replace(" ", "-").ToLower()

              }).Where(x => x.CategoryId == id).Select(x => new CharacteristicDisplay
              {
                  Id = x.Id,
                  Name = x.Name,
                  Code = x.Code,
                  CategoryId = x.CategoryId,
                  Description = x.Description,
                  Status = x.Status,
                  LinkName = x.Name.Replace(" ", "-").ToLower()
              }).FirstOrDefaultAsync();

            return Ok(data);
        }

        // PUT: api/Characteristic/5
        [HttpPost]
        [Route("PutCharacteristic")]
        public async Task<IActionResult> PutCharacteristic([FromBody] Characteristic characteristic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(characteristic).State = EntityState.Modified;

            try
            {
                characteristic.Status = true;
                characteristic.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Characteristic
        [HttpPost]
        public async Task<IActionResult> PostCharacteristic([FromBody] Characteristic characteristic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Characteristics.Add(characteristic);
            await _context.SaveChangesAsync();

            characteristic = _context.Characteristics.Join(_context.Category, ca => ca.CategoryId,
              ne => ne.Id, (ca, ne) => new { ca, ne }).Where(x => x.ca.Id == characteristic.Id).Select(x => new CharacteristicDisplay
              {
                  Id = x.ca.Id,
                  Code = x.ca.Code,
                  Name = x.ca.Name,
                  Description = x.ca.Description,
                  CategoryId = x.ca.CategoryId,
                  Status = x.ca.Status,
                  CreatedDate = x.ca.CreatedDate,
                  ModifiedDate = x.ca.ModifiedDate,
                  CateName = x.ne.Name
              }).FirstOrDefault();

            return CreatedAtAction("GetCharacteristic", new { id = characteristic.Id }, characteristic);
        }

        // DELETE: api/Characteristic/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacteristic([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var characteristic = await _context.Characteristics.FindAsync(id);
            if (characteristic == null)
            {
                return NotFound();
            }

            _context.Characteristics.Remove(characteristic);
            await _context.SaveChangesAsync();

            return Ok(characteristic);
        }

        private bool CharacteristicExists(int? id)
        {
            return _context.Characteristics.Any(e => e.Id == id);
        }
    }
}