using Arragro.Core.Web.Auth.Hmac.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Arragro.Core.Web.Auth.Hmac
{
    public static class HmacAuthenticationExtensions
    {
        public static AuthenticationBuilder AddHmacAuthentication(
            this AuthenticationBuilder builder,
            string authenticationScheme = HmacAuthenticationDefaults.AuthenticationScheme,
            string displayName = HmacAuthenticationDefaults.AuthenticationType,
            Action<HmacAuthenticationSchemeOptions> configureOptions = null)
        {
            builder.Services.AddSingleton<IPostConfigureOptions<HmacAuthenticationSchemeOptions>, HmacAuthenticationPostConfigureOptions>();
            return builder.AddScheme<HmacAuthenticationSchemeOptions, HmacAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
