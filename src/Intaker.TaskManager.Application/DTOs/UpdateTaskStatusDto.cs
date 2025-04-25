using Intaker.TaskManager.Domain;

namespace Intaker.TaskManager.Application.DTOs
{
    public class UpdateTaskStatusDto
    {
        public Domain.TaskStatus Status { get; set; }
    }
} 