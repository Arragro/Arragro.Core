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
            return controllerContext.HttpContext.UserId();
        }

        public static Guid UserId(this HttpContext httpContext)
        {
            if (httpContext != null)
            {
                return httpContext.User.UserId();
            }

            return Guid.Empty;
        }

        public static Guid UserId(this ClaimsPrincipal principal)
        {
            if (principal != null &&
                principal.Identity != null &&
                !string.IsNullOrEmpty(principal.Identity.Name))
            {
                if (principal.Identity.AuthenticationType == "Identity.Application" ||
                    principal.Identity.AuthenticationType == "Cookies")
                {
                    return Guid.Parse(principal.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
                }
                else if (principal.Identity.AuthenticationType == "AuthenticationTypes.Federation")
                {
                    return Guid.Parse(principal.Claims.Single(x => x.Type == "User-Identifier").Value);
                }

            }

            return Guid.Empty;
        }
    }
}
