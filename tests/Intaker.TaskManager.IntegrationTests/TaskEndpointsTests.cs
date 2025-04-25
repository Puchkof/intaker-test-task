using Intaker.TaskManager.Api.Routes;
using Intaker.TaskManager.Application.DTOs;
using Intaker.TaskManager.Domain;
using Intaker.TaskManager.Domain.Outbox;
using Intaker.TaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskStatus = Intaker.TaskManager.Domain.TaskStatus;

namespace Intaker.TaskManager.IntegrationTests;

public class TaskEndpointsTests : IntegrationTestBase
{
    public TaskEndpointsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetTasks_ReturnsEmptyList_WhenNoTasksExist()
    {
        // Act
        var tasks = await GetAsync<IEnumerable<TaskItemDto>>(ApiRoutes.Tasks.GetAll);
        
        // Assert
        Assert.NotNull(tasks);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task CreateTask_ReturnsCreatedTask_WithValidData()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto
        {
            Name = "Test Task",
            Description = "This is a test task"
        };
        
        // Act
        var createdTask = await PostAsync<CreateTaskDto, TaskItemDto>(ApiRoutes.Tasks.Create, createTaskDto);
        
        // Assert
        Assert.NotNull(createdTask);
        Assert.Equal(createTaskDto.Name, createdTask.Name);
        Assert.Equal(createTaskDto.Description, createdTask.Description);
        Assert.Equal(TaskStatus.NotStarted, createdTask.Status);
        Assert.NotEqual(0, createdTask.Id);
    }

    [Fact]
    public async Task GetTaskById_ReturnsTask_WhenTaskExists()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto
        {
            Name = "Task to retrieve",
            Description = "This task will be retrieved by ID"
        };
        
        var createdTask = await PostAsync<CreateTaskDto, TaskItemDto>(ApiRoutes.Tasks.Create, createTaskDto);
        
        // Act
        var retrievedTask = await GetAsync<TaskItemDto>($"{ApiRoutes.Tasks.Base}/{createdTask!.Id}");
        
        // Assert
        Assert.NotNull(retrievedTask);
        Assert.Equal(createdTask.Id, retrievedTask.Id);
        Assert.Equal(createdTask.Name, retrievedTask.Name);
        Assert.Equal(createdTask.Description, retrievedTask.Description);
    }

    [Fact]
    public async Task UpdateTaskStatus_ChangesTaskStatus_WithValidData()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto
        {
            Name = "Task to update",
            Description = "This task's status will be updated"
        };
        
        var createdTask = await PostAsync<CreateTaskDto, TaskItemDto>(ApiRoutes.Tasks.Create, createTaskDto);
        
        var updateStatusDto = new UpdateTaskStatusDto
        {
            Status = TaskStatus.InProgress
        };
        
        // Act
        var updatedTask = await PatchAsync<UpdateTaskStatusDto, TaskItemDto>(
            $"{ApiRoutes.Tasks.Base}/{createdTask!.Id}/status", 
            updateStatusDto);
        
        // Assert
        Assert.NotNull(updatedTask);
        Assert.Equal(createdTask.Id, updatedTask.Id);
        Assert.Equal(TaskStatus.InProgress, updatedTask.Status);
    }
    
    [Fact]
    public async Task UpdateTaskStatus_CreatesOutboxMessage_WhenTaskIsCompleted()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto
        {
            Name = "Task for outbox test",
            Description = "This task will be completed to test outbox"
        };
        
        var createdTask = await PostAsync<CreateTaskDto, TaskItemDto>(ApiRoutes.Tasks.Create, createTaskDto);
        
        // First update to InProgress
        var inProgressDto = new UpdateTaskStatusDto
        {
            Status = TaskStatus.InProgress
        };
        
        await PatchAsync<UpdateTaskStatusDto, TaskItemDto>(
            $"{ApiRoutes.Tasks.Base}/{createdTask!.Id}/status", 
            inProgressDto);
        
        // Then update to Completed
        var completedDto = new UpdateTaskStatusDto
        {
            Status = TaskStatus.Completed
        };
        
        // Act
        var updatedTask = await PatchAsync<UpdateTaskStatusDto, TaskItemDto>(
            $"{ApiRoutes.Tasks.Base}/{createdTask.Id}/status", 
            completedDto);
        
        // Assert
        Assert.NotNull(updatedTask);
        Assert.Equal(TaskStatus.Completed, updatedTask.Status);
        
        // Check if an outbox message was created
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TasksDbContext>();
        
        var outboxMessages = await dbContext.OutboxMessages
            .Where(m => m.Type == "TaskCompletedEvent")
            .ToListAsync();
            
        Assert.NotEmpty(outboxMessages);
        var message = outboxMessages.First();
        Assert.Contains(createdTask.Id.ToString(), message.Content);
        Assert.False(string.IsNullOrEmpty(message.Content));
    }
}