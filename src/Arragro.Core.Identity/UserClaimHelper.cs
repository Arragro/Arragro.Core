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
            if (httpContext != null &&
                httpContext.User != null &&
                httpContext.User.Identity != null &&
                !string.IsNullOrEmpty(httpContext.User.Identity.Name))
            {
                return Guid.Parse(httpContext.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
            }

            return Guid.Empty;
        }

        public static string UserEmailAddress(this HttpContext httpContext)
        {
            if (httpContext != null &&
                httpContext.User != null &&
                httpContext.User.Identity != null &&
                !string.IsNullOrEmpty(httpContext.User.Identity.Name))
            {
                return httpContext.User.Claims.Single(x => x.Type == ClaimTypes.Email).Value;
            }

            return string.Empty;
        }

        public static string UserName(this HttpContext httpContext)
        {
            if (httpContext != null &&
                httpContext.User != null &&
                httpContext.User.Identity != null &&
                !string.IsNullOrEmpty(httpContext.User.Identity.Name))
            {
                return httpContext.User.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
            }

            return string.Empty;
        }

        public static string UserModifiedDate(this HttpContext httpContext)
        {
            if (httpContext != null &&
                httpContext.User != null &&
                httpContext.User.Identity != null &&
                !string.IsNullOrEmpty(httpContext.User.Identity.Name))
            {
                return httpContext.User.Claims.Single(x => x.Type == ArragroClaimTypes.ModifiedDate).Value;
            }

            return string.Empty;
        }

        public static bool? UserIsExternalManaged(this HttpContext httpContext)
        {
            if (httpContext != null &&
                httpContext.User != null &&
                httpContext.User.Identity != null &&
                !string.IsNullOrEmpty(httpContext.User.Identity.Name))
            {
                return bool.Parse(httpContext.User.Claims.Single(x => x.Type == ArragroClaimTypes.IsExternalManaged).Value);
            }

            return null;
        }
    }
}
