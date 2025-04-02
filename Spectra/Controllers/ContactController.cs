using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
    public class ContactController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ContactController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Contact
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Contact> GetContacts()
        {
            return _context.Contacts;
        }

        // GET: api/Contact/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetContact([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        // PUT: api/Contact/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact([FromRoute] int? id, [FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contact.Id)
            {
                return BadRequest();
            }

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
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

        // POST: api/Contact
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostContact([FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var random = new Random();
            contact.Code = random.Next(100000, 999999);
            contact.CreatedDate = DateTime.Now;
            contact.Status = true;
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContact", new { id = contact.Id }, contact);
        }
        [HttpPost]
        [Route("SendEmail")]
        [AllowAnonymous]
        public ActionResult SendEmail([FromBody] string email)
        {
            var date = DateTime.Now;
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("mayhutsuaspectra@gmail.com", "Spectra");
                    var receiverEmail = new MailAddress(email, "Receiver");
                    var password = "mieopkmqngqmotfk";
                    var subject = "Spectra Viet Nam - Cảm ơn bạn đã liên hệ với chúng tôi";
                    string body = "<div style='text-align: center;font-family: unset; font-size: 15px;'>"
                        + "<p style='text-align: center;'><img style='margin-left: 45px;' src='https://spectrababy.com.vn/assets/images/logo/logo_black_1x.png'></p>"
                        + "<h2 style='color: #10cb04;font-size: 19px;'>Bạn hãy phản hồi thắc mắc yêu cầu của bạn ngay sau mail này.</h2>"
                        + "<p>Bộ phận liên hệ của chúng tôi sẽ liên lạc với bạn.</p>"
                        + "<h4>Mọi thắc mắc xin liên hệ:</h4>"
                        + "<p>SĐT: 0916001923.</p>"
                        + "<a href='https://www.facebook.com/spectra.vietnam'>Facebook: Spectra Baby</a>"
                        + "<h3>Cảm ơn quý khách !</h3></div>";

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };

                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    })
                    {
                        smtp.Send(mess);
                    }
                }
                return NoContent();
            }
            catch (Exception)
            {

            }
            return NoContent();
        }
        // DELETE: api/Contact/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok(contact);
        }

        private bool ContactExists(int? id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}