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
    public class QuestionServiceController : ControllerBase
    {
        private readonly AppDBContext _context;

        public QuestionServiceController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/QuestionService
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<QuestionService> GetQuestionServices()
        {
            return _context.QuestionServices.Join(_context.ServiceDetails, ai => ai.ServiceDetailId,
              al => al.Id, (ai, al) => new
              {
                  Id = ai.Id,
                  Title = ai.Title,
                  Code = ai.Code,
                  ServiceDetailId = ai.ServiceDetailId,
                  Description = ai.Description,
                  Status = ai.Status,
                  ServiceName = al.Name,
                  LinkName = ai.Title.Replace(" ", "-").ToLower()

              }).Select(x => new QuestionDisplay
              {
                  Id = x.Id,
                  Title = x.Title,
                  Code = x.Code,
                  ServiceDetailId = x.ServiceDetailId,
                  Description = x.Description,
                  Status = x.Status,
                  ServiceName = x.ServiceName,
                  LinkName = x.Title.Replace(" ", "-").ToLower()
              }).Where(x => x.Status == true).ToList();
        }

        [HttpGet]
        [Route("getquestionService/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetquestionService([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = await _context.QuestionServices
        .Join(_context.ServiceDetails, ai => ai.ServiceDetailId,
              al => al.Id, (ai, al) => new
              {
                  Id = ai.Id,
                  Title = ai.Title,
                  Code = ai.Code,
                  ServiceDetailId = ai.ServiceDetailId,
                  Description = ai.Description,
                  Status = ai.Status,
                  LinkName = ai.Title.Replace(" ", "-").ToLower()

              }).Where(x => x.ServiceDetailId == id).Select(x => new QuestionDisplay
              {
                  Id = x.Id,
                  Title = x.Title,
                  Code = x.Code,
                  ServiceDetailId = x.ServiceDetailId,
                  Description = x.Description,
                  Status = x.Status,
                  LinkName = x.Title.Replace(" ", "-").ToLower()
              }).ToListAsync();

            return Ok(data);
        }

        // GET: api/QuestionService/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestionService([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var questionService = await _context.QuestionServices.FindAsync(id);

            if (questionService == null)
            {
                return NotFound();
            }

            return Ok(questionService);
        }

        // PUT: api/QuestionService/5
        [HttpPost]
        [Route("PutQuestionService")]
        public async Task<IActionResult> PutQuestionService([FromBody] QuestionService questionService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(questionService).State = EntityState.Modified;

            try
            {
                questionService.Status = true;
                questionService.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/QuestionService
        [HttpPost]
        public async Task<IActionResult> PostQuestionService([FromBody] QuestionService questionService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.QuestionServices.Add(questionService);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuestionService", new { id = questionService.Id }, questionService);
        }

        // DELETE: api/QuestionService/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestionService([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var questionService = await _context.QuestionServices.FindAsync(id);
            if (questionService == null)
            {
                return NotFound();
            }

            _context.QuestionServices.Remove(questionService);
            await _context.SaveChangesAsync();

            return Ok(questionService);
        }

        private bool QuestionServiceExists(int? id)
        {
            return _context.QuestionServices.Any(e => e.Id == id);
        }
    }
}