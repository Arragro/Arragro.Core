using Arragro.Core.Common.Interfaces;
using Arragro.Core.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arragro.Core.Common.Extensions
{
    public static class TenantIdResolverExtensions
    {
        public static IServiceCollection ConfigureTenantIdResolver(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddScoped<ITenantIdResolver, TenantIdResolver>();
            return serviceCollection;
        }
    }
}
