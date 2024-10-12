using Arragro.Core.HostedServices;
using Microsoft.Extensions.Logging;

namespace Arragro.Core.HealthCheck.HostedService
{
    public class HealthCheckShedule : CronJobService
    {
        public HealthCheckShedule(
            string cronExpression,
            bool includeSeconds,
            TimeZoneInfo timeZoneInfo,
            ILogger logger,
            string jobName,
            bool runOnStartup = false,
            bool logInfo = true,
            bool logNextOccurance = true) : base(cronExpression, includeSeconds, timeZoneInfo, logger, jobName, runOnStartup, logInfo, logNextOccurance)
        {
        }
    }
}
