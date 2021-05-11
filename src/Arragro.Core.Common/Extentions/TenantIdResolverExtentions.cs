using Arragro.Core.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Extentions
{
    public class NoTenantIdResolver : ITenantIdResolver
    {
        public Guid ResolveTenantId()
        {
            return Guid.Empty;
        }

        public Task<Guid> ResolveTenantIdAsync()
        {
            return Task.FromResult(Guid.Empty);
        }
    }

    public static class TenantIdResolverExtentions
    {
        public static IServiceCollection ConfigureNoTenantIdResolver(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ITenantIdResolver, NoTenantIdResolver>();
        }

        public static IServiceCollection ConfigureTenantIdResolver<T>(this IServiceCollection serviceCollection)
            where T : class, ITenantIdResolver
        {
            return serviceCollection.AddScoped<ITenantIdResolver, T>();
        }
    }
}
