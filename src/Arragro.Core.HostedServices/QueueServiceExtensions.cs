using Microsoft.Extensions.DependencyInjection;
using System;

namespace Arragro.Core.HostedServices
{
    public static class QueueServiceExtensions
    {
        public static IServiceCollection AddQueueJob<T>(
            this IServiceCollection services, 
            string connectionString,
            string queueName,
            string cronExpression = null, 
            bool? includeSeconds = null,
            int maxMessages = 20,
            bool? deleteOnCompltion = null) where T : QueueJobService
        {
            var config = new QueueConfig<T>
            {
                ConnectionString = connectionString,
                QueueName = queueName,
                CronExpression = cronExpression ?? "*/30 * * * * *",
                IncludeSeconds = includeSeconds ?? true,
                TimeZoneInfo = TimeZoneInfo.Utc,
                MaxMessages = maxMessages,
                DeleteOnCompletion = deleteOnCompltion ?? false
            };

            services.AddSingleton<IQueueConfig<T>>(config);
            services.AddHostedService<T>();
            return services;
        }
    }
}
