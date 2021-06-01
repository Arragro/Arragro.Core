using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Arragro.Core.Web.Auth.IntegrationTests
{
    [Route("api/[controller]")]
    [Authorize]
    public class TestController : Controller
    {
        public ActionResult Index()
        {
            return Ok();
        }

        [Route("Name")]
        public ActionResult GetName()
        {
            return Ok(User.Identity.Name);
        }

        [Route("Claims")]
        public ActionResult GetClaims()
        {
            return Ok(User.Claims.Select(p => new { Name = p.Type, Value = p.Value }));
        }
    }
}
