using FluentValidation;
using Intaker.TaskManager.Application.Commands;
using Intaker.TaskManager.Domain;

namespace Intaker.TaskManager.Application.Validation.Commands
{
    public class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
    {
        public UpdateTaskStatusCommandValidator()
        {
            RuleFor(x => x.TaskId)
                .GreaterThan(0).WithMessage("Task ID must be greater than 0");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid task status");
        }
    }
}