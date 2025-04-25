using FluentValidation.TestHelper;
using Intaker.TaskManager.Application.Commands;
using Intaker.TaskManager.Application.Validation.Commands;
using Intaker.TaskManager.Domain;
using Xunit;
using TaskStatus = Intaker.TaskManager.Domain.TaskStatus;

namespace Intaker.TaskManager.Tests.Application.Validation
{
    public class UpdateTaskStatusCommandValidatorTests
    {
        private readonly UpdateTaskStatusCommandValidator _validator;
        
        public UpdateTaskStatusCommandValidatorTests()
        {
            _validator = new UpdateTaskStatusCommandValidator();
        }
        
        [Fact]
        public void Should_Not_Have_Error_When_TaskId_Is_Valid()
        {
            // Arrange
            var command = new UpdateTaskStatusCommand(1, TaskStatus.InProgress);
            
            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.TaskId);
        }
        
        [Fact]
        public void Should_Have_Error_When_TaskId_Is_Invalid()
        {
            // Arrange
            var command = new UpdateTaskStatusCommand(0, TaskStatus.InProgress);
            
            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.TaskId);
        }
        
        [Fact]
        public void Should_Not_Have_Error_When_Status_Is_Valid()
        {
            // Arrange
            var command = new UpdateTaskStatusCommand(1, TaskStatus.InProgress);
            
            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Status);
        }
        
        [Fact]
        public void Should_Have_Error_When_Status_Is_Invalid()
        {
            // Arrange
            var command = new UpdateTaskStatusCommand(1, (TaskStatus)99); // Invalid enum value
            
            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Status);
        }
    }
}