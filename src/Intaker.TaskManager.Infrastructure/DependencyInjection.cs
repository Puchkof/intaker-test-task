using Azure.Messaging.ServiceBus;
using Azure.Storage.Queues;
using Intaker.TaskManager.Application.Interfaces;
using Intaker.TaskManager.Infrastructure.Data;
using Intaker.TaskManager.Infrastructure.Messaging;
using Intaker.TaskManager.Infrastructure.Options;
using Intaker.TaskManager.Infrastructure.Outbox;
using Intaker.TaskManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Intaker.TaskManager.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<TasksDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(TasksDbContext).Assembly.FullName)));
            
            // Register repositories
            services.AddScoped<ITaskRepository, TaskRepository>();
            
            // Register StorageQueueOptions
            services.AddStorageQueueOptions(configuration);
            
            // Register queue client factory
            services.AddSingleton<IQueueClientFactory, QueueClientFactory>();
            
            // Register TaskCompleted queue client
            services.AddSingleton(provider =>
            {
                var factory = provider.GetRequiredService<IQueueClientFactory>();
                return factory.CreateQueueClient("TaskCompleted");
            });
            
            // Register message bus
            services.AddSingleton<IMessageBus, AzureStorageQueueBus>();
            
            // Register outbox service
            services.AddScoped<IOutboxService, OutboxService>();
            
            // Register background services
            services.AddHostedService<OutboxProcessor>();
            services.AddHostedService<TaskCompletionQueueProcessor>();
            
            return services;
        }
    }
} 