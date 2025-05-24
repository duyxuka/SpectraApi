using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectra.Models;

namespace Spectra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserRoleAdminsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public UserRoleAdminsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/UserRoleAdmins
        [HttpGet]
        public IEnumerable<UserRoleAdmin> GetUserRoleAdmins()
        {
            return _context.UserRoleAdmins;
        }

        // GET: api/UserRoleAdmins/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserRoleAdmin([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userRoleAdmin = await _context.UserRoleAdmins.FindAsync(id);

            if (userRoleAdmin == null)
            {
                return NotFound();
            }

            return Ok(userRoleAdmin);
        }

        // PUT: api/UserRoleAdmins/5
        [HttpPut]
        [Route("PutUserRoleAdmin")]
        public async Task<IActionResult> PutUserRoleAdmin([FromBody] UserRoleAdminDto userRoleAdminDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Remove existing roles for the AccountAdminId
            var existingRoles = _context.UserRoleAdmins
                .Where(ura => ura.AccountAdminId == userRoleAdminDto.AccountAdminId);
            _context.UserRoleAdmins.RemoveRange(existingRoles);

            // Add updated roles
            foreach (var roleId in userRoleAdminDto.RoleIds)
            {
                var userRoleAdmin = new UserRoleAdmin
                {
                    AccountAdminId = userRoleAdminDto.AccountAdminId,
                    RolesId = roleId
                };
                _context.UserRoleAdmins.Add(userRoleAdmin);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UserRoleAdmins.Any(ura => ura.AccountAdminId == userRoleAdminDto.AccountAdminId))
                {
                    return NotFound();
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Error updating roles.", error = ex.Message });
            }

            return NoContent();
        }

        public class UserRolePermissionUpdateModel
        {
            public List<int> RoleId { get; set; }
            public List<ModulePermissionUpdateModel> ModulePermissions { get; set; }
        }
        public class ModulePermissionUpdateModel
        {
            public int ModuleId { get; set; }
            public int PermissionValue { get; set; }
        }

        // POST: api/UserRoleAdmins
        [HttpPost]
        [Route("PostUserRoleAdmin")]
        public async Task<IActionResult> PostUserRoleAdmin([FromBody] UserRoleAdminDto userRoleAdminDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Remove existing roles for the AccountAdminId to avoid duplicates
            var existingRoles = _context.UserRoleAdmins
                .Where(ura => ura.AccountAdminId == userRoleAdminDto.AccountAdminId);
            _context.UserRoleAdmins.RemoveRange(existingRoles);

            // Add new roles
            foreach (var roleId in userRoleAdminDto.RoleIds)
            {
                var userRoleAdmin = new UserRoleAdmin
                {
                    AccountAdminId = userRoleAdminDto.AccountAdminId,
                    RolesId = roleId
                };
                _context.UserRoleAdmins.Add(userRoleAdmin);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Error saving roles.", error = ex.Message });
            }

            return CreatedAtAction("GetUserRoleAdmin", new { id = userRoleAdminDto.AccountAdminId }, userRoleAdminDto);
        }

        // DELETE: api/UserRoleAdmins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserRoleAdmin([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userRoleAdmin = await _context.UserRoleAdmins.FindAsync(id);
            if (userRoleAdmin == null)
            {
                return NotFound();
            }

            _context.UserRoleAdmins.Remove(userRoleAdmin);
            await _context.SaveChangesAsync();

            return Ok(userRoleAdmin);
        }

        private bool UserRoleAdminExists(int id)
        {
            return _context.UserRoleAdmins.Any(e => e.Id == id);
        }
    }
}