using Microsoft.Extensions.DependencyInjection;
using System;

namespace Arragro.Core.HostedServices
{
    public static class QueueServiceExtensions
    {
        public static IServiceCollection AddQueueJob<T>(this IServiceCollection services, string cronExpression = null, bool? includeSeconds = null) where T : QueueJobService
        {
            var config = new ScheduleConfig<T>
            {
                CronExpression = cronExpression ?? "*/30 * * * * *",
                IncludeSeconds = includeSeconds ?? true,
                TimeZoneInfo = TimeZoneInfo.Utc
            };

            services.AddSingleton<IScheduleConfig<T>>(config);
            services.AddHostedService<T>();
            return services;
        }
    }
}
