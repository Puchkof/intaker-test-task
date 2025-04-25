using System.Collections.Generic;

namespace Intaker.TaskManager.Infrastructure.Options
{
    public class StorageQueueOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        
        public Dictionary<string, QueueOptions> Topics { get; set; } = new Dictionary<string, QueueOptions>();
        
        public class QueueOptions
        {
            public string Name { get; set; } = string.Empty;
            public string? SubscriptionName { get; set; }
            public int MaxConcurrentCalls { get; set; } = 1;
            public bool AutoCompleteMessages { get; set; } = true;
        }
    }
} 