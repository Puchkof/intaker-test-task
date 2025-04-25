using FluentValidation;
using Intaker.TaskManager.Application.DTOs;
using Intaker.TaskManager.Domain;

namespace Intaker.TaskManager.Application.Validation
{
    public class UpdateTaskStatusDtoValidator : AbstractValidator<UpdateTaskStatusDto>
    {
        public UpdateTaskStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid task status");
        }
    }
} 