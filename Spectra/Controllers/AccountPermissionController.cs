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
    public class AccountPermissionController : ControllerBase
    {
        private readonly AppDBContext _context;

        public AccountPermissionController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/AccountPermission
        [HttpGet]
        [BinaryAuthorize("Admin", ActionType.Xem)]
        public IEnumerable<AccountPermissions> GetAccountPermissions()
        {
            return _context.AccountPermissions;
        }

        // GET: api/AccountPermission/5
        [HttpGet("{id}")]
        [BinaryAuthorize("Admin", ActionType.Xem)]
        public async Task<IActionResult> GetAccountPermissions([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accountPermissions = await _context.AccountPermissions.FindAsync(id);

            if (accountPermissions == null)
            {
                return NotFound();
            }

            return Ok(accountPermissions);
        }


        [HttpPost]
        [Route("assign-account-permission")]
        [BinaryAuthorize("Admin", ActionType.Them)]
        public async Task<IActionResult> AssignAccountPermission([FromBody] List<AccountPermissions> models)
        {
            if (models == null || !models.Any())
            {
                return BadRequest(new { Message = "Danh sách quyền không hợp lệ." });
            }

            var accountId = models[0].AccountAdminId;
            var account = await _context.AccountAdmins.FindAsync(accountId);
            if (account == null)
            {
                return NotFound(new { Message = "Tài khoản không tồn tại." });
            }

            // Lấy danh sách moduleIds từ models để xác định quyền cần giữ
            var moduleIdsToKeep = models.Select(m => m.ModulesId).ToList();

            // Xóa các quyền cũ không còn trong danh sách
            var existingPermissions = await _context.AccountPermissions
                .Where(ap => ap.AccountAdminId == accountId && !moduleIdsToKeep.Contains(ap.ModulesId))
                .ToListAsync();
            _context.AccountPermissions.RemoveRange(existingPermissions);

            foreach (var model in models)
            {
                var module = await _context.Modules.FindAsync(model.ModulesId);
                if (module == null)
                {
                    return NotFound(new { Message = $"Module với ID {model.ModulesId} không tồn tại." });
                }

                var existingPermission = await _context.AccountPermissions
                    .FirstOrDefaultAsync(ap => ap.AccountAdminId == model.AccountAdminId && ap.ModulesId == model.ModulesId);

                if (existingPermission != null)
                {
                    existingPermission.PermissionValue = model.PermissionValue;
                }
                else
                {
                    var permission = new AccountPermissions
                    {
                        AccountAdminId = model.AccountAdminId,
                        ModulesId = model.ModulesId,
                        PermissionValue = model.PermissionValue
                    };
                    _context.AccountPermissions.Add(permission);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Gán quyền thành công." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);
                return StatusCode(500, new { Message = "Lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }
        // PUT: api/AccountPermission/5
        [HttpPost]
        [Route("PutAccountPermission")]
        [BinaryAuthorize("Admin", ActionType.Sua)]
        public async Task<IActionResult> PutAccountPermissions([FromBody] AccountPermissions accountPermissions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(accountPermissions).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }

            return NoContent();
        }

        // POST: api/AccountPermission
        [HttpPost]
        [BinaryAuthorize("Admin", ActionType.Them)]
        public async Task<IActionResult> PostAccountPermissions([FromBody] AccountPermissions accountPermissions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AccountPermissions.Add(accountPermissions);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccountPermissions", new { id = accountPermissions.Id }, accountPermissions);
        }

        // DELETE: api/AccountPermission/5
        [HttpDelete("{id}")]
        [BinaryAuthorize("Admin", ActionType.Xoa)]
        public async Task<IActionResult> DeleteAccountPermissions([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accountPermissions = await _context.AccountPermissions.FindAsync(id);
            if (accountPermissions == null)
            {
                return NotFound();
            }

            _context.AccountPermissions.Remove(accountPermissions);
            await _context.SaveChangesAsync();

            return Ok(accountPermissions);
        }

        private bool AccountPermissionsExists(int id)
        {
            return _context.AccountPermissions.Any(e => e.Id == id);
        }
    }
}