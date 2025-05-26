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
using Spectra.Models.Authorize;

namespace Spectra.Controllers
{
    [EnableCors("AddCors")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecruitmentSeoController : ControllerBase
    {
        private readonly AppDBContext _context;

        public RecruitmentSeoController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/RecruitmentSeo
        [HttpGet]
        [BinaryAuthorize("SEOPage", ActionType.Xem)]
        public IEnumerable<RecruitmentSeo> GetRecruitmentSeos()
        {
            return _context.RecruitmentSeos.AsNoTracking().OrderByDescending(x => x.Id).ToList(); 
        }
        [HttpGet]
        [Route("RecruitmentUser")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecruitmentUser()
        {
            var data = await _context.RecruitmentSeos
             .Where(x => x.Status == true)
            .Select(x => new RecruitmentSeoDisplay
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
        // GET: api/RecruitmentSeo/5
        [HttpGet("{id}")]
        [BinaryAuthorize("SEOPage", ActionType.Xem)]
        public async Task<IActionResult> GetRecruitmentSeo([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recruitmentSeo = await _context.RecruitmentSeos.FindAsync(id);

            if (recruitmentSeo == null)
            {
                return NotFound();
            }

            return Ok(recruitmentSeo);
        }

        // PUT: api/RecruitmentSeo/5
        [HttpPost]
        [Route("PutRecruitment")]
        [BinaryAuthorize("SEOPage", ActionType.Sua)]
        public async Task<IActionResult> PutRecruitmentSeo([FromBody] RecruitmentSeo recruitmentSeo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(recruitmentSeo).State = EntityState.Modified;

            try
            {
                recruitmentSeo.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/RecruitmentSeo
        [HttpPost]
        [BinaryAuthorize("SEOPage", ActionType.Them)]
        public async Task<IActionResult> PostRecruitmentSeo([FromBody] RecruitmentSeo recruitmentSeo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            _context.RecruitmentSeos.Add(recruitmentSeo);
            recruitmentSeo.Status = false;
            recruitmentSeo.CreatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecruitmentSeo", new { id = recruitmentSeo.Id }, recruitmentSeo);
        }

        // DELETE: api/RecruitmentSeo/5
        [HttpDelete("{id}")]
        [BinaryAuthorize("SEOPage", ActionType.Xoa)]
        public async Task<IActionResult> DeleteRecruitmentSeo([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recruitmentSeo = await _context.RecruitmentSeos.FindAsync(id);
            if (recruitmentSeo == null)
            {
                return NotFound();
            }

            _context.RecruitmentSeos.Remove(recruitmentSeo);
            await _context.SaveChangesAsync();

            return Ok(recruitmentSeo);
        }

        private bool RecruitmentSeoExists(int? id)
        {
            return _context.RecruitmentSeos.Any(e => e.Id == id);
        }
    }
}