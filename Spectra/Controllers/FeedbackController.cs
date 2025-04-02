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
    
    public class FeedbackController : ControllerBase
    {
        private readonly AppDBContext _context;

        public FeedbackController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Feedback
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Feedback> GetFeedbacks()
        {
            var data = _context.Feedbacks.Join(_context.Products, ai => ai.ProductId,
              al => al.Id, (ai, al) => new { ai, al }).Select(x => new ProFeedbackDisplay
              {
                  Id = x.ai.Id,
                  Code = x.ai.Code,
                  Title = x.ai.Title,
                  Description = x.ai.Description,
                  Image = x.ai.Image,
                  ProductId = x.ai.ProductId,
                  Rating = x.ai.Rating,
                  Status = x.ai.Status,
                  CreatedDate = x.ai.CreatedDate,
                  ModifiedDate = x.ai.ModifiedDate,
                  ProductName = x.al.Name

              }).Where(x => x.Status == true).ToList();
            return data;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetFeedbackHome")]
        public IEnumerable<Feedback> GetFeedbackshome()
        {
            var data = _context.Feedbacks.Join(_context.Products, ai => ai.ProductId,
              al => al.Id, (ai, al) => new { ai, al }).Select(x => new ProFeedbackDisplay
              {
                  Id = x.ai.Id,
                  Code = x.ai.Code,
                  Title = x.ai.Title,
                  Description = x.ai.Description,
                  Image = x.ai.Image,
                  ProductId = x.ai.ProductId,
                  Status = x.ai.Status,
                  CreatedDate = x.ai.CreatedDate,
                  ModifiedDate = x.ai.ModifiedDate,
                  ProductName = x.al.Name

              }).Where(x => x.Status == true).ToList().Take(5);
            return data;
        }
        // GET: api/Feedback/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeedback([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feedback = await _context.Feedbacks.FindAsync(id);

            if (feedback == null)
            {
                return NotFound();
            }

            return Ok(feedback);
        }

        // PUT: api/Feedback/5
        [HttpPost]
        [Route("PutFeedback")]
        public async Task<IActionResult> PutFeedback([FromRoute] int? id, [FromBody] Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(feedback).State = EntityState.Modified;

            try
            {

                feedback.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok(feedback);
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return NoContent();
        }

        [HttpGet]
        [Route("getproductId/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory([FromRoute] int? id)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = from p in _context.Feedbacks
                         select p.Rating;
            var data = await _context.Feedbacks
        .Join(_context.Products, ai => ai.ProductId,
              al => al.Id, (ai, al) => new
              {
                  Id = ai.Id,
                  Title = ai.Title,
                  Code = ai.Code,
                  Image = ai.Image,
                  ProductId = ai.ProductId,
                  Rating = ai.Rating,
                  Description = ai.Description,
                  Status = ai.Status,
                  CreatedDate = ai.CreatedDate,
                  LinkName = ai.Title.Replace(" ", "_").ToLower(),
                  RatingAvg = result.Average()

              }).Where(x => x.ProductId == id).Select(x => new ProFeedbackDisplay
              {
                  Id = x.Id,
                  Title = x.Title,
                  Code = x.Code,
                  Image = x.Image,
                  ProductId = x.ProductId,
                  Rating = x.Rating,
                  Description = x.Description,
                  Status = x.Status,
                  CreatedDate = x.CreatedDate,
                  LinkName = x.Title.Replace(" ", "_").ToLower(),
                  RatingAvg = x.RatingAvg
              }).ToListAsync();

            return Ok(data);
        }

        [HttpGet]
        [Route("getRating/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRating([FromRoute] int? id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = from p in _context.Feedbacks
                         select p.Rating;
            var data = await _context.Feedbacks.Select(x => new ProFeedbackDisplay
            {
                ProductId = x.ProductId,
                RatingAvg = result.Average()

            }).Where(x => x.ProductId == id).FirstOrDefaultAsync();

            return Ok(data);
        }
        // POST: api/Feedback
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostFeedback([FromBody] Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Feedbacks.Add(feedback);
            feedback.CreatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFeedback", new { id = feedback.Id }, feedback);
        }

        // DELETE: api/Feedback/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return Ok(feedback);
        }

        private bool FeedbackExists(int? id)
        {
            return _context.Feedbacks.Any(e => e.Id == id);
        }
    }
}