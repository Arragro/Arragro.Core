using System;

namespace Arragro.Core.CronJobService
{
    public interface IScheduleConfig<T>
    {
        string CronExpression { get; set; }
        bool IncludeSeconds { get; set; }
        TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public class ScheduleConfig<T> : IScheduleConfig<T>
    {
        public string CronExpression { get; set; }
        public bool IncludeSeconds { get; set; } = false;
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }
}
