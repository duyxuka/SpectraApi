using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectra.Models;
using Spectra.Models.Authorize;

namespace Spectra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class RolesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public RolesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        //[BinaryAuthorize("Role", ActionType.Xem)]
        public IEnumerable<Roles> GetRoles()
        {
            return _context.Roles;
        }

        [HttpGet]
        [Route("GetAdminRoles")]
        //[BinaryAuthorize("Role", ActionType.Xem)]
        public async Task<IActionResult> GetAdminRoles()
        {
            var adminRoles = await _context.Roles
                .Where(r => r.RoleType == "Admin") // Lọc các vai trò có RoleType là "Admin"
                .Select(r => new { r.Id, r.Name }) // Chỉ lấy Id và Name để trả về
                .ToListAsync();

            return Ok(adminRoles);
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        //[BinaryAuthorize("Role", ActionType.Xem)]
        public async Task<IActionResult> GetRoles([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roles = await _context.Roles.FindAsync(id);

            if (roles == null)
            {
                return NotFound();
            }

            return Ok(roles);
        }

        // PUT: api/Roles/5
        [HttpPut("{id}")]
        //[BinaryAuthorize("Role", ActionType.Sua)]
        public async Task<IActionResult> PutRoles([FromRoute] int id, [FromBody] Roles roles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != roles.Id)
            {
                return BadRequest();
            }

            _context.Entry(roles).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolesExists(id))
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

        // POST: api/Roles
        [HttpPost]
       //[BinaryAuthorize("Role", ActionType.Them)]
        public async Task<IActionResult> PostRoles([FromBody] Roles roles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Roles.Add(roles);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoles", new { id = roles.Id }, roles);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        //[BinaryAuthorize("Role", ActionType.Xoa)]
        public async Task<IActionResult> DeleteRoles([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roles = await _context.Roles.FindAsync(id);
            if (roles == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(roles);
            await _context.SaveChangesAsync();

            return Ok(roles);
        }

        private bool RolesExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}