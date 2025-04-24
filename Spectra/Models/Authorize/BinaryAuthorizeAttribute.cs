using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Spectra.Models.Authorize
{
    public class BinaryAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _module;
        private readonly int _requiredPermission;

        public BinaryAuthorizeAttribute(string module, object permission)
        {
            _module = module;
            if (permission is int intPermission)
            {
                _requiredPermission = intPermission;
            }
            else if (permission is ActionType action)
            {
                _requiredPermission = (int)action;
            }
            else
            {
                throw new ArgumentException("Permission must be an int or ActionType enum");
            }
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var permissionJson = context.HttpContext.User.FindFirst("Permissions")?.Value;

                if (string.IsNullOrEmpty(permissionJson))
                {
                    context.Result = new JsonResult(new { message = "Không có quyền truy cập (thiếu thông tin quyền)." })
                    {
                        StatusCode = 403
                    };
                    return;
                }

                var permissions = JsonConvert.DeserializeObject<Dictionary<string, int>>(permissionJson);

                if (permissions == null || !permissions.TryGetValue(_module, out int actualPermission))
                {
                    context.Result = new JsonResult(new { message = "Không có quyền truy cập module này." })
                    {
                        StatusCode = 403
                    };
                    return;
                }

                if ((_requiredPermission & actualPermission) != _requiredPermission)
                {
                    context.Result = new JsonResult(new { message = "Bạn không có quyền thực hiện hành động này." })
                    {
                        StatusCode = 403
                    };
                }
            }
            catch (JsonException)
            {
                context.Result = new JsonResult(new { message = "Lỗi định dạng dữ liệu quyền." })
                {
                    StatusCode = 403
                };
            }
            catch (Exception ex)
            {
                context.Result = new JsonResult(new { message = "Đã xảy ra lỗi trong quá trình kiểm tra quyền truy cập." })
                {
                    StatusCode = 403
                };
            }
        }
    }
}
