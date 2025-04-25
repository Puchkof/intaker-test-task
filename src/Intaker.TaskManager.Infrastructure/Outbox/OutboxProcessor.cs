using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Intaker.TaskManager.Application.Interfaces;
using Intaker.TaskManager.Domain.Events;
using Intaker.TaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Intaker.TaskManager.Infrastructure.Outbox
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxProcessor> _logger;
        private readonly TimeSpan _processInterval = TimeSpan.FromSeconds(10);

        public OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox processor started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessOutboxMessages(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox messages");
                    // Not rethrowing since we want the background service to continue
                }

                await Task.Delay(_processInterval, stoppingToken);
            }
        }

        private async Task ProcessOutboxMessages(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TasksDbContext>();
            var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

            var messages = await dbContext.OutboxMessages
                .Where(m => m.ProcessedAt == null)
                .OrderBy(m => m.CreatedAt)
                .Take(20)
                .ToListAsync(stoppingToken);

            if (!messages.Any())
            {
                return;
            }

            foreach (var message in messages)
            {
                try
                {
                    switch (message.Type)
                    {
                        case nameof(TaskCompletedEvent):
                            var taskCompletedEvent = JsonSerializer.Deserialize<TaskCompletedEvent>(message.Content);
                            await messageBus.PublishAsync("TaskCompleted", taskCompletedEvent);
                            break;
                            
                        default:
                            _logger.LogWarning("Unknown message type: {MessageType}", message.Type);
                            break;
                    }

                    message.ProcessedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    // Log but continue with other messages - this is appropriate for a background processor
                    _logger.LogError(ex, "Error processing outbox message {MessageId}", message.Id);
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }
} 