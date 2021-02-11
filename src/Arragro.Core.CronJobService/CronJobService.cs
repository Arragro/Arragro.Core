using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.CronJobService
{
    public abstract class CronJobService : IHostedService, IDisposable
    {
        private System.Timers.Timer _timer;
        private readonly CronExpression _expression;
        private readonly TimeZoneInfo _timeZoneInfo;
        private readonly ILogger _logger;

        protected CronJobService(
            string cronExpression, 
            bool includeSeconds, 
            TimeZoneInfo timeZoneInfo,
            ILogger logger)
        {
            _expression = CronExpression.Parse(cronExpression, includeSeconds ? CronFormat.IncludeSeconds : CronFormat.Standard);
            _timeZoneInfo = timeZoneInfo;
            _logger = logger;
        }

        public virtual async Task StartAsync(string purpose, CancellationToken cancellationToken)
        {
            await ScheduleJob(purpose, cancellationToken);
        }

        protected virtual async Task ScheduleJob(string purpose, CancellationToken cancellationToken)
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
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        _logger.LogInformation($"Starting {purpose}");

                        await DoWork(cancellationToken);

                        stopwatch.Stop();
                        _logger.LogInformation($"Completed {purpose} in {stopwatch.ElapsedMilliseconds}ms");
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await ScheduleJob(purpose, cancellationToken);    // reschedule next
                    }
                };
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
