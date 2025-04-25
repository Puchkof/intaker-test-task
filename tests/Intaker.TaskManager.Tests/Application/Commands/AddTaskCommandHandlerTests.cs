using System.Threading;
using System.Threading.Tasks;
using Intaker.TaskManager.Application.Commands;
using Intaker.TaskManager.Application.DTOs;
using Intaker.TaskManager.Application.Interfaces;
using Intaker.TaskManager.Domain;
using Moq;
using Xunit;
using TaskStatus = Intaker.TaskManager.Domain.TaskStatus;

namespace Intaker.TaskManager.Tests.Application.Commands
{
    public class AddTaskCommandHandlerTests
    {
        private readonly Mock<ITaskRepository> _mockRepository;
        private readonly AddTaskCommandHandler _handler;
        
        public AddTaskCommandHandlerTests()
        {
            _mockRepository = new Mock<ITaskRepository>();
            _handler = new AddTaskCommandHandler(_mockRepository.Object);
        }
        
        [Fact]
        public async Task Handle_ShouldCreateTaskWithCorrectProperties()
        {
            // Arrange
            var command = new AddTaskCommand("Test Task", "Task Description");
            
            var savedTask = new TaskItem("Test Task", "Task Description") { Id = 1 };
            
            _mockRepository
                .Setup(repo => repo.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(savedTask);
            
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Task", result.Name);
            Assert.Equal("Task Description", result.Description);
            Assert.Equal(TaskStatus.NotStarted, result.Status);
            
            _mockRepository.Verify(repo => repo.AddAsync(
                It.Is<TaskItem>(t => 
                    t.Name == command.Name && 
                    t.Description == command.Description &&
                    t.Status == TaskStatus.NotStarted), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
} 