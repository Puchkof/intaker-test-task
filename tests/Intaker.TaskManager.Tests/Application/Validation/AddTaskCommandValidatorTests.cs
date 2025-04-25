using FluentValidation.TestHelper;
using Intaker.TaskManager.Application.Commands;
using Intaker.TaskManager.Application.Validation.Commands;
using Xunit;

namespace Intaker.TaskManager.Tests.Application.Validation
{
    public class AddTaskCommandValidatorTests
    {
        private readonly AddTaskCommandValidator _validator;
        
        public AddTaskCommandValidatorTests()
        {
            _validator = new AddTaskCommandValidator();
        }
        
        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Specified()
        {
            // Arrange
            var command = new AddTaskCommand("Test Task", "Description");
            
            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
        
        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            // Arrange
            var command = new AddTaskCommand("", "Description");
            
            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }
        
        [Fact]
        public void Should_Have_Error_When_Name_Exceeds_Max_Length()
        {
            // Arrange
            var longName = new string('A', 101); // 101 characters
            var command = new AddTaskCommand(longName, "Description");
            
            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }
        
        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Valid()
        {
            // Arrange
            var command = new AddTaskCommand("Name", "Valid description");
            
            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }
        
        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_Max_Length()
        {
            // Arrange
            var longDescription = new string('A', 501); // 501 characters
            var command = new AddTaskCommand("Name", longDescription);
            
            // Act & Assert
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
    }
}