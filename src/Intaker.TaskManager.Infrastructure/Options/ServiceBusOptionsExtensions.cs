using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Intaker.TaskManager.Infrastructure.Options
{
    public static class StorageQueueOptionsExtensions
    {
        public static IServiceCollection AddStorageQueueOptions(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the StorageQueueOptions from configuration
            services.Configure<StorageQueueOptions>(options => 
            {
                // Set the connection string from configuration
                options.ConnectionString = configuration.GetConnectionString("ServiceBus") ?? string.Empty;
            });
            
            // Bind the Topics section from configuration
            services.Configure<StorageQueueOptions>(configuration.GetSection("ServiceBus"));
            
            return services;
        }
    }
} 