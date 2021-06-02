using System;

namespace Arragro.Core.HostedServices
{
    public class ScheduledJobStatus
    {
        public bool IsRunning { get; set; }
        public DateTimeOffset RunningDate { get; set; }

        public void SetIsRunning(bool isRunning)
        {
            if (!IsRunning && isRunning)
                RunningDate = DateTimeOffset.UtcNow;
            IsRunning = isRunning;
        }
    }
}
