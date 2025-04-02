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
    public class QuestionController : ControllerBase
    {
        private readonly AppDBContext _context;

        public QuestionController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Question
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Question> GetQuestions()
        {
            return _context.Questions.Join(_context.Products, ai => ai.ProductId,
              al => al.Id, (ai, al) => new
              {
                  Id = ai.Id,
                  Title = ai.Title,
                  Code = ai.Code,
                  ProductId = ai.ProductId,
                  Description = ai.Description,
                  Status = ai.Status,
                  ProductName = al.Name,
                  LinkName = ai.Title.Replace(" ", "-").ToLower()

              }).Select(x => new QuestionModels
              {
                  Id = x.Id,
                  Title = x.Title,
                  Code = x.Code,
                  ProductId = x.ProductId,
                  Description = x.Description,
                  Status = x.Status,
                  ProductName = x.ProductName,
                  LinkName = x.Title.Replace(" ", "-").ToLower()
              }).Where(x => x.Status == true).ToList();
        }

        [HttpGet]
        [Route("getquestionpro/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestionPro([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = await _context.Questions
        .Join(_context.Products, ai => ai.ProductId,
              al => al.Id, (ai, al) => new
              {
                  Id = ai.Id,
                  Title = ai.Title,
                  Code = ai.Code,
                  ProductId = ai.ProductId,
                  Description = ai.Description,
                  Status = ai.Status,
                  LinkName = ai.Title.Replace(" ", "-").ToLower()

              }).Where(x => x.ProductId == id).Select(x => new QuestionModels
              {
                  Id = x.Id,
                  Title = x.Title,
                  Code = x.Code,
                  ProductId = x.ProductId,
                  Description = x.Description,
                  Status = x.Status,
                  LinkName = x.Title.Replace(" ", "-").ToLower()
              }).ToListAsync();

            return Ok(data);
        }

        // GET: api/Question/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestion([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var question = await _context.Questions.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }

        // PUT: api/Question/5
        [HttpPost]
        [Route("PutQuestion")]
        public async Task<IActionResult> PutQuestion([FromBody] Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(question).State = EntityState.Modified;

            try
            {
                question.Status = true;
                question.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        // POST: api/Question
        [HttpPost]
        public async Task<IActionResult> PostQuestion([FromBody] Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuestion", new { id = question.Id }, question);
        }

        // DELETE: api/Question/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok(question);
        }

        private bool QuestionExists(int? id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}