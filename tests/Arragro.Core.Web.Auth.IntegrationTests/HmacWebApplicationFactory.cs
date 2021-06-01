using Arragro.Core.DistributedCache;
using Arragro.Core.Web.Auth.Hmac;
using Arragro.Core.Web.Auth.Hmac.Configuration;
using Arragro.Core.Web.Auth.Hmac.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Arragro.Core.Web.Auth.IntegrationTests
{
    public class HmacWebApplicationFactory : WebApplicationFactory<TestStartup>
    {
        private readonly IHmacAuthorizationProvider _hmacAuthorizationProvider;

        public HmacWebApplicationFactory(IHmacAuthorizationProvider hmacAuthorizationProvider)
        {
            _hmacAuthorizationProvider = hmacAuthorizationProvider;
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
                    .AddSingleton(_hmacAuthorizationProvider)
                    .AddAuthentication(o =>
                    {
                        o.DefaultScheme = HmacAuthenticationDefaults.AuthenticationScheme;
                    })
                    .AddHmacAuthentication();
            });

            base.ConfigureWebHost(builder);
        }
    }
}
