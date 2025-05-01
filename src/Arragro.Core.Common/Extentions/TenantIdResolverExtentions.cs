using Arragro.Core.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        public Task<bool> ValidateTenandIdAccess(Guid tenantId)
        {
            return Task.FromResult(true);
        }
    }

    public static class TenantIdResolverExtentions
    {
        public static IServiceCollection ConfigureNoTenantIdResolver(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddScoped<ITenantIdResolver, NoTenantIdResolver>();
            return serviceCollection;
        }

        public static IServiceCollection ConfigureTenantIdResolver<T>(this IServiceCollection serviceCollection)
            where T : class, ITenantIdResolver
        {
            serviceCollection.TryAddScoped<ITenantIdResolver, T>();
            return serviceCollection;
        }
    }
}
