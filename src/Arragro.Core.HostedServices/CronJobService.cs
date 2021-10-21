using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.HostedServices
{
    public abstract class CronJobService : IHostedService, IDisposable
    {
        protected System.Timers.Timer _timer;
        protected readonly CronExpression _expression;
        protected readonly TimeZoneInfo _timeZoneInfo;
        protected readonly ILogger _logger;
        private readonly string _jobName;
        private readonly bool _logNextOccurance;

        protected CronJobService(
            string cronExpression,
            bool includeSeconds,
            TimeZoneInfo timeZoneInfo,
            ILogger logger,
            string jobName,
            bool logInfo = true,
            bool logNextOccurance = true)
        {
            _expression = CronExpression.Parse(cronExpression, includeSeconds ? CronFormat.IncludeSeconds : CronFormat.Standard);
            _timeZoneInfo = timeZoneInfo;
            _logger = logger;
            _jobName = jobName;
            _logNextOccurance = logNextOccurance;
            if (logInfo)
            {
                var nextOccurrences = _expression.GetOccurrences(DateTime.UtcNow, DateTime.UtcNow.AddDays(3));
                logger.LogInformation($"Configured Cron Job for: {_jobName}, next occurrences: {string.Join(", ", nextOccurrences.Take(5).Select(x => x.ToString("yyyy-MM-ddTHH:mm:ss")))}");
            }
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await ScheduleJob(cancellationToken);
        }

        protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
        {
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();  // reset and dispose timer
                    _timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await DoWork(cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await ScheduleJob(cancellationToken);    // reschedule next
                    }
                };
                
                if (_logNextOccurance)
                    _logger.LogInformation($"Timer for: {_jobName}, next occurrance: {next.Value.ToString("yyyy-MM-ddTHH:mm:ss")}");
                else
                    _logger.LogDebug($"Timer for: {_jobName}, next occurrance: {next.Value.ToString("yyyy-MM-ddTHH:mm:ss")}");

                _timer.Start();
            }
            await Task.CompletedTask;
        }

        public virtual async Task DoWork(CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken);  // do the work
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();
            await Task.CompletedTask;
        }

        public virtual void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
