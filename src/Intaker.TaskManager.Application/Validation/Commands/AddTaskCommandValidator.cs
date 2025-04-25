using FluentValidation;
using Intaker.TaskManager.Application.Commands;

namespace Intaker.TaskManager.Application.Validation.Commands
{
    public class AddTaskCommandValidator : AbstractValidator<AddTaskCommand>
    {
        public AddTaskCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Task name is required")
                .MaximumLength(100).WithMessage("Task name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}