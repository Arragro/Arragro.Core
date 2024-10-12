using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace Arragro.Core.HealthCheck.HostedService
{
    public class HealthCheckResult
    {
        public DateTimeOffset DateRan { get; set; } = DateTimeOffset.UtcNow;
        public HealthReport HealthReport { get; set; }

        public HealthCheckResult(HealthReport healthReport)
        {
            HealthReport = healthReport;

        }
    }
}
