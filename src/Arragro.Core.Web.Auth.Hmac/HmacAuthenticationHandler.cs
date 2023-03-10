using Arragro.Core.DistributedCache;
using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using Arragro.Core.Web.Auth.Hmac.Models;
using Arragro.Core.Web.Auth.Hmac.Configuration;

namespace Arragro.Core.Web.Auth.Hmac
{
    public class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationSchemeOptions>
    {
        private readonly ILogger<HmacAuthenticationHandler> _logger;
        private readonly IDistributedCacheManager _distributedCacheManager;

        public HmacAuthenticationHandler(
            ILogger<HmacAuthenticationHandler> loggerHmacAuthenticationHandler,
            IDistributedCacheManager distributedCacheManager,
            IOptionsMonitor<HmacAuthenticationSchemeOptions> options,
            ILoggerFactory logger, 
            UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _logger = loggerHmacAuthenticationHandler;
            _distributedCacheManager = distributedCacheManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Options.AuthorizationProvider == null)
            {
                throw new ArgumentException($"{nameof(Options.AuthorizationProvider)} is required.");
            }

            if (!Request.Headers.TryGetValue("Authorization", out var authorization))
            {
                return AuthenticateResult.Fail("Missing 'Authorization' header.");
            }

            var validationResult = await ValidateAsync(Request);

            if (validationResult.Valid)
            {
                var claimsToSet = new Claim[] { new Claim(ClaimTypes.NameIdentifier, validationResult.Username) };

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claimsToSet, HmacAuthenticationDefaults.AuthenticationType, ClaimTypes.NameIdentifier, ClaimTypes.Role));
                var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), Options.AuthenticationScheme);
                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("Authentication failed");
        }

        private async Task<ValidationResult> ValidateAsync(HttpRequest request)
        {
            var result = new ValidationResult();

            if (request.Headers.TryGetValue("Authorization", out var header))
            {
                var authenticationHeader = AuthenticationHeaderValue.Parse(header);
                if (Options.AuthenticationScheme.Equals(authenticationHeader.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    var rawAuthenticationHeader = authenticationHeader.Parameter;
                    var authenticationHeaderArray = GetAuthenticationValues(rawAuthenticationHeader);

                    if (authenticationHeaderArray != null)
                    {
                        var appId = authenticationHeaderArray[0];
                        var incomingBase64Signature = authenticationHeaderArray[1];
                        var nonce = authenticationHeaderArray[2];
                        var requestTimeStamp = authenticationHeaderArray[3];

                        // Note that we must not dispose the memoryStream here, because the stream is needed in subsequent handlers
                        var memoryStream = new MemoryStream();

                        await request.Body.CopyToAsync(memoryStream);
                        request.Body = memoryStream;

                        try
                        {
                            result.Valid = await IsValidRequestAsync(request, memoryStream.ToArray(), appId, incomingBase64Signature, nonce, requestTimeStamp);
                            result.Username = appId;
                        }
                        finally
                        {
                            // We need to reset the stream so that subsequent handlers have a fresh stream which they can consume.
                            memoryStream.Seek(0, SeekOrigin.Begin);
                        }
                    }
                }
            }

            return result;
        }

        private async Task<bool> IsValidRequestAsync(HttpRequest req, byte[] body, string appId, string incomingBase64Signature, string nonce, string requestTimeStamp)
        {
            var requestContentBase64String = string.Empty;
            //var absoluteUri = string.Concat(
            //            req.Scheme,
            //            "://",
            //            req.Host.ToUriComponent(),
            //            req.PathBase.ToUriComponent(),
            //            req.Path.ToUriComponent(),
            //            req.QueryString.ToUriComponent());

            //_logger.LogInformation("IsValidRequestAsync:absoluteUri", absoluteUri);

            //var requestUri = WebUtility.UrlEncode(absoluteUri.ToLower());
            var requestHttpMethod = req.Method;

            var authorizationProviderResult = await Options.AuthorizationProvider.TryGetApiKeyAsync(appId);

            if (!authorizationProviderResult.Found)
            {
                return false;
            }

            if (await IsReplayRequestAsync(nonce, requestTimeStamp))
            {
                return false;
            }

            var hash = ComputeHash(body);

            if (hash != null)
            {
                requestContentBase64String = Convert.ToBase64String(hash);
            }

            var data = $"{appId}{requestHttpMethod}{requestTimeStamp}{nonce}{requestContentBase64String}";
            _logger.LogDebug("IsValidRequestAsync:data - @data", data);

            var apiKeyBytes = Convert.FromBase64String(authorizationProviderResult.ValidationKey);

            var signature = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA256(apiKeyBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);

                return incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal);
            }
        }

        private static string[] GetAuthenticationValues(string rawAuthenticationHeader)
        {
            var credArray = rawAuthenticationHeader.Split(':');
            return credArray.Length == 4 ? credArray : null;
        }

        private async Task<bool> IsReplayRequestAsync(string nonce, string requestTimeStamp)
        {
            var nonceInMemory = await _distributedCacheManager.GetAsync<string>(nonce);
            if (nonceInMemory != null)
            {
                return true;
            }

            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToInt64(requestTimeStamp);
            var diff = Math.Abs(serverTotalSeconds - requestTotalSeconds);

            if (diff > Options.MaxRequestAgeInSeconds)
            {
                return true;
            }

            await _distributedCacheManager.SetAsync(nonce.ToString(), requestTimeStamp, new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(Options.MaxRequestAgeInSeconds) });
            return false;
        }

        private static byte[] ComputeHash(byte[] body)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = null;
                if (body.Length != 0)
                {
                    hash = md5.ComputeHash(body);
                }

                return hash;
            }
        }
    }
}
