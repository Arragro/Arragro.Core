using Arragro.Core.DistributedCache;
using Arragro.Core.Web.Auth.Hmac;
using Arragro.Core.Web.Auth.Hmac.Interfaces;
using Arragro.Core.Web.Auth.Hmac.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Arragro.Core.Web.Auth.IntegrationTests
{
    public class HmacWebApplicationFactory : WebApplicationFactory<TestStartup>
    {
        private readonly HmacApplicationSetting _hmacApplicationSetting;

        public HmacWebApplicationFactory(HmacApplicationSetting hmacApplicationSetting)
        {
            _hmacApplicationSetting = hmacApplicationSetting;
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return new WebHostBuilder()
                .UseStartup<TestStartup>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services
                    .AddDistributedMemoryCache()
                    .AddTransient<IDistributedCacheManager, DistributedCacheManager>()
                    .AddSingleton(new DistributedCacheEntryOptions { SlidingExpiration = new TimeSpan(0, 5, 0) })
                    .AddAuthentication(o =>
                    {
                        o.DefaultScheme = HmacAuthenticationDefaults.AuthenticationScheme;
                    })
                    .AddHmacAuthentication(_hmacApplicationSetting);
            });

            base.ConfigureWebHost(builder);
        }
    }
}
