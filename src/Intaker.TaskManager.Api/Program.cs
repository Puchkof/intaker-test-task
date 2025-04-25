using Intaker.TaskManager.Api.Extensions;
using Intaker.TaskManager.Api.Middleware;
using Intaker.TaskManager.Application;
using Intaker.TaskManager.Application.Validation;
using Intaker.TaskManager.Infrastructure;
using Intaker.TaskManager.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskDtoValidator>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Intaker Task Manager API",
        Version = "v1",
        Description = "API for managing tasks with Azure Service Bus integration"
    });
});

var app = builder.Build();

// Apply database migrations
app.ApplyMigrations();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

// Register API endpoints
app.MapTaskEndpoints();

app.Run();

public partial class Program { }
