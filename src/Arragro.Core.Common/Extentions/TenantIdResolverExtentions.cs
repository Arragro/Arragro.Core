using Arragro.Core.Common.Interfaces;
using Arragro.Core.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Arragro.Core.Common.Extentions
{
    public static class TenantIdResolverExtentions
    {
        public static IServiceCollection ConfigureTenantIdResolver(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddScoped<ITenantIdResolver, TenantIdResolver>();
            return serviceCollection;
        }
    }
}
