using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Arragro.Core.Fastly
{
    public static class FastlyExtensions
    {
        public static IServiceCollection ConfigureFastlyClient(this IServiceCollection serviceCollection, FastlyApiTokens fastlyApiTokens)
        {
            serviceCollection.AddSingleton(fastlyApiTokens);
            serviceCollection.AddSingleton<FastlyHelper>();
            if (fastlyApiTokens.Enabled)
            {
                serviceCollection.AddHttpClient<IFastlyClient, FastlyClient>();   
            }
            else
            {
                serviceCollection.AddHttpClient<IFastlyClient, DummyFastlyClient>();
            }

            return serviceCollection;
        }
    }
}