using System;
using System.Linq;
using System.Threading.Tasks;
using Intaker.TaskManager.Api;
using Intaker.TaskManager.Domain.Outbox;
using Intaker.TaskManager.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

namespace Intaker.TaskManager.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:14")
        .WithDatabase("integration_test_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithCleanUp(true)
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Find and remove the default DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TasksDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add a database context using the test container connection string
            services.AddDbContext<TasksDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            // Create and migrate the database
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<TasksDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory>>();

            try
            {
                // Create the database and tables
                db.Database.EnsureCreated();
                
                // Add initial test data if needed
                if (!db.Tasks.Any())
                {
                    logger.LogInformation("Database is empty. Adding initial test data...");
                    
                    // The test data will be added by the tests themselves
                }

                // Ensure OutboxMessages table exists
                if (db.Model.FindEntityType(typeof(OutboxMessage)) == null)
                {
                    logger.LogWarning("OutboxMessage entity not found in the model!");
                }
                else
                {
                    logger.LogInformation("OutboxMessage entity found in the model.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while setting up the test database. Error: {Message}", 
                    ex.Message);
            }
        });
    }
}