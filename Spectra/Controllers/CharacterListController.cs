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
    public class CharacterListController : ControllerBase
    {
        private readonly AppDBContext _context;

        public CharacterListController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/CharacterList
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<CharacterList> GetCharacterLists()
        {
            return _context.CharacterLists.Join(_context.Characteristics, ai => ai.CharacteristicId,
              al => al.Id, (ai, al) => new
              {
                  Id = ai.Id,
                  Name = ai.Name,
                  Code = ai.Code,
                  CharacteristicId = ai.CharacteristicId,
                  Description = ai.Description,
                  Image = ai.Image,
                  Status = ai.Status,
                  CateName = al.Name,
                  LinkName = ai.Name.Replace(" ", "-").ToLower()

              }).Select(x => new CharacterListDisplay
              {
                  Id = x.Id,
                  Name = x.Name,
                  Code = x.Code,
                  CharacteristicId = x.CharacteristicId,
                  Description = x.Description,
                  Status = x.Status,
                  Image = x.Image,
                  CateName = x.CateName,
                  LinkName = x.Name.Replace(" ", "-").ToLower()
              }).Where(x => x.Status == true).ToList();
        }
        [HttpGet]
        [Route("getcharacterId/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetChacterId([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = await _context.CharacterLists
        .Join(_context.Characteristics, ai => ai.CharacteristicId,
              al => al.Id, (ai, al) => new
              {
                  Id = ai.Id,
                  Name = ai.Name,
                  Code = ai.Code,
                  Image = ai.Image,
                  CharacteristicId = ai.CharacteristicId,
                  Description = ai.Description,
                  Status = ai.Status,
                  LinkName = ai.Name.Replace(" ", "-").ToLower()

              }).Where(x => x.CharacteristicId == id).Select(x => new CharacterListDisplay
              {
                  Id = x.Id,
                  Name = x.Name,
                  Code = x.Code,
                  Image = x.Image,
                  CharacteristicId = x.CharacteristicId,
                  Description = x.Description,
                  Status = x.Status,
                  LinkName = x.Name.Replace(" ", "-").ToLower()
              }).ToListAsync();

            return Ok(data);
        }
        // GET: api/CharacterList/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCharacterList([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var characterList = await _context.CharacterLists.FindAsync(id);

            if (characterList == null)
            {
                return NotFound();
            }

            return Ok(characterList);
        }

        // PUT: api/CharacterList/5
        [HttpPost]
        [Route("PutCharacterList")]
        public async Task<IActionResult> PutCharacterList([FromBody] CharacterList characterList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(characterList).State = EntityState.Modified;

            try
            {
                characterList.Status = true;
                characterList.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/CharacterList
        [HttpPost]
        public async Task<IActionResult> PostCharacterList([FromBody] CharacterList characterList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CharacterLists.Add(characterList);
            await _context.SaveChangesAsync();

            characterList = _context.CharacterLists.Join(_context.Characteristics, ca => ca.CharacteristicId,
              ne => ne.Id, (ca, ne) => new { ca, ne }).Where(x => x.ca.Id == characterList.Id).Select(x => new CharacterListDisplay
              {
                  Id = x.ca.Id,
                  Code = x.ca.Code,
                  Name = x.ca.Name,
                  Description = x.ca.Description,
                  CharacteristicId = x.ca.CharacteristicId,
                  Status = x.ca.Status,
                  CreatedDate = x.ca.CreatedDate,
                  ModifiedDate = x.ca.ModifiedDate,
                  CateName = x.ne.Name
              }).FirstOrDefault();

            return CreatedAtAction("GetCharacterList", new { id = characterList.Id }, characterList);
        }

        // DELETE: api/CharacterList/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacterList([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var characterList = await _context.CharacterLists.FindAsync(id);
            if (characterList == null)
            {
                return NotFound();
            }

            _context.CharacterLists.Remove(characterList);
            await _context.SaveChangesAsync();

            return Ok(characterList);
        }

        private bool CharacterListExists(int? id)
        {
            return _context.CharacterLists.Any(e => e.Id == id);
        }
    }
}