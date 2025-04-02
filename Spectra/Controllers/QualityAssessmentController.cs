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
    public class QualityAssessmentController : ControllerBase
    {
        private readonly AppDBContext _context;

        public QualityAssessmentController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/QualityAssessment
        [HttpGet]
        public IEnumerable<QualityAssessment> GetQualityAssessments()
        {
            return _context.QualityAssessments.OrderByDescending(x=> x.CreatedDate);
        }

        // GET: api/QualityAssessment/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQualityAssessment([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var qualityAssessment = await _context.QualityAssessments.FindAsync(id);

            if (qualityAssessment == null)
            {
                return NotFound();
            }

            return Ok(qualityAssessment);
        }

        // PUT: api/QualityAssessment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQualityAssessment([FromRoute] int? id, [FromBody] QualityAssessment qualityAssessment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != qualityAssessment.Id)
            {
                return BadRequest();
            }

            _context.Entry(qualityAssessment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QualityAssessmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/QualityAssessment
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostQualityAssessment([FromBody] QualityAssessment qualityAssessment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            qualityAssessment.CreatedDate = DateTime.Now;
            _context.QualityAssessments.Add(qualityAssessment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQualityAssessment", new { id = qualityAssessment.Id }, qualityAssessment);
        }

        // DELETE: api/QualityAssessment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQualityAssessment([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var qualityAssessment = await _context.QualityAssessments.FindAsync(id);
            if (qualityAssessment == null)
            {
                return NotFound();
            }

            _context.QualityAssessments.Remove(qualityAssessment);
            await _context.SaveChangesAsync();

            return Ok(qualityAssessment);
        }

        private bool QualityAssessmentExists(int? id)
        {
            return _context.QualityAssessments.Any(e => e.Id == id);
        }
    }
}