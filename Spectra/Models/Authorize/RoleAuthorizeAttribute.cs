using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Spectra.Models.Authorize
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Lấy claim UserId và UserType
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            var userTypeClaim = user.FindFirst("UserType")?.Value;

            if (userIdClaim == null || string.IsNullOrEmpty(userTypeClaim) || userTypeClaim != "Admin")
            {
                context.Result = new ForbidResult();
                return;
            }

            // Ép kiểu an toàn
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Lấy DbContext từ DI
            var db = context.HttpContext.RequestServices.GetService<AppDBContext>();
            if (db == null)
            {
                context.Result = new StatusCodeResult(500); // Server error nếu không lấy được DbContext
                return;
            }

            // Lấy danh sách tên quyền (roles) của admin hiện tại
            var userRoles = db.UserRoles
                              .Where(r => r.UserId == userId && r.UserType == UserTypeEnum.Admin)
                              .Select(r => r.Roles.Name)
                              .ToList();

            // Nếu user không có role nào phù hợp
            if (!_roles.Any(role => userRoles.Contains(role)))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
