using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using Microsoft.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.Core.Web.Http
{
    public static class ETagHelper
    {
        public static string GenerateETag(string path, DateTimeOffset? lastModified)
        {
            var encoding = new UTF8Encoding();

            if (!lastModified.HasValue) lastModified = DateTimeOffset.UtcNow;

            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(encoding.GetBytes($"{path}-{lastModified.Value.Ticks}"));
                return $"\"{Convert.ToBase64String(bytes)}\"";
            }
        }

        public static EntityTagHeaderValue GetEntityTagHeaderValue(string path, DateTimeOffset? lastModified)
        {
            return EntityTagHeaderValue.Parse(GenerateETag(path, lastModified));
        }
        
        public static IActionResult HandleETag<T>(this Controller controller, DateTimeOffset? lastModified, T data)
        {
            if (lastModified.HasValue)
            {
                var etag = GetEntityTagHeaderValue(controller.Request.Path, lastModified);
                var requestHeaders = controller.Request.GetTypedHeaders();

                if (requestHeaders.IfNoneMatch != null && requestHeaders.IfNoneMatch.Any() && requestHeaders.IfNoneMatch.First().Tag == etag.Tag)
                {
                    return controller.StatusCode((int)HttpStatusCode.NotModified);
                }

                var responseHeaders = controller.Response.GetTypedHeaders();
                responseHeaders.ETag = etag;
                responseHeaders.LastModified = lastModified;
            }
            return controller.Ok(data);
        }

        public static async Task<IActionResult> HandleETagAsync<T>(this Controller controller, DateTimeOffset? lastModified, Func<Task<T>> buildData)
        {
            if (lastModified.HasValue)
            {
                var etag = GetEntityTagHeaderValue(controller.Request.Path, lastModified);
                var requestHeaders = controller.Request.GetTypedHeaders();

                if (requestHeaders.IfNoneMatch != null && requestHeaders.IfNoneMatch.Any() && requestHeaders.IfNoneMatch.First().Tag == etag.Tag)
                {
                    return controller.StatusCode((int)HttpStatusCode.NotModified);
                }

                var responseHeaders = controller.Response.GetTypedHeaders();
                responseHeaders.ETag = etag;
                responseHeaders.LastModified = lastModified;
            }
            return controller.Ok(await buildData());
        }
    }
}
