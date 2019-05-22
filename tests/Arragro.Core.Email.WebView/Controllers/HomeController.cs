using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Arragro.Core.Email.WebView.Models;
using Arragro.Core.Email.Razor.Services;
using Arragro.Core.Email.Razor.Models;
using System.Net;
using System.Text.RegularExpressions;

namespace Arragro.Core.Email.WebView.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

        public HomeController(IRazorViewToStringRenderer razorViewToStringRenderer)
        {
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ConfirmAccountEmailHtml()
        {
            var confirmAccountEmailViewModel = new ConfirmAccountEmailViewModel(true, "https://www.google.com");
            var content = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Html/Account/ConfirmEmail.cshtml", confirmAccountEmailViewModel);
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = content
            };
        }

        public async Task<IActionResult> ConfirmAccountEmailText()
        {
            var confirmAccountEmailViewModel = new ConfirmAccountEmailViewModel(true, "https://www.google.com");
            var content = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Text/Account/ConfirmEmail.cshtml", confirmAccountEmailViewModel);

            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = content
            };
        }

        public async Task<IActionResult> ResetPasswordHtml()
        {
            var resetPasswordViewModel = new ResetPasswordViewModel("https://www.google.com");
            var content = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Html/Account/ResetPassword.cshtml", resetPasswordViewModel);
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = content
            };
        }

        public async Task<IActionResult> ResetPasswordText()
        {
            var resetPasswordViewModel = new ResetPasswordViewModel("https://www.google.com");
            var content = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/Text/Account/ResetPassword.cshtml", resetPasswordViewModel);

            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = content
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
