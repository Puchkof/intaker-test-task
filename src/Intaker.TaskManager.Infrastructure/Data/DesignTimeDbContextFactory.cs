using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Intaker.TaskManager.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TasksDbContext>
    {
        public TasksDbContext CreateDbContext(string[] args)
        {
            // Get the directory of the current assembly
            var basePath = Directory.GetCurrentDirectory();
            var apiProjectPath = Path.Combine(basePath, "..", "Intaker.TaskManager.Api");
            Console.WriteLine($"API project path: {apiProjectPath}");
            
            // Build the configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"Using connection string: {connectionString}");
            
            // Configure DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<TasksDbContext>();
            optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("Intaker.TaskManager.Infrastructure"));
            
            return new TasksDbContext(optionsBuilder.Options);
        }
    }
} 