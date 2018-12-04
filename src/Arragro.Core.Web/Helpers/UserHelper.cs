using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace Arragro.Core.Web.Helpers
{
    public static class UserHelper
    {
        public static Guid UserId(this ControllerContext controllerContext)
        {
            if (controllerContext.HttpContext != null &&
                controllerContext.HttpContext.User != null &&
                controllerContext.HttpContext.User.Identity != null &&
                !string.IsNullOrEmpty(controllerContext.HttpContext.User.Identity.Name))
            {
                if (controllerContext.HttpContext.User.Identity.AuthenticationType == "Identity.Application" ||
                    controllerContext.HttpContext.User.Identity.AuthenticationType == "Cookies")
                {
                    return Guid.Parse(controllerContext.HttpContext.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
                }
                else if (controllerContext.HttpContext.User.Identity.AuthenticationType == "AuthenticationTypes.Federation")
                {
                    return Guid.Parse(controllerContext.HttpContext.User.Claims.Single(x => x.Type == "User-Identifier").Value);
                }
            }

            return Guid.Empty;
        }

        public static Guid UserId(this HttpContext httpContext)
        {
            if (httpContext != null &&
                httpContext.User != null &&
                httpContext.User.Identity != null &&
                !string.IsNullOrEmpty(httpContext.User.Identity.Name))
            {
                return Guid.Parse(httpContext.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
            }

            return Guid.Empty;
        }
    }
}
