using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace Arragro.Core.Identity
{
    public static class UserClaimHelper
    {
        public static Guid UserId(this HttpContext httpContext)
        {
            if (httpContext != null)
            {
                return UserId(httpContext.User);
            }

            return Guid.Empty;
        }

        public static string UserEmailAddress(this HttpContext httpContext)
        {
            if (httpContext != null)
            {
                return UserEmailAddress(httpContext.User);
            }

            return string.Empty;
        }

        public static string UserName(this HttpContext httpContext)
        {
            if (httpContext != null)
            {
                return UserName(httpContext);
            }

            return string.Empty;
        }

        public static string UserModifiedDate(this HttpContext httpContext)
        {
            if (httpContext != null)
            {
                return UserModifiedDate(httpContext);
            }

            return string.Empty;
        }

        public static bool? UserIsExternalManaged(this HttpContext httpContext)
        {
            if (httpContext != null)
            {
                return UserIsExternalManaged(httpContext);
            }

            return null;
        }
        public static Guid UserId(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal != null &&
                claimsPrincipal.Identity != null &&
                !string.IsNullOrEmpty(claimsPrincipal.Identity.Name))
            {
                return Guid.Parse(claimsPrincipal.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
            }

            return Guid.Empty;
        }

        public static string UserEmailAddress(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal != null &&
                claimsPrincipal.Identity != null &&
                !string.IsNullOrEmpty(claimsPrincipal.Identity.Name))
            {
                return claimsPrincipal.Claims.Single(x => x.Type == ClaimTypes.Email).Value;
            }

            return string.Empty;
        }

        public static string UserName(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal != null &&
                claimsPrincipal.Identity != null &&
                !string.IsNullOrEmpty(claimsPrincipal.Identity.Name))
            {
                return claimsPrincipal.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
            }

            return string.Empty;
        }

        public static string UserModifiedDate(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal != null &&
                claimsPrincipal.Identity != null &&
                !string.IsNullOrEmpty(claimsPrincipal.Identity.Name))
            {
                return claimsPrincipal.Claims.Single(x => x.Type == ArragroClaimTypes.ModifiedDate).Value;
            }

            return string.Empty;
        }

        public static bool? UserIsExternalManaged(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal != null &&
                claimsPrincipal.Identity != null &&
                !string.IsNullOrEmpty(claimsPrincipal.Identity.Name))
            {
                return bool.Parse(claimsPrincipal.Claims.Single(x => x.Type == ArragroClaimTypes.IsExternalManaged).Value);
            }

            return null;
        }
    }
}
