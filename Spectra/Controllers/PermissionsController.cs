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
using Spectra.Models.Authorize;

namespace Spectra.Controllers
{
    [EnableCors("AddCors")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public PermissionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Permissions
        [HttpGet]
        [BinaryAuthorize("Roles", ActionType.Xem)]
        public IEnumerable<Permissions> GetPermissions()
        {
            return _context.Permissions;
        }

        // GET: api/Permissions/5
        [HttpGet("{id}")]
        [BinaryAuthorize("Roles", ActionType.Xem)]
        public async Task<IActionResult> GetPermissions([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var permissions = await _context.Permissions.FindAsync(id);

            if (permissions == null)
            {
                return NotFound();
            }

            return Ok(permissions);
        }

        // PUT: api/Permissions/5
        [HttpPut("{id}")]
        [BinaryAuthorize("Roles", ActionType.Sua)]
        public async Task<IActionResult> PutPermissions([FromRoute] int id, [FromBody] Permissions permissions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != permissions.Id)
            {
                return BadRequest();
            }

            _context.Entry(permissions).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PermissionsExists(id))
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

        // POST: api/Permissions
        [HttpPost]
        [BinaryAuthorize("Roles", ActionType.Them)]
        public async Task<IActionResult> PostPermissions([FromBody] Permissions permissions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Permissions.Add(permissions);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPermissions", new { id = permissions.Id }, permissions);
        }

        [HttpPost("SaveMultiple")]
        [BinaryAuthorize("Roles", ActionType.Them)]
        public async Task<IActionResult> SaveMultiplePermissions([FromBody] List<Permissions> permissionsList)
        {
            foreach (var item in permissionsList)
            {
                var existing = await _context.Permissions
                    .FirstOrDefaultAsync(x => x.RolesId == item.RolesId && x.ModulesId == item.ModulesId);

                if (existing != null)
                {
                    existing.PermissionValue = item.PermissionValue;
                    _context.Permissions.Update(existing);
                }
                else
                {
                    _context.Permissions.Add(item);
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Permissions/5
        [HttpDelete("{id}")]
        [BinaryAuthorize("Roles", ActionType.Xoa)]
        public async Task<IActionResult> DeletePermissions([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var permissions = await _context.Permissions.FindAsync(id);
            if (permissions == null)
            {
                return NotFound();
            }

            _context.Permissions.Remove(permissions);
            await _context.SaveChangesAsync();

            return Ok(permissions);
        }

        private bool PermissionsExists(int id)
        {
            return _context.Permissions.Any(e => e.Id == id);
        }
    }
}