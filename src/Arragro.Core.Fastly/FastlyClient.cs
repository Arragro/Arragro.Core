using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Fastly
{
    public class FastlyClient : IFastlyClient
    {
        private readonly ILogger<FastlyClient> _logger;
        private readonly HttpClient _httpClient;
        private readonly List<string> _serviceIds;
        private readonly List<string> _apiTokens;
        private static long _currentIndex = 0;

        public FastlyClient(
            ILogger<FastlyClient> logger,
            HttpClient httpClient,
            FastlyApiTokens fastlyApiTokens)
        {
            _logger = logger;
            _httpClient = httpClient;
            _serviceIds = fastlyApiTokens.GetServiceIds();
            _apiTokens = fastlyApiTokens.GetApiTokens();
            _httpClient.BaseAddress = new Uri("https://api.fastly.com");
        }

        private void SetCurrentIndex()
        {
            var currentIndex = Interlocked.Read(ref _currentIndex);
            if (currentIndex >= _apiTokens.Count - 1)
            {
                Interlocked.Exchange(ref _currentIndex, 0);
            }
            else
            {
                Interlocked.Add(ref _currentIndex, 1);
            }
        }

        private async Task<bool> PurgeKeysBatchAsync(string serviceId, string[] keys)
        {
            var result = true;
            var currentIndex = Interlocked.Read(ref _currentIndex);
            var apiToken = _apiTokens[(int)currentIndex];
            if (apiToken == "testing") return true;
            try
            {
                _logger.LogInformation("Purging {@Key} with {@ApiToken} for ServiceId {@ServiceId}", string.Join(",", keys), apiToken, serviceId);

                var request = new HttpRequestMessage(HttpMethod.Post, $"/service/{serviceId}/purge");
                request.Headers.Add("Fastly-Key", apiToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("surrogate-key", string.Join(" ", keys));

                var httpResponseMessage = await _httpClient.SendAsync(request);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var body = await httpResponseMessage.Content.ReadAsStringAsync();
                    var fastlyException = new FastlyException("Something has gone wrong when purging against fastly.", httpResponseMessage, body);
                    _logger.LogError("Failed to purge key '{@Key}' with {@ApiToken} {@Exception}.", string.Join(",", keys), apiToken, fastlyException);
                    throw fastlyException;
                }
            }
            catch (Exception ex)
            {
                if (!(ex is FastlyException))
                {
                    _logger.LogError("Fastly issue.", ex);
                }
                result = false;
            }
            finally
            {
                SetCurrentIndex();
            }
            return result;
        }

        private async Task<bool> PurgeKeysAsync(string serviceId, string[] keys)
        {
            if (!string.IsNullOrEmpty(serviceId) &&
                _apiTokens.Any())
            {
                var keyLength = keys.Length / 256;
                var remainder = keys.Length % 256;

                for (var i = 0; i < keyLength; i++)
                {
                    var keysToPurge = keys.Skip(i * 256).Take(256).ToArray();
                    var result = await PurgeKeysBatchAsync(serviceId, keysToPurge);
                    if (!result) return false;
                }
                if (remainder > 0)
                {
                    var keysToPurge = keys.Skip(keyLength * 256).Take(256).ToArray();
                    var result = await PurgeKeysBatchAsync(serviceId, keysToPurge);
                    if (!result) return false;
                }
            }
            return true;
        }

        public async Task<bool> PurgeKeysAsync(string[] keys, int? waitMilliseconds = null)
        {
            foreach (var serviceId in _serviceIds)
            {
                if (serviceId != "testing")
                {
                    var result = await PurgeKeysAsync(serviceId, keys);
                    if (!result) return result;
                }
                if (waitMilliseconds.HasValue)
                {
                    Thread.Sleep(waitMilliseconds.Value);
                }
            }
            return true;
        }

        public async Task<bool> PurgeAllAsync(string serviceId)
        {
            var result = true;
            var currentIndex = Interlocked.Read(ref _currentIndex);
            var apiToken = _apiTokens[(int)currentIndex];
            if (apiToken == "testing") return true;

            try
            {
                _logger.LogInformation("Purging All with {@ApiToken} for ServiceId {@ServiceId}", apiToken, serviceId);
                var request = new HttpRequestMessage(HttpMethod.Post, $"/service/{serviceId}/purge_all");
                request.Headers.Add("Fastly-Key", apiToken);

                var httpResponseMessage = await _httpClient.SendAsync(request);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var body = await httpResponseMessage.Content.ReadAsStringAsync();
                    var fastlyException = new FastlyException("Something has gone wrong when purging against fastly.", httpResponseMessage, body);
                    _logger.LogError("Failed to purge all {@ApiToken} {@Exception}.", apiToken, fastlyException);
                    throw fastlyException;
                }
            }
            catch (Exception ex)
            {
                if (!(ex is FastlyException))
                {
                    _logger.LogError("Fastly issue.", ex);
                }
                result = false;
            }
            finally
            {
                SetCurrentIndex();
            }
            return result;
        }

        public async Task<bool> PurgeAllAsync(int? waitMilliseconds = null)
        {
            foreach (var serviceId in _serviceIds)
            {
                if (serviceId != "testing")
                {
                    var result = await PurgeAllAsync(serviceId);
                    if (!result) return result;
                }
                if (waitMilliseconds.HasValue)
                {
                    Thread.Sleep(waitMilliseconds.Value);
                }
            }
            return true;
        }
    }
}
