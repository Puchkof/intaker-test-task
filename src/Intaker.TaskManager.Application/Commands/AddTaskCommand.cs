using System.Threading;
using System.Threading.Tasks;
using Intaker.TaskManager.Application.DTOs;
using Intaker.TaskManager.Application.Interfaces;
using Intaker.TaskManager.Domain;
using MediatR;

namespace Intaker.TaskManager.Application.Commands
{
    public record AddTaskCommand(string Name, string Description) : IRequest<TaskItemDto>;
    
    public class AddTaskCommandHandler : IRequestHandler<AddTaskCommand, TaskItemDto>
    {
        private readonly ITaskRepository _taskRepository;
        
        public AddTaskCommandHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        
        public async Task<TaskItemDto> Handle(AddTaskCommand request, CancellationToken cancellationToken)
        {
            // Create entity
            var task = new TaskItem(request.Name, request.Description);
            
            // Save to repository
            var createdTask = await _taskRepository.AddAsync(task, cancellationToken);
            
            // Return DTO
            return TaskItemDto.FromDomain(createdTask);
        }
    }
} 