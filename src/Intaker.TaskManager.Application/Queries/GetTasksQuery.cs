using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intaker.TaskManager.Application.DTOs;
using Intaker.TaskManager.Application.Interfaces;
using MediatR;

namespace Intaker.TaskManager.Application.Queries
{
    public record GetTasksQuery : IRequest<IEnumerable<TaskItemDto>>;
    
    public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, IEnumerable<TaskItemDto>>
    {
        private readonly ITaskRepository _taskRepository;
        
        public GetTasksQueryHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        
        public async Task<IEnumerable<TaskItemDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _taskRepository.GetAllAsync(cancellationToken);
            
            return tasks.Select(task => TaskItemDto.FromDomain(task));
        }
    }
} 