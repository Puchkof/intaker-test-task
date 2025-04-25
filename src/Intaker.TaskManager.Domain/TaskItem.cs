using System;

namespace Intaker.TaskManager.Domain
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public TaskItem()
        {
            CreatedAt = DateTime.UtcNow;
            Status = TaskStatus.NotStarted;
        }
        
        public TaskItem(string name, string description) : this()
        {
            Name = name;
            Description = description;
        }
        
        public void UpdateStatus(TaskStatus newStatus)
        {
            // Validate status transition
            ValidateStatusTransition(newStatus);
            
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }
        
        private void ValidateStatusTransition(TaskStatus newStatus)
        {
            // Only allow valid status transitions
            if (Status == newStatus)
                return;
                
            switch (Status)
            {
                case TaskStatus.NotStarted:
                    if (newStatus != TaskStatus.InProgress)
                        throw new InvalidOperationException($"Cannot transition from {Status} to {newStatus}. Task must be set to InProgress first.");
                    break;
                case TaskStatus.InProgress:
                    if (newStatus != TaskStatus.Completed)
                        throw new InvalidOperationException($"Cannot transition from {Status} to {newStatus}. Task in progress can only be completed.");
                    break;
                case TaskStatus.Completed:
                    throw new InvalidOperationException("Cannot change status of a completed task.");
            }
        }
    }
} 