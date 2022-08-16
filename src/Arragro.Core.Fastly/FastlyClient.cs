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
        private readonly List<string> _apiTokens;
        private static long _currentIndex = 0;

        public FastlyClient(
            ILogger<FastlyClient> logger,
            HttpClient httpClient,
            FastlyApiTokens fastlyApiTokens)
        {
            _logger = logger;
            _httpClient = httpClient;
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

        public async Task<bool> PurgeKeysAsync(string serviceId, string[] keys)
        {
            var result = true;
            if (!string.IsNullOrEmpty(serviceId) &&
                _apiTokens.Any())
            {
                await keys.ForEachAsync(4, async key =>
                {
                    var currentIndex = Interlocked.Read(ref _currentIndex);
                    var apiToken = _apiTokens[(int)currentIndex];
                    try
                    {

                        _logger.LogDebug("Purging {@Key} with {@ApiToken}", key, apiToken);

                        var request = new HttpRequestMessage(HttpMethod.Post, $"/service/{serviceId}/purge/{key}");
                        request.Headers.Add("Fastly-Key", apiToken);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var httpResponseMessage = await _httpClient.SendAsync(request);

                        if (!httpResponseMessage.IsSuccessStatusCode)
                        {
                            var body = await httpResponseMessage.Content.ReadAsStringAsync();
                            var fastlyException = new FastlyException("Something has gone wrong when purging against fastly.", httpResponseMessage, body);
                            _logger.LogError("Failed to purge key '{@Key}' with {@ApiToken} {@Exception}.", key, apiToken, fastlyException);
                            throw fastlyException;
                        }
                    }
                    catch (Exception ex)
                    {
                        result = false;
                    }
                    finally
                    {
                        SetCurrentIndex();
                    }
                });
            }

            return result;
        }
    }
}
