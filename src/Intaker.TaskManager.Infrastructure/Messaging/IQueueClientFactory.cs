using Azure.Storage.Queues;

namespace Intaker.TaskManager.Infrastructure.Messaging
{
    public interface IQueueClientFactory
    {
        QueueClient CreateQueueClient(string topicName);
    }
} 