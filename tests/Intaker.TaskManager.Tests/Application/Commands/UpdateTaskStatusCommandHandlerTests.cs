using System;
using System.Threading;
using System.Threading.Tasks;
using Intaker.TaskManager.Application.Commands;
using Intaker.TaskManager.Application.DTOs;
using Intaker.TaskManager.Application.Interfaces;
using Intaker.TaskManager.Domain;
using Intaker.TaskManager.Domain.Events;
using Moq;
using Xunit;
using TaskStatus = Intaker.TaskManager.Domain.TaskStatus;

namespace Intaker.TaskManager.Tests.Application.Commands
{
    public class UpdateTaskStatusCommandHandlerTests
    {
        private readonly Mock<ITaskRepository> _mockRepository;
        private readonly Mock<IOutboxService> _mockOutboxService;
        private readonly UpdateTaskStatusCommandHandler _handler;
        
        public UpdateTaskStatusCommandHandlerTests()
        {
            _mockRepository = new Mock<ITaskRepository>();
            _mockOutboxService = new Mock<IOutboxService>();
            _handler = new UpdateTaskStatusCommandHandler(_mockRepository.Object, _mockOutboxService.Object);
        }
        
        [Fact]
        public async Task Handle_ShouldUpdateTaskStatus()
        {
            // Arrange
            var taskId = 1;
            var existingTask = new TaskItem("Test Task", "Description") { Id = taskId };
            
            _mockRepository
                .Setup(repo => repo.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTask);
                
            var updatedTask = new TaskItem("Test Task", "Description") { 
                Id = taskId, 
                Status = TaskStatus.InProgress,
                UpdatedAt = DateTime.UtcNow
            };
            
            _mockRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedTask);
                
            var command = new UpdateTaskStatusCommand(taskId, TaskStatus.InProgress);
            
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.Equal(taskId, result.Id);
            Assert.Equal(TaskStatus.InProgress, result.Status);
            Assert.NotNull(result.UpdatedAt);
            
            _mockRepository.Verify(repo => repo.UpdateAsync(
                It.Is<TaskItem>(t => t.Id == taskId && t.Status == TaskStatus.InProgress), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
                
            // Verify outbox service was not called for non-completed status
            _mockOutboxService.Verify(service => 
                service.SaveMessageAsync(
                    It.IsAny<string>(), 
                    It.IsAny<TaskCompletedEvent>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Never);
        }
        
        [Fact]
        public async Task Handle_WhenTaskCompleted_ShouldSaveOutboxMessage()
        {
            // Arrange
            var taskId = 1;
            var existingTask = new TaskItem("Test Task", "Description") { 
                Id = taskId,
                Status = TaskStatus.InProgress 
            };
            
            _mockRepository
                .Setup(repo => repo.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTask);
                
            var updatedTask = new TaskItem("Test Task", "Description") { 
                Id = taskId, 
                Status = TaskStatus.Completed,
                UpdatedAt = DateTime.UtcNow
            };
            
            _mockRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem t, CancellationToken _) => {
                    t.Status = TaskStatus.Completed;
                    t.UpdatedAt = DateTime.UtcNow;
                    return t;
                });
                
            var command = new UpdateTaskStatusCommand(taskId, TaskStatus.Completed);
            
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.Equal(taskId, result.Id);
            Assert.Equal(TaskStatus.Completed, result.Status);
            
            _mockOutboxService.Verify(service => 
                service.SaveMessageAsync(
                    nameof(TaskCompletedEvent),
                    It.Is<TaskCompletedEvent>(e => e.TaskId == taskId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async Task Handle_WhenTaskNotFound_ShouldThrowException()
        {
            // Arrange
            var taskId = 999;
            
            _mockRepository
                .Setup(repo => repo.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem)null);
                
            var command = new UpdateTaskStatusCommand(taskId, TaskStatus.InProgress);
            
            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _handler.Handle(command, CancellationToken.None));
                
            Assert.Contains($"Task with ID {taskId} not found", exception.Message);
            
            _mockRepository.Verify(repo => 
                repo.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), 
                Times.Never);
                
            _mockOutboxService.Verify(service => 
                service.SaveMessageAsync(
                    It.IsAny<string>(), 
                    It.IsAny<object>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Never);
        }
    }
}