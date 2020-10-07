using Arragro.Core.Web.Extensions;
using Arragro.Core.Web.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Arragro.Core.Web.Middleware
{
    public class CookieServiceMiddleware : IMiddleware
    {
        private readonly ICookieService _cookieService;

        public CookieServiceMiddleware(
            ICookieService cookieService)
        {
            _cookieService = cookieService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // write cookies to response right before it starts writing out from MVC/api responses...
            context.Response.OnStarting(() =>
            {
                // cookie service should not write out cookies on 500, possibly others as well
                if (!context.Response.StatusCode.IsInRange(500, 599))
                {
                    _cookieService.WriteToResponse(context);
                }
                return Task.CompletedTask;
            });

            await next(context);
        }
    }
}
