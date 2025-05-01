using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arragro.Core.Fastly
{
    public static class FastlyExtensions
    {
        public static IServiceCollection ConfigureFastlyClient(this IServiceCollection serviceCollection, FastlyApiTokens fastlyApiTokens, bool enabled = true)
        {
            serviceCollection.TryAddSingleton(fastlyApiTokens);
            serviceCollection.TryAddSingleton<FastlyHelper>();
            if (fastlyApiTokens.Enabled && enabled)
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