using System;
using System.Threading;
using System.Threading.Tasks;
using Intaker.TaskManager.Application.DTOs;
using Intaker.TaskManager.Application.Interfaces;
using Intaker.TaskManager.Domain.Events;
using MediatR;
using TaskStatus = Intaker.TaskManager.Domain.TaskStatus;

namespace Intaker.TaskManager.Application.Commands
{
    public record UpdateTaskStatusCommand(int TaskId, TaskStatus Status) : IRequest<TaskItemDto>;
    
    public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, TaskItemDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IOutboxService _outboxService;
        
        public UpdateTaskStatusCommandHandler(
            ITaskRepository taskRepository,
            IOutboxService outboxService)
        {
            _taskRepository = taskRepository;
            _outboxService = outboxService;
        }
        
        public async Task<TaskItemDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            // Get existing task
            var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
            if (task == null)
            {
                throw new InvalidOperationException($"Task with ID {request.TaskId} not found");
            }
            
            // Update status
            task.UpdateStatus(request.Status);
            
            // Save changes
            var updatedTask = await _taskRepository.UpdateAsync(task, cancellationToken);
            
            // If task was completed, store a message in the outbox
            if (task.Status == TaskStatus.Completed)
            {
                var taskCompletedEvent = new TaskCompletedEvent
                {
                    TaskId = task.Id,
                    CompletedAt = DateTime.UtcNow
                };
                
                await _outboxService.SaveMessageAsync(
                    nameof(TaskCompletedEvent), 
                    taskCompletedEvent, 
                    cancellationToken);
            }
            
            // Return updated task
            return TaskItemDto.FromDomain(updatedTask);
        }
    }
} 