using System;

namespace Intaker.TaskManager.Domain.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        
        public OutboxMessage()
        {
            // For EF Core
        }
        
        public OutboxMessage(string type, string content)
        {
            Id = Guid.NewGuid();
            Type = type;
            Content = content;
            CreatedAt = DateTime.UtcNow;
        }
    }
}