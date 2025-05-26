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
    public class RolesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public RolesController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [BinaryAuthorize("Roles", ActionType.Xem)]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                // Kiểm tra bảng Roles và Permissions đã được khởi tạo chưa
                if (_context.Roles == null || _context.Permissions == null)
                {
                    return StatusCode(500, "Roles or Permissions table is not initialized in DbContext.");
                }

                // Lấy danh sách roles kèm permissions (nếu có)
                var roles = await _context.Roles
                    .GroupJoin(
                        _context.Permissions,
                        role => role.Id,
                        perm => perm.RolesId,
                        (role, perms) => new
                        {
                            Id = role.Id,
                            Name = role.Name,
                            RoleType = role.RoleType,
                            Permissions = perms.Select(p => new
                            {
                                RolesId = p.RolesId,
                                ModulesId = p.ModulesId,
                                PermissionValue = p.PermissionValue
                            }).ToList()
                        }
                    ).ToListAsync();

                if (roles == null || !roles.Any())
                {
                    Console.WriteLine("No roles found.");
                    return NotFound("No roles found.");
                }

                Console.WriteLine($"Found {roles.Count} roles.");
                return Ok(roles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRoles: {ex.Message}\nStackTrace: {ex.StackTrace}\nInnerException: {ex.InnerException?.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("GetAdminRoles")]
        [BinaryAuthorize("Roles", ActionType.Xem)]
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
        [BinaryAuthorize("Roles", ActionType.Xem)]
        public async Task<IActionResult> GetRoles([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Truy vấn role với permissions bằng JOIN
            var roleQuery = from role in _context.Roles
                            where role.Id == id
                            join permission in _context.Permissions on role.Id equals permission.RolesId into rolePermissions
                            from permission in rolePermissions.DefaultIfEmpty() // LEFT JOIN để lấy role ngay cả khi không có permissions
                            select new
                            {
                                role.Id,
                                role.Name,
                                role.RoleType,
                                Permissions = rolePermissions.Select(p => new
                                {
                                    p.RolesId,
                                    p.ModulesId,
                                    p.PermissionValue
                                }).ToList()
                            };

            var roleData = await roleQuery.FirstOrDefaultAsync();

            if (roleData == null)
            {
                Console.WriteLine($"Role with ID {id} not found.");
                return NotFound();
            }

            // Format dữ liệu để khớp với định dạng mong đợi của frontend
            var result = new
            {
                roleData.Id,
                roleData.Name,
                roleData.RoleType,
                Permissions = roleData.Permissions.Where(p => p != null).Select(p => new
                {
                    rolesId = p.RolesId,
                    modulesId = p.ModulesId,
                    permissionValue = p.PermissionValue
                }).ToList()
            };

            Console.WriteLine($"Role found: ID={roleData.Id}, Name={roleData.Name}, PermissionsCount={roleData.Permissions.Count(p => p != null)}");
            return Ok(result);
        }

        // PUT: api/Roles/5
        [HttpPost]
        [Route("Update")]
        [BinaryAuthorize("Roles", ActionType.Sua)]
        public async Task<IActionResult> PutRoles([FromBody] RoleUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate the role ID from the body
            if (model.Id <= 0)
            {
                return BadRequest("Invalid role ID.");
            }

            var role = await _context.Roles
                .Include(r => r.Permissions) // Ensure permissions are loaded
                .FirstOrDefaultAsync(r => r.Id == model.Id);

            if (role == null)
            {
                return NotFound("Role not found.");
            }

            // Update role details
            role.Name = model.Name;
            role.RoleType = model.RoleType;

            // Nếu có permissions được gửi lên → mới cập nhật lại quyền
            if (model.Permissions != null)
            {
                // Xóa toàn bộ quyền cũ
                _context.Permissions.RemoveRange(role.Permissions);
                await _context.SaveChangesAsync();

                // Thêm mới quyền (nếu có)
                if (model.Permissions.Any())
                {
                    var newPermissions = model.Permissions.Select(p => new Permissions
                    {
                        RolesId = role.Id,
                        ModulesId = p.ModuleId,
                        PermissionValue = p.PermissionValue
                    }).ToList();

                    _context.Permissions.AddRange(newPermissions);
                    await _context.SaveChangesAsync();
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Log the concurrency issue
                Console.WriteLine($"Concurrency error updating role ID {model.Id}: {ex.Message}");

                if (!RolesExists(model.Id))
                {
                    return NotFound("Role no longer exists.");
                }
                else
                {
                    return Conflict("Concurrency conflict occurred. Please reload and try again.");
                }
            }
            catch (Exception ex)
            {
                // Log any other unexpected errors
                Console.WriteLine($"Error updating role ID {model.Id}: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the role.");
            }

            return NoContent();
        }

        // POST: api/Roles
        [HttpPost]
        [BinaryAuthorize("Roles", ActionType.Them)] 
        [AllowAnonymous]
        public async Task<IActionResult> PostRoles([FromBody] RoleCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Tạo mới role
            var role = new Roles
            {
                Name = model.Name,
                RoleType = model.RoleType
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            // Lưu permissions (nếu có)
            if (model.Permissions != null && model.Permissions.Any())
            {
                foreach (var permission in model.Permissions)
                {
                    var permissionEntity = new Permissions
                    {
                        RolesId = role.Id,
                        ModulesId = permission.ModuleId,
                        PermissionValue = permission.PermissionValue
                    };
                    _context.Permissions.Add(permissionEntity);
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetRoles", new { id = role.Id }, role);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        [BinaryAuthorize("Roles", ActionType.Xoa)]
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