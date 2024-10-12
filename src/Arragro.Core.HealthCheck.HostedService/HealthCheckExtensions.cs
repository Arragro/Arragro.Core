using Arragro.Core.HostedServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Arragro.Core.HealthCheck.HostedService
{
    public static class HealthCheckExtensions
    {
        public static WebApplication ConfigureHealthCheckEndpoint(this WebApplication application, IDictionary<HealthStatus, int> resultStatusCodes, string pattern = "/hc")
        {
            application.MapGet("/hc", async (IMemoryCache memoryCache, HealthCheckService healthCheckService) =>
            {
                var data = memoryCache.Get<HealthCheckResult>("health-check");
                if (data == null)
                {
                    data = new HealthCheckResult(await healthCheckService.CheckHealthAsync());
                    memoryCache.Set("health-check", data);
                }
                if (resultStatusCodes.ContainsKey(data.HealthReport.Status))
                {
                    if (resultStatusCodes[data.HealthReport.Status] == StatusCodes.Status200OK)
                    {
                        return Results.Ok();
                    }
                    return Results.StatusCode(resultStatusCodes[data.HealthReport.Status]);
                }
                return Results.Ok();
            });

            application.MapGet("/hc-detailed", async (IMemoryCache memoryCache, HealthCheckService healthCheckService) =>
            {
                var data = memoryCache.Get<HealthCheckResult>("health-check");
                if (data == null)
                {
                    data = new HealthCheckResult(await healthCheckService.CheckHealthAsync());
                    memoryCache.Set("health-check", data);
                }
                return Results.Ok(data);
            });
            return application;
        }

        public static IServiceCollection ConfigureHealthCheckSchedule(this IServiceCollection serviceCollection, string cronExpression = @"*/5 * * * * *")
        {
            serviceCollection.AddCronJob<HealthCheckSchedule>(options =>
                {
                    options.TimeZoneInfo = TimeZoneInfo.Utc;
                    options.IncludeSeconds = true;
                    options.CronExpression = @"*/5 * * * * *";
                });
            return serviceCollection;
        }
    }
}
