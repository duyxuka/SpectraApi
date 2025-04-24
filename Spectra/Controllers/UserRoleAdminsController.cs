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
    [Authorize]
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserRoleAdmin([FromRoute] int id, [FromBody] UserRoleAdmin userRoleAdmin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userRoleAdmin.Id)
            {
                return BadRequest();
            }

            _context.Entry(userRoleAdmin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRoleAdminExists(id))
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

        [HttpPut]
        [Route("UpdateUserRolePermissions/{accountAdminId}")]
        public async Task<IActionResult> UpdateUserRolePermissions(int accountAdminId, [FromBody] UserRolePermissionUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Xóa các vai trò hiện tại của tài khoản
            var existingUserRoles = await _context.UserRoleAdmins
                .Where(ura => ura.AccountAdminId == accountAdminId)
                .ToListAsync();
            _context.UserRoleAdmins.RemoveRange(existingUserRoles);

            // 2. Thêm các vai trò mới cho tài khoản
            foreach (var roleId in model.RoleId)
            {
                _context.UserRoleAdmins.Add(new UserRoleAdmin
                {
                    AccountAdminId = accountAdminId,
                    RolesId = roleId
                });
            }

            // 3. Cập nhật quyền cho các vai trò (nếu cần)
            // Lưu ý: Theo schema, quyền được gán cho vai trò, không phải tài khoản.
            // Nếu cần cập nhật quyền, bạn nên có một API riêng để quản lý bảng Permissions.
            // Ở đây, chúng ta chỉ cập nhật mối quan hệ giữa AccountAdmin và Roles.
            // Tuy nhiên, nếu frontend gửi ModulePermissions, chúng ta có thể cập nhật Permissions cho các vai trò.

            foreach (var roleId in model.RoleId)
            {
                // Xóa các quyền cũ của vai trò này
                var existingPermissions = await _context.Permissions
                    .Where(p => p.RolesId == roleId)
                    .ToListAsync();
                _context.Permissions.RemoveRange(existingPermissions);

                // Thêm các quyền mới
                foreach (var permission in model.ModulePermissions)
                {
                    if (permission.PermissionValue > 0) // Chỉ thêm nếu có quyền
                    {
                        _context.Permissions.Add(new Permissions
                        {
                            RolesId = roleId,
                            ModulesId = permission.ModuleId,
                            PermissionValue = permission.PermissionValue
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Vai trò và quyền của người dùng đã được cập nhật thành công." });
        }

        // POST: api/UserRoleAdmins
        [HttpPost]
        public async Task<IActionResult> PostUserRoleAdmin([FromBody] UserRoleAdmin userRoleAdmin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UserRoleAdmins.Add(userRoleAdmin);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserRoleAdmin", new { id = userRoleAdmin.Id }, userRoleAdmin);
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