using Arragro.Core.Client.Auth.Hmac;
using Arragro.Core.Web.Auth.Hmac;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xunit;

namespace Arragro.Core.Web.Auth.IntegrationTests
{
    public class HmacTests
    {
        private string _applicationId;
        private string _vendorKey;

        public HmacTests()
        {
            _applicationId = Guid.NewGuid().ToString();
            _vendorKey = GenerateHmacKey();
        }

        private string GenerateHmacKey()
        {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                byte[] secretKeyByteArray = new byte[32];
                cryptoProvider.GetBytes(secretKeyByteArray);
                return Convert.ToBase64String(secretKeyByteArray);
            }
        }

        [Fact]
        public Task Request_Authorized()
        {
            return this.TestRequestAsync(
                new Dictionary<string, string>() { { _applicationId, _vendorKey } },
                _applicationId, 
                _vendorKey,
                HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("appIdTest", "y3ScPfAzPjMU/8mNjbAe0uBwCrR1DuElHx8D64015hY=")]
        [InlineData("wrongAppId", "h++EfSifTAveqQ01SXuSDLrDbr7HsfCE/hI9rEZOYwk=")]
        public Task Request_Unauthorized(string appId, string apiKey)
        {
            return TestRequestAsync(
                new Dictionary<string, string>() { { "appIdTest", "h++EfSifTAveqQ01SXuSDLrDbr7HsfCE/hI9rEZOYwk=" } },
                appId,
                apiKey,
                HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("appId", "asdfsadfsdf")]
        [InlineData("wrongAppId", "asdfsadfsdf")]
        public Task Request_ApiKeyBadFormat_ThrowsException(string appId, string apiKey)
        {
            return Assert.ThrowsAsync<ArgumentException>(() => TestRequestAsync(
                new Dictionary<string, string>() { { _applicationId, _vendorKey } },
                appId,
                apiKey,
                HttpStatusCode.Unauthorized));
        }

        [Fact]
        public async Task Request_Authorized_UsernameAppIdSet()
        {
            var result = await TestRequestAsync(
                new Dictionary<string, string>() { { _applicationId, _vendorKey } },
                _applicationId, 
                _vendorKey,
                HttpStatusCode.OK,
                "api/test/name");

            var content = await result.Content.ReadAsStringAsync();

            Assert.Equal(_applicationId, content);
        }

        [Fact]
        public async Task Request_Authorized_Claims()
        {
            var result = await TestRequestAsync(
                new Dictionary<string, string>() { { _applicationId, _vendorKey } },
                _applicationId, 
                _vendorKey,
                HttpStatusCode.OK,
                "api/test/claims");

            var contentAsString = await result.Content.ReadAsStringAsync();
            dynamic content = JArray.Parse(contentAsString);

            Assert.Equal(1, content.Count);
            Assert.Equal(ClaimTypes.NameIdentifier, content[0].name.Value);
            Assert.Equal(_applicationId, content[0].value.Value);
        }


        private async Task<HttpResponseMessage> TestRequestAsync(
            IDictionary<string, string> authenticatedApps,
            string appId,
            string validationKey,
            HttpStatusCode expectedStatusCode,
            string endpoint = "api/test")
        {
            using (var client = GetHttpClient(
                authenticatedApps,
                appId,
                validationKey))
            {
                var response = await client.GetAsync(endpoint);
                Assert.True(response.StatusCode == expectedStatusCode);

                return response;
            }
        }

        private HttpClient GetHttpClient(IDictionary<string, string> hmacAuthenticatedApps, string appId, string validationKey)
        {
            var factory = new HmacWebApplicationFactory(new MemoryHmacAuthenticationProvider(hmacAuthenticatedApps))
                .WithWebHostBuilder(builder => builder.UseSolutionRelativeContentRoot("../../"));
            return factory.CreateDefaultClient(new ValidationKeyDelegatingHandler(appId, validationKey));
        }
    }
}
