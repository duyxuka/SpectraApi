using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectra.Models;

namespace Spectra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionPrizeController : ControllerBase
    {
        private readonly AppDBContext _context;

        public QuestionPrizeController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/QuestionPrize
        [HttpGet]
        public IEnumerable<QuestionPrize> GetQuestionPrizes()
        {
            return _context.QuestionPrizes.OrderByDescending(x=> x.Id);
        }

        [HttpGet]
        [Route("QuestionPrizeUser")]
        public IEnumerable<QuestionPrize> GetQuestionPrizesUser()
        {
            return _context.QuestionPrizes.Where(x => x.Status == true);
        }

        // GET: api/QuestionPrize/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionPrize([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var questionPrize = await _context.QuestionPrizes.FindAsync(id);

            if (questionPrize == null)
            {
                return NotFound();
            }

            return Ok(questionPrize);
        }

        // PUT: api/QuestionPrize/5
        [HttpPost]
        [Route("PutQuestionPrize")]
        public async Task<IActionResult> PutQuestionPrize([FromBody] QuestionPrize questionPrize)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(questionPrize).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }

            return NoContent();
        }

        // POST: api/QuestionPrize
        [HttpPost]
        public async Task<IActionResult> PostQuestionPrize([FromBody] QuestionPrize questionPrize)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            questionPrize.CreatedDate = DateTime.Now;
            _context.QuestionPrizes.Add(questionPrize);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuestionPrize", new { id = questionPrize.Id }, questionPrize);
        }

        // DELETE: api/QuestionPrize/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestionPrize([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var questionPrize = await _context.QuestionPrizes.FindAsync(id);
            if (questionPrize == null)
            {
                return NotFound();
            }

            _context.QuestionPrizes.Remove(questionPrize);
            await _context.SaveChangesAsync();

            return Ok(questionPrize);
        }

        private bool QuestionPrizeExists(int? id)
        {
            return _context.QuestionPrizes.Any(e => e.Id == id);
        }
    }
}