using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Intaker.TaskManager.Infrastructure.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Intaker.TaskManager.Infrastructure.Messaging
{
    public class TaskCompletionQueueProcessor : BackgroundService
    {
        private readonly ILogger<TaskCompletionQueueProcessor> _logger;
        private readonly QueueClient _queueClient;
        private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(10);
        
        public TaskCompletionQueueProcessor(
            QueueClient queueClient,
            ILogger<TaskCompletionQueueProcessor> logger)
        {
            _logger = logger;
            _queueClient = queueClient;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting the task completion queue processor");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var messages = await _queueClient.ReceiveMessagesAsync(
                        maxMessages: 10, 
                        visibilityTimeout: TimeSpan.FromSeconds(30),
                        cancellationToken: stoppingToken);
                        
                    foreach (var message in messages.Value)
                    {
                        try
                        {
                            var messageBodyBytes = Convert.FromBase64String(message.MessageText);
                            var messageBody = Encoding.UTF8.GetString(messageBodyBytes);
                            
                            _logger.LogInformation("Received task completion message: {MessageBody}", messageBody);
                            
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing queue message {MessageId}", message.MessageId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing messages from queue");
                }
                
                await Task.Delay(_pollingInterval, stoppingToken);
            }
            
            _logger.LogInformation("Task completion queue processor stopped");
        }
        
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stopping the task completion queue processor");
            return base.StopAsync(stoppingToken);
        }
    }
} 