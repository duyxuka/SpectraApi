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
    public class RolePermissionsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public RolePermissionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/RolePermissions
        [HttpGet]
        public IEnumerable<RolePermissions> GetRolePermissions()
        {
            return _context.RolePermissions;
        }

        // GET: api/RolePermissions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRolePermissions([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rolePermissions = await _context.RolePermissions.FindAsync(id);

            if (rolePermissions == null)
            {
                return NotFound();
            }

            return Ok(rolePermissions);
        }

        // PUT: api/RolePermissions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRolePermissions([FromRoute] int id, [FromBody] RolePermissions rolePermissions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rolePermissions.Id)
            {
                return BadRequest();
            }

            _context.Entry(rolePermissions).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolePermissionsExists(id))
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

        // POST: api/RolePermissions
        [HttpPost]
        public async Task<IActionResult> PostRolePermissions([FromBody] RolePermissions rolePermissions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RolePermissions.Add(rolePermissions);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRolePermissions", new { id = rolePermissions.Id }, rolePermissions);
        }

        // DELETE: api/RolePermissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRolePermissions([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rolePermissions = await _context.RolePermissions.FindAsync(id);
            if (rolePermissions == null)
            {
                return NotFound();
            }

            _context.RolePermissions.Remove(rolePermissions);
            await _context.SaveChangesAsync();

            return Ok(rolePermissions);
        }

        private bool RolePermissionsExists(int id)
        {
            return _context.RolePermissions.Any(e => e.Id == id);
        }
    }
}