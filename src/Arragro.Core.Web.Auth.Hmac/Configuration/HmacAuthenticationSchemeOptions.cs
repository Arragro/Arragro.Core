using Arragro.Core.Web.Auth.Hmac.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace Arragro.Core.Web.Auth.Hmac.Configuration
{
    public class HmacAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public long MaxRequestAgeInSeconds { get; set; }

        public string AuthenticationScheme { get; set; }

        public IHmacAuthorizationProvider AuthorizationProvider { get; set; }

        public HmacAuthenticationSchemeOptions()
        {
            MaxRequestAgeInSeconds = HmacAuthenticationDefaults.MaxRequestAgeInSeconds;
            AuthenticationScheme = HmacAuthenticationDefaults.AuthenticationScheme;
        }
    }
}
