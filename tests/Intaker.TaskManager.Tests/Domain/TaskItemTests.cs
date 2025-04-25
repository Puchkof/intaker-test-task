using System;
using Intaker.TaskManager.Domain;
using Xunit;
using TaskStatus = Intaker.TaskManager.Domain.TaskStatus;

namespace Intaker.TaskManager.Tests.Domain
{
    public class TaskItemTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var task = new TaskItem();
            
            // Assert
            Assert.Equal(TaskStatus.NotStarted, task.Status);
            Assert.NotEqual(default, task.CreatedAt);
            Assert.Null(task.UpdatedAt);
        }
        
        [Fact]
        public void Constructor_WithNameAndDescription_ShouldSetProperties()
        {
            // Arrange
            string name = "Test Task";
            string description = "Test Description";
            
            // Act
            var task = new TaskItem(name, description);
            
            // Assert
            Assert.Equal(name, task.Name);
            Assert.Equal(description, task.Description);
            Assert.Equal(TaskStatus.NotStarted, task.Status);
        }
        
        [Fact]
        public void UpdateStatus_FromNotStartedToInProgress_ShouldSucceed()
        {
            // Arrange
            var task = new TaskItem("Test", "Description");
            
            // Act
            task.UpdateStatus(TaskStatus.InProgress);
            
            // Assert
            Assert.Equal(TaskStatus.InProgress, task.Status);
            Assert.NotNull(task.UpdatedAt);
        }
        
        [Fact]
        public void UpdateStatus_FromInProgressToCompleted_ShouldSucceed()
        {
            // Arrange
            var task = new TaskItem("Test", "Description");
            task.UpdateStatus(TaskStatus.InProgress);
            
            // Act
            task.UpdateStatus(TaskStatus.Completed);
            
            // Assert
            Assert.Equal(TaskStatus.Completed, task.Status);
        }
        
        [Fact]
        public void UpdateStatus_FromNotStartedToCompleted_ShouldThrowException()
        {
            // Arrange
            var task = new TaskItem("Test", "Description");
            
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => task.UpdateStatus(TaskStatus.Completed));
            Assert.Contains("Cannot transition from", exception.Message);
        }
        
        [Fact]
        public void UpdateStatus_FromCompletedToInProgress_ShouldThrowException()
        {
            // Arrange
            var task = new TaskItem("Test", "Description");
            task.UpdateStatus(TaskStatus.InProgress);
            task.UpdateStatus(TaskStatus.Completed);
            
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => task.UpdateStatus(TaskStatus.InProgress));
            Assert.Contains("Cannot change status of a completed task", exception.Message);
        }
        
        [Fact]
        public void UpdateStatus_ToSameStatus_ShouldNotThrowException()
        {
            // Arrange
            var task = new TaskItem("Test", "Description");
            
            // Act & Assert
            var exception = Record.Exception(() => task.UpdateStatus(TaskStatus.NotStarted));
            Assert.Null(exception);
        }
    }
} 