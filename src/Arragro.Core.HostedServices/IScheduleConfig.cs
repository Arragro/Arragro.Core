using System;

namespace Arragro.Core.HostedServices
{
    public interface IScheduleConfig<T>
    {
        string CronExpression { get; set; }
        bool IncludeSeconds { get; set; }
        TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public interface IQueueConfig<T> : IScheduleConfig<T>
    {
        string ConnectionString { get; set; }
        string QueueName { get; set; }
    }

    public class ScheduleConfig<T> : IScheduleConfig<T>
    {
        public string CronExpression { get; set; }
        public bool IncludeSeconds { get; set; } = false;
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public class QueueConfig<T> : ScheduleConfig<T>, IQueueConfig<T>
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}
