using Arragro.Core.Web.Auth.Hmac.Configuration;
using Arragro.Core.Web.Auth.Hmac.Interfaces;
using Arragro.Core.Web.Auth.Hmac.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arragro.Core.Web.Auth.Hmac
{
    public static class HmacAuthenticationExtensions
    {
        public static AuthenticationBuilder AddHmacAuthentication(
            this AuthenticationBuilder builder,
            IEnumerable<HmacApplicationSetting> hmacApplicationSettings,
            string authenticationScheme = HmacAuthenticationDefaults.AuthenticationScheme,
            string displayName = HmacAuthenticationDefaults.AuthenticationType,
            Action<HmacAuthenticationSchemeOptions> configureOptions = null)
        {
            builder.Services.AddSingleton<IHmacAuthorizationProvider>(_ => new MemoryHmacAuthenticationProvider(hmacApplicationSettings.ToDictionary(x => x.ApplicationId, x => x.ValidationKey)));
            builder.Services.AddSingleton<IPostConfigureOptions<HmacAuthenticationSchemeOptions>, HmacAuthenticationPostConfigureOptions>();
            return builder.AddScheme<HmacAuthenticationSchemeOptions, HmacAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
        public static AuthenticationBuilder AddHmacAuthentication(
            this AuthenticationBuilder builder,
            HmacApplicationSetting hmacApplicationSetting,
            string authenticationScheme = HmacAuthenticationDefaults.AuthenticationScheme,
            string displayName = HmacAuthenticationDefaults.AuthenticationType,
            Action<HmacAuthenticationSchemeOptions> configureOptions = null)
        {
            return builder.AddHmacAuthentication(new List<HmacApplicationSetting> { hmacApplicationSetting }, authenticationScheme, displayName, configureOptions);
        }
    }
}
