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
    public class PolicyDetailController : ControllerBase
    {
        private readonly AppDBContext _context;

        public PolicyDetailController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/PolicyDetail
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<PolicyDetail> GetPolicyDetails()
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);
            var data = _context.PolicyDetails
                .AsNoTracking()
                .Join(_context.Policies, ai => ai.PolicyId, al => al.Id, (ai, al) => new
                {
                    Id = ai.Id,
                    Code = ai.Code,
                    Name = ai.Name,
                    Description = ai.Description,
                    Status = ai.Status,
                    PolicyId = ai.PolicyId,
                    CreatedDate = ai.CreatedDate,
                    ModifiedDate = ai.ModifiedDate,
                    CatePoName = al.Name,
                    LinkName = rgx.Replace(ai.Name, "-").ToLower()
                })
                .Where(x => x.Status == true)
                .Select(x => new PolicyDetailDisplay()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    Status = x.Status,
                    PolicyId = x.PolicyId,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    CatePoName = x.CatePoName,
                    LinkName = rgx.Replace(x.Name, "-").ToLower()
                })
                .ToList();

            return data;
        }


        [HttpGet]
        [Route("getCategoryId/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryId([FromRoute] int? id)
        {
            string pattern = "[ ,+(){}.*+?^$|]";
            Regex rgx = new Regex(pattern);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = await _context.PolicyDetails
                .AsNoTracking()
                .Join(_context.Policies, ai => ai.PolicyId, al => al.Id, (ai, al) => new
                {
                    Id = ai.Id,
                    Name = ai.Name,
                    Code = ai.Code,
                    PolicyId = ai.PolicyId,
                    Description = ai.Description,
                    Status = ai.Status,
                    CreatedDate = ai.CreatedDate,
                    LinkName = rgx.Replace(ai.Name, "-").ToLower()
                })
                .Where(x => x.PolicyId == id)
                .Select(x => new PolicyDetailDisplay
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    PolicyId = x.PolicyId,
                    Description = x.Description,
                    Status = x.Status,
                    CreatedDate = x.CreatedDate,
                    LinkName = rgx.Replace(x.Name, "-").ToLower()
                })
                .ToListAsync();

            return Ok(data);
        }


        // GET: api/PolicyDetail/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPolicyDetail([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var policyDetail = await _context.PolicyDetails.FindAsync(id);

            if (policyDetail == null)
            {
                return NotFound();
            }

            return Ok(policyDetail);
        }

        // PUT: api/PolicyDetail/5
        [HttpPost]
        [Route("PutPolicyDetail")]
        public async Task<IActionResult> PutPolicyDetail([FromBody] PolicyDetail policyDetail)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(policyDetail).State = EntityState.Modified;

            try
            {
                policyDetail.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                policyDetail = _context.PolicyDetails.Join(_context.Policies, ai => ai.PolicyId,
                  al => al.Id, (ai, al) => new { ai, al }).Where(x => x.ai.Id == policyDetail.Id).Select(x => new PolicyDetailDisplay
                  {
                      Id = x.ai.Id,
                      Code = x.ai.Code,
                      Name = x.ai.Name,
                      Description = x.ai.Description,
                      PolicyId = x.ai.PolicyId,
                      Status = x.ai.Status,
                      CreatedDate = x.ai.CreatedDate,
                      ModifiedDate = x.ai.ModifiedDate,
                      CatePoName = x.al.Name
                  }).FirstOrDefault();
                return Ok(policyDetail);
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/PolicyDetail
        [HttpPost]
        public async Task<IActionResult> PostPolicyDetail([FromBody] PolicyDetail policyDetail)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            policyDetail.CreatedDate = DateTime.Now;
            _context.PolicyDetails.Add(policyDetail);
            await _context.SaveChangesAsync();

            policyDetail = _context.PolicyDetails.Join(_context.Policies, ai => ai.PolicyId,
              al => al.Id, (ai, al) => new { ai, al }).Where(x => x.ai.Id == policyDetail.Id).Select(x => new PolicyDetailDisplay
              {
                  Id = x.ai.Id,
                  Code = x.ai.Code,
                  Name = x.ai.Name,
                  Description = x.ai.Description,
                  PolicyId = x.ai.PolicyId,
                  Status = x.ai.Status,
                  CreatedDate = x.ai.CreatedDate,
                  ModifiedDate = x.ai.ModifiedDate,
                  CatePoName = x.al.Name
              }).FirstOrDefault();
            return CreatedAtAction("GetPolicyDetail", new { id = policyDetail.Id }, policyDetail);
        }

        // DELETE: api/PolicyDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePolicyDetail([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var policyDetail = await _context.PolicyDetails.FindAsync(id);
            if (policyDetail == null)
            {
                return NotFound();
            }

            _context.PolicyDetails.Remove(policyDetail);
            await _context.SaveChangesAsync();

            return Ok(policyDetail);
        }

        private bool PolicyDetailExists(int? id)
        {
            return _context.PolicyDetails.Any(e => e.Id == id);
        }
    }
}