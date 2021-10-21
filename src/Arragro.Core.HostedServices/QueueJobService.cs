using Arragro.Core.Common.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.HostedServices
{

    public abstract class QueueJobService : CronJobService
    {
        private readonly QueueClient _queueClient;
        private readonly QueueClient _queueClientFailure;
        private readonly string _queueName;
        private static IDictionary<string, int> _failures = new Dictionary<string, int>();

        protected QueueJobService(
            string queueName,
            string connectionString,
            string cronExpression,
            bool includeSeconds,
            TimeZoneInfo timeZoneInfo,
            ILogger<QueueJobService> logger) : base (cronExpression, includeSeconds, timeZoneInfo, logger, queueName)
        {
            _queueClient = new QueueClient(connectionString, queueName);
            _queueClientFailure = new QueueClient(connectionString, $"{queueName}-failures");

            _queueClient.CreateIfNotExists();
            _queueClientFailure.CreateIfNotExists();

            _queueName = queueName;

            logger.LogInformation($"Configured Queue Job for: {queueName}");
        }

        protected override async Task ScheduleJob(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"{_queueName} QueueJobService Scheduled Job running for {_queueName}");
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
                                    _logger.LogDebug($"{_queueName} There are {receivedMessages.Length} messages to process");
                                    foreach (var message in receivedMessages)
                                    {
                                        _logger.LogDebug($"{_queueName} De-queued message: '{message.MessageId}'");

                                        try
                                        {
                                            await DoWork(message, cancellationToken);

                                            // Delete the message
                                            _queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
                                        }
                                        catch (Exception ex)
                                        {
                                            if (_failures.ContainsKey(message.MessageId))
                                                _failures[message.MessageId] = _failures[message.MessageId] + 1;
                                            else
                                                _failures.Add(message.MessageId, 1);
                                            if (_failures[message.MessageId] > 5)
                                            {
                                                var response = await _queueClientFailure.SendMessageAsync(message.MessageText);
                                                _logger.LogError(ex, $"{_queueName} failed to process message {message.MessageId}, moved to failure queue with MessageId {response.Value.MessageId}.");
                                                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                                                _failures.Remove(message.MessageId);
                                            }
                                            else
                                            {
                                                _logger.LogError(ex, $"{_queueName} failed to process message {message.MessageId}.");
                                            }
                                        }
                                    }

                                    receivedMessages = await _queueClient.ReceiveMessagesAsync(20, cancellationToken: cancellationToken);
                                } while (receivedMessages.Length > 0);
                            }
                        }
                        else
                        {
                            _logger.LogDebug($"There are no messages to process");
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
            _logger.LogInformation($"{_queueName} - De-queued message: '{message.MessageText}'");
            await Task.Delay(5000, cancellationToken);  // do the work
        }
    }
}
