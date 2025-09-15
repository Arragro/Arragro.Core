using Arragro.Core.HostedServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Arragro.Core.HealthCheck.HostedService
{
    public static class HealthCheckExtensions
    {
        public static WebApplication ConfigureHealthCheckEndpoint(this WebApplication application, IDictionary<HealthStatus, int> resultStatusCodes, string pattern = "/hc")
        {
            application.MapGet("/hc", async (ILogger<WebApplication> logger, IMemoryCache memoryCache, HealthCheckService healthCheckService, HttpContext http) =>
            {
                try
                {
                    http.Response.Headers.CacheControl = $"public,max-age=0";
                    var data = memoryCache.Get<Arragro.Core.HealthCheck.HostedService.HealthCheckResult>("health-check");
                    if (data == null)
                    {
                        data = new Arragro.Core.HealthCheck.HostedService.HealthCheckResult(await healthCheckService.CheckHealthAsync());
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
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Something went wrong building health check");
                    return Results.BadRequest();
                }
            });

            application.MapGet("/hc-detailed", async (IMemoryCache memoryCache, HealthCheckService healthCheckService, HttpContext http) =>
            {
                http.Response.Headers.CacheControl = $"public,max-age=0";
                var data = memoryCache.Get<Arragro.Core.HealthCheck.HostedService.HealthCheckResult>("health-check");
                if (data == null)
                {
                    data = new Arragro.Core.HealthCheck.HostedService.HealthCheckResult(await healthCheckService.CheckHealthAsync());
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
