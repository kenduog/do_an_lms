using Interface.Services;
using Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AppAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {

        public AppAuthorize()
        {
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (UserLoginModel)context.HttpContext.Items["User"];//.User;
            string controllerName = string.Empty;
            string actionName = string.Empty;
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                controllerName = descriptor.ControllerName;
                actionName = descriptor.ActionName;
            }
            if (user == null)
            {
                context.Result = new UnauthorizedObjectResult(new { resultMessage = "Không có quyền truy cập!" });
            }

            IPermissionService permissionService = (IPermissionService)context.HttpContext.RequestServices.GetService(typeof(IPermissionService));
            var hasPermit = false;
            if (user.isSuperUser || user.isMobile)
            {
                hasPermit = true;
            }
            else
            {
                var userCheckResult = permissionService.GetPermission(user.userId, controllerName, actionName);
                hasPermit = userCheckResult.Result.Any();
            }

            if (!hasPermit)
            {
                context.Result = new UnauthorizedObjectResult(new { resultMessage = "Không có quyền truy cập!" });
            }
        }
    }
}
