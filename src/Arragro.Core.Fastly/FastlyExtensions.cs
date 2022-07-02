using Microsoft.Extensions.DependencyInjection;

namespace Arragro.Core.Fastly
{
    public static class FastlyExtensions
    {
        public static IServiceCollection ConfigureFastlyClient(this IServiceCollection serviceCollection, FastlyApiTokens fastlyApiTokens)
        {
            serviceCollection.AddSingleton(fastlyApiTokens);
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