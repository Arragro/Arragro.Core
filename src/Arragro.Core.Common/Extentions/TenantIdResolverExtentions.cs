using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Arragro.Core.Common.Extentions
{
    public class TenantIdResolver
    {
        private Guid? _tenantId { get; set; } = null;

        public Guid? GetTenantId()
        {
            return _tenantId;
        }

        public void SetTenantId(Guid tenantId)
        {
            _tenantId = tenantId;
        }
    }

    public static class TenantIdResolverExtentions
    {
        public static IServiceCollection ConfigureTenantIdResolver(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddScoped<TenantIdResolver>();
            return serviceCollection;
        }
    }
}
