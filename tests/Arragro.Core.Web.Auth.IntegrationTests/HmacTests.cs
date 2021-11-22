using Arragro.Core.Client.Auth.Hmac;
using Arragro.Core.Web.Auth.Hmac.Models;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using System;
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
        private HmacApplicationSetting _hmacApplicationSetting;

        public HmacTests()
        {
            _hmacApplicationSetting = new HmacApplicationSetting
            {
                ApplicationId = Guid.NewGuid().ToString(),
                ValidationKey = GenerateHmacKey()
            };
        }

        private string GenerateHmacKey()
        {
            var secretKeyByteArray = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(secretKeyByteArray);
        }

        [Fact]
        public Task Request_Authorized()
        {
            return this.TestRequestAsync(
                _hmacApplicationSetting,
                _hmacApplicationSetting.ApplicationId,
                _hmacApplicationSetting.ValidationKey,
                HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("appIdTest", "y3ScPfAzPjMU/8mNjbAe0uBwCrR1DuElHx8D64015hY=")]
        [InlineData("wrongAppId", "h++EfSifTAveqQ01SXuSDLrDbr7HsfCE/hI9rEZOYwk=")]
        public Task Request_Unauthorized(string appId, string valiidationKey)
        {
            return TestRequestAsync(
                new HmacApplicationSetting
                {
                    ApplicationId = "appIdTest", 
                    ValidationKey = "h++EfSifTAveqQ01SXuSDLrDbr7HsfCE/hI9rEZOYwk=" 
                },
                appId,
                valiidationKey,
                HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("appId", "asdfsadfsdf")]
        [InlineData("wrongAppId", "asdfsadfsdf")]
        public Task Request_ApiKeyBadFormat_ThrowsException(string appId, string validationKey)
        {
            return Assert.ThrowsAsync<ArgumentException>(() => TestRequestAsync(
                _hmacApplicationSetting,
                appId,
                validationKey,
                HttpStatusCode.Unauthorized));
        }

        [Fact]
        public async Task Request_Authorized_UsernameAppIdSet()
        {
            var result = await TestRequestAsync(
                _hmacApplicationSetting,
                _hmacApplicationSetting.ApplicationId,
                _hmacApplicationSetting.ValidationKey,
                HttpStatusCode.OK,
                "api/test/name");

            var content = await result.Content.ReadAsStringAsync();

            Assert.Equal(_hmacApplicationSetting.ApplicationId, content);
        }

        [Fact]
        public async Task Request_Authorized_Claims()
        {
            var result = await TestRequestAsync(
                _hmacApplicationSetting,
                _hmacApplicationSetting.ApplicationId, 
                _hmacApplicationSetting.ValidationKey,
                HttpStatusCode.OK,
                "api/test/claims");

            var contentAsString = await result.Content.ReadAsStringAsync();
            dynamic content = JArray.Parse(contentAsString);

            Assert.Equal(1, content.Count);
            Assert.Equal(ClaimTypes.NameIdentifier, content[0].name.Value);
            Assert.Equal(_hmacApplicationSetting.ApplicationId, content[0].value.Value);
        }


        private async Task<HttpResponseMessage> TestRequestAsync(
            HmacApplicationSetting hmacApplicationSetting,
            string appId, 
            string validationKey,
            HttpStatusCode expectedStatusCode,
            string endpoint = "api/test")
        {
            using (var client = GetHttpClient(hmacApplicationSetting, appId, validationKey))
            {
                var response = await client.GetAsync(endpoint);
                Assert.True(response.StatusCode == expectedStatusCode);

                return response;
            }
        }

        private HttpClient GetHttpClient(HmacApplicationSetting hmacApplicationSetting, string appId, string validationKey)
        {
            var factory = new HmacWebApplicationFactory(hmacApplicationSetting)
                .WithWebHostBuilder(builder => builder.UseSolutionRelativeContentRoot("../../"));
            return factory.CreateDefaultClient(new ValidationKeyDelegatingHandler(appId, validationKey));
        }
    }
}
