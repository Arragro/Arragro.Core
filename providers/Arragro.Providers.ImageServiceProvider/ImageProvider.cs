using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arragro.Providers.ImageServiceProvider
{
    public class ImageProvider : IImageProvider
    {
        private readonly HttpClient _httpClient;

        public ImageProvider(
            string imageServiceUrl,
            int timout = 3000)
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress = new Uri(imageServiceUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.Timeout = TimeSpan.FromMilliseconds(timout);
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