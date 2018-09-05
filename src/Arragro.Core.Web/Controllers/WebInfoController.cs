using Arragro.Core.Common.Interfaces;
using Arragro.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Arragro.Core.Web.Controllers
{
    [Route("web-info")]
    [AllowAnonymous]
    public class WebInfoController : Controller
    {
        private static DateTime _startupTime = DateTime.UtcNow;
        private readonly BaseSettings _baseSettings;
        private bool _migrationsAppliedNoMoreTests = false;

        public WebInfoController(
            BaseSettings baseSettings)
        {
            _baseSettings = baseSettings;
        }

        private IEnumerable<dynamic> GetIpAddress()
        {
            var interfaces = 
                NetworkInterface.GetAllNetworkInterfaces()
                    .Where(c => c.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                                c.OperationalStatus == OperationalStatus.Up);

            if (interfaces.Any())
            {
                var output = new List<dynamic>();
                foreach (var i in interfaces)
                {
                    var props = i.GetIPProperties();
                    // get first IPV4 address assigned to this interface
                    var ipInfo = props.UnicastAddresses
                        .Where(c => c.Address.AddressFamily == AddressFamily.InterNetwork)
                        .ToList();

                    output.Add(new
                    {
                        Name = i.Name,
                        Description = i.Description,
                        MacAddress = i.GetPhysicalAddress().ToString(),
                        IPAddress = ipInfo.Select(x => x.Address.ToString())
                    });
                }
                return output;
            }
            return Enumerable.Empty<UnicastIPAddressInformation>();
        }

        [HttpGet("{secret}")]
        public ActionResult Index(Guid secret)
        {
            if (_baseSettings != null &&
                _baseSettings.WebInfoSettings.IsWebInfoEnabled &&
                _baseSettings.WebInfoSettings.Secret != Guid.Empty &&
                _baseSettings.WebInfoSettings.Secret == secret &&
                _baseSettings.AllDbContextMigrationsApplied != null)
            {
                if (!_migrationsAppliedNoMoreTests)
                {
                    var results = _baseSettings.AllDbContextMigrationsApplied.TestDbContextsMigrated();
                    if (results.Any(x => !x.Migrated))
                    {
                        return new ConflictResult();
                    }
                    else
                    {
                        _migrationsAppliedNoMoreTests = true;
                    }
                }

                return Json(
                new {
                    AssemblyVersion = typeof(RuntimeEnvironment).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version,
                    IPAddressInformation = GetIpAddress(),
                    UpTime = DateTime.UtcNow - _startupTime
                });
            }
            return new NotFoundResult();
        }
    }
}
