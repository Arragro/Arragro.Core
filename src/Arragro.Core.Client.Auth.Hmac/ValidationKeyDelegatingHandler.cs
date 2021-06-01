using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Client.Auth.Hmac
{
    public class ValidationKeyDelegatingHandler : DelegatingHandler
    {
        private readonly string _appId;
        private readonly string _validationKey;

        public ValidationKeyDelegatingHandler(string appId, string validationKey)
        {
            _appId = !string.IsNullOrWhiteSpace(appId) ? appId : throw new ArgumentNullException(nameof(appId));
            _validationKey = !string.IsNullOrWhiteSpace(validationKey) ? validationKey : throw new ArgumentNullException(nameof(validationKey));

            try
            {
                Convert.FromBase64String(_validationKey);
            }
            catch (FormatException)
            {
                throw new ArgumentException($"{nameof(validationKey)} must be a valid base64 string.");
            }
        }

        public ValidationKeyDelegatingHandler(string appId, string apiKey, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _appId = appId ?? throw new ArgumentNullException(nameof(appId));
            _validationKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestContentBase64String = string.Empty;

            var requestUri = WebUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());

            var requestHttpMethod = request.Method.Method;

            // Calculate UNIX time
            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = DateTime.UtcNow - epochStart;
            var requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            // create random nonce for each request
            var nonce = Guid.NewGuid().ToString("N");

            // Checking if the request contains body, usually will be null wiht HTTP GET and DELETE
            if (request.Content != null)
            {
                var content = await request.Content.ReadAsByteArrayAsync();
                var md5 = MD5.Create();

                // Hashing the request body, any change in request body will result in different hash, we'll incure message integrity
                var requestContentHash = md5.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            // Creating the raw signature string
            var signatureRawData = $"{_appId}{requestHttpMethod}{requestUri}{requestTimeStamp}{nonce}{requestContentBase64String}";

            var apiKeyBytes = Convert.FromBase64String(_validationKey);
            var signature = Encoding.UTF8.GetBytes(signatureRawData);

            using (var hmac = new HMACSHA256(apiKeyBytes))
            {
                var signatureBytes = hmac.ComputeHash(signature);
                var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

                // Setting the values in the Authorization header using custom scheme (Hmac)
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Hmac",
                    $"{_appId}:{requestSignatureBase64String}:{nonce}:{requestTimeStamp}");
            }

            var response = await base.SendAsync(request, cancellationToken);

            return response;
        }
    }
}
