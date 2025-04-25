using Intaker.TaskManager.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Intaker.TaskManager.Api.Extensions
{
    public static class MigrationExtensions
    {
        public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<TasksDbContext>();
                    dbContext.Database.Migrate();
                    var logger = services.GetRequiredService<ILogger<TasksDbContext>>();
                    logger.LogInformation("Database migrations applied successfully");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<TasksDbContext>>();
                    logger.LogError(ex, "An error occurred while applying migrations");
                }
            }
            
            return app;
        }
    }
} 