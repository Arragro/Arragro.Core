using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arragro.Providers.ImageServiceProvider
{
    public static class ImageProviderExtentions
    {
        public static void ConfigureImageProvider(
            this IServiceCollection services,
            string imageServiceUrl,
            int timout = 3000)
        {
            services.AddHttpClient(nameof(ImageProvider), config =>
            {
                config.BaseAddress = new Uri(imageServiceUrl);
                config.DefaultRequestHeaders.Accept.Clear();
                config.Timeout = TimeSpan.FromMilliseconds(timout);
            }).AddPolicyHandler(GetRetryPolicy());

            services.AddTransient<IImageProvider, ImageProvider>();
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
              // Handle HttpRequestExceptions, 408 and 5xx status codes
              .HandleTransientHttpError()
              // Handle 404 not found
              //.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
              // Handle 401 Unauthorized
              //.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.Unauthorized)
              // What to do if any of the above erros occur:
              // Retry 3 times, each time wait 1,2 and 4 seconds before retrying.
              .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }

    public class ImageProvider : IImageProvider
    {
        private readonly HttpClient _httpClient;

        public ImageProvider(
            IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(ImageProvider));
        }

        private string GetHeaderValue(HttpResponseMessage response, string name)
        {
            IEnumerable<string> values;
            if (response.Headers.TryGetValues(name, out values))
            {
                return values.First();
            }
            return null;
        }

        private async Task<ImageProcessResult> ProcessResponse(HttpResponseMessage response)
        {
            var output = await response.Content.ReadAsByteArrayAsync();
            var mimeType = response.Content.Headers.ContentType.MediaType;
            var responseWidth = int.Parse(GetHeaderValue(response, "Image-Width"));
            var responseHeight = int.Parse(GetHeaderValue(response, "Image-Height"));
            var isImage = bool.Parse(GetHeaderValue(response, "IsImage"));
            return new ImageProcessResult { Bytes = output, IsImage = isImage, Width = responseWidth, Height = responseHeight, Size = output.Length, MimeType = mimeType };
        }

        public async Task<ImageProcessResult> GetImage(byte[] bytes, int quality = 80, bool asProgressiveJpeg = false)
        {
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StreamContent(new MemoryStream(bytes)), "image", "image");
                content.Add(new StringContent(quality.ToString()), "quality");
                content.Add(new StringContent(asProgressiveJpeg.ToString()), "asProgressiveJpeg");

                using (var response = await _httpClient.PostAsync("/image/resize", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return await ProcessResponse(response);
                    }
                    else
                    {
                        return new ImageProcessResult { Bytes = bytes, IsImage = false, Size = bytes.Length };
                    }
                }
            }
        }

        public async Task<ImageProcessResult> GetImage(byte[] bytes, int width, int quality = 80, bool asProgressiveJpeg = false)
        {
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StreamContent(new MemoryStream(bytes)), "image", "image");
                content.Add(new StringContent(width.ToString()), "width");
                content.Add(new StringContent(quality.ToString()), "quality");
                content.Add(new StringContent(asProgressiveJpeg.ToString()), "asProgressiveJpeg");

                using (var response = await _httpClient.PostAsync("/image/resize", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return await ProcessResponse(response);
                    }
                    else
                    {
                        return new ImageProcessResult { Bytes = bytes, IsImage = false, Size = bytes.Length };
                    }
                }
            }
        }
    }
}