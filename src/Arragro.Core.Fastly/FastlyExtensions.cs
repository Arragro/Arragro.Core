using Microsoft.Extensions.DependencyInjection;

namespace Arragro.Core.Fastly
{
    public static class FastlyExtensions
    {
        public static IServiceCollection ConfigureFastlyClient(this IServiceCollection serviceCollection, FastlyApiTokens fastlyApiTokens, bool enabled = true)
        {
            serviceCollection.AddSingleton(fastlyApiTokens);
            serviceCollection.AddSingleton<FastlyHelper>();
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