using Arragro.Core.Web.Auth.Hmac.Interfaces;
using Arragro.Core.Web.Auth.Hmac.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.Core.Web.Auth.Hmac
{
    public class MemoryHmacAuthenticationProvider : IHmacAuthorizationProvider
    {
        private IDictionary<string, string> _hmacAuthenticatedApps;

        public MemoryHmacAuthenticationProvider(IDictionary<string, string> hmacAuthenticatedApps)
        {
            _hmacAuthenticatedApps = hmacAuthenticatedApps ?? throw new ArgumentNullException(nameof(hmacAuthenticatedApps));
        }

        public Task<AuthorizationProviderResult> TryGetApiKeyAsync(string appId)
        {
            if (_hmacAuthenticatedApps.TryGetValue(appId, out var validationKey))
            {
                return Task.FromResult(new AuthorizationProviderResult(appId, found: true, validationKey));
            }
            else
            {
                return Task.FromResult(new AuthorizationProviderResult(appId, found: false, validationKey: null));
            }
        }
    }
}
