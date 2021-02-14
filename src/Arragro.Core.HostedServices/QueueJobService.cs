using Arragro.Core.Common.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.HostedServices
{

    public abstract class QueueJobService : CronJobService
    {
        private readonly QueueJobServiceConfiguration _queueJobServiceConfiguration;
        private readonly QueueClient _queueClient;

        protected QueueJobService(
            string cronExpression,
            bool includeSeconds,
            TimeZoneInfo timeZoneInfo,
            QueueJobServiceConfiguration queueJobServiceConfiguration,
            ILogger logger) : base (cronExpression, includeSeconds, timeZoneInfo, logger)
        {
            _queueJobServiceConfiguration = queueJobServiceConfiguration;
            _queueClient = new QueueClient(_queueJobServiceConfiguration.ConnectionString, _queueJobServiceConfiguration.QueueName);
        }

        protected override async Task ScheduleJob(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Scheduled Job running");
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
                        if (await _queueClient.ExistsAsync())
                        {
                            // Receive and process 20 messages
                            QueueMessage[] receivedMessages = await _queueClient.ReceiveMessagesAsync(20, cancellationToken: cancellationToken);
                            if (receivedMessages.Length > 0)
                            {
                                do
                                {
                                    _logger.LogInformation($"There are {receivedMessages.Length} messages to process");
                                    foreach (var message in receivedMessages)
                                    {
                                        _logger.LogInformation($"De-queued message: '{message.MessageId}'");
                                        _logger.LogDebug($"De-queued message: '{message.MessageText}'");

                                        await DoWork(message, cancellationToken);

                                        // Delete the message
                                        _queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
                                    }

                                    receivedMessages = await _queueClient.ReceiveMessagesAsync(20, cancellationToken: cancellationToken);
                                } while (receivedMessages.Length > 0);
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"There are no messages to process");
                        }
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await ScheduleJob(cancellationToken);    // reschedule next
                    }
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            throw new Exception("Don't use this method here.");
        }

        public virtual async Task DoWork(QueueMessage message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"De-queued message: '{message.MessageText}'");
            await Task.Delay(5000, cancellationToken);  // do the work
        }
    }
}
