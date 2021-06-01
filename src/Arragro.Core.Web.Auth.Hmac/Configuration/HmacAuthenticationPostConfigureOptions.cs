using Arragro.Core.Web.Auth.Hmac.Interfaces;
using Microsoft.Extensions.Options;
using System;

namespace Arragro.Core.Web.Auth.Hmac.Configuration
{
    public class HmacAuthenticationPostConfigureOptions : IPostConfigureOptions<HmacAuthenticationSchemeOptions>
    {
        private readonly IHmacAuthorizationProvider _hmacAuthorizationProvider;

        public HmacAuthenticationPostConfigureOptions(IHmacAuthorizationProvider hmacAuthorizationProvider = null)
        {
            _hmacAuthorizationProvider = hmacAuthorizationProvider;
        }

        public void PostConfigure(string name, HmacAuthenticationSchemeOptions options)
        {
            if (_hmacAuthorizationProvider != null)
            {
                options.AuthorizationProvider = _hmacAuthorizationProvider;
            }
            else
            {
                throw new InvalidOperationException("IHmacAuthorizationProvider must be configured in DI.");
            }
        }
    }
}
