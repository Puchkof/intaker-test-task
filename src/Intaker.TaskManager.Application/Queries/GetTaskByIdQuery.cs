using System;
using System.Threading;
using System.Threading.Tasks;
using Intaker.TaskManager.Application.DTOs;
using Intaker.TaskManager.Application.Interfaces;
using MediatR;

namespace Intaker.TaskManager.Application.Queries
{
    public record GetTaskByIdQuery(int Id) : IRequest<TaskItemDto>;
    
    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskItemDto>
    {
        private readonly ITaskRepository _taskRepository;
        
        public GetTaskByIdQueryHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        
        public async Task<TaskItemDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (task == null)
            {
                throw new InvalidOperationException($"Task with ID {request.Id} not found");
            }
            
            return TaskItemDto.FromDomain(task);
        }
    }
} 