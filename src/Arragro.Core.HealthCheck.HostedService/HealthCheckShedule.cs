using Arragro.Core.HostedServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Arragro.Core.HealthCheck.HostedService
{
    public class HealthCheckSchedule : CronJobService
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly IMemoryCache _memoryCache;
        private static bool IsRunning = false;

        public HealthCheckSchedule(
            IScheduleConfig<HealthCheckSchedule> config,
            ILogger<HealthCheckSchedule> logger,
            HealthCheckService healthCheckService,
            IMemoryCache memoryCache) : base(config.CronExpression, config.IncludeSeconds, config.TimeZoneInfo, logger, nameof(HealthCheckSchedule))
        {
            _healthCheckService = healthCheckService;
            _memoryCache = memoryCache;
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                if (!IsRunning)
                {
                    IsRunning = true;
                    var report = await _healthCheckService.CheckHealthAsync(cancellationToken);
                    _memoryCache.Set("health-check", new HealthCheckResult(report));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong with Uploading analytics data");
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}
