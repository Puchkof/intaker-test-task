using System;

namespace Intaker.TaskManager.Domain.Events
{
    public class TaskCompletedEvent
    {
        public int TaskId { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}