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
    public class PolicyController : ControllerBase
    {
        private readonly AppDBContext _context;

        public PolicyController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Policies
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Policy> GetPolicies()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);
            var data = _context.Policies.Select(x => new PolicyDisplay
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

            }).Where(x => x.Status == true).ToList();

            return data;
        }

        // GET: api/Policies/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPolicy([FromRoute] int? id)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var policy = await _context.Policies.Select(x => new PolicyDisplay {
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

            if (policy == null)
            {
                return NotFound();
            }

            return Ok(policy);
        }

        // PUT: api/Policies/5
        [HttpPost]
        [Route("PutPolicy")]
        public async Task<IActionResult> PutPolicy([FromBody] Policy policy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(policy).State = EntityState.Modified;

            try
            {
                policy.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Policies
        [HttpPost]
        public async Task<IActionResult> PostPolicy([FromBody] Policy policy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            policy.CreatedDate = DateTime.Now;
            _context.Policies.Add(policy);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPolicy", new { id = policy.Id }, policy);
        }

        // DELETE: api/Policies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePolicy([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var policy = await _context.Policies.FindAsync(id);
            if (policy == null)
            {
                return NotFound();
            }

            _context.Policies.Remove(policy);
            await _context.SaveChangesAsync();

            return Ok(policy);
        }

        private bool PolicyExists(int? id)
        {
            return _context.Policies.Any(e => e.Id == id);
        }
    }
}