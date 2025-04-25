using Azure.Storage.Queues;
using Intaker.TaskManager.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System;

namespace Intaker.TaskManager.Infrastructure.Messaging
{
    public class QueueClientFactory : IQueueClientFactory
    {
        private readonly StorageQueueOptions _options;

        public QueueClientFactory(IOptions<StorageQueueOptions> options)
        {
            _options = options.Value;
        }

        public QueueClient CreateQueueClient(string topicName)
        {
            string queueName = topicName;
            if (_options.Topics.TryGetValue(topicName, out var topicOptions))
            {
                queueName = topicOptions.Name;
            }
            else
            {
                throw new InvalidOperationException($"Queue configuration not found for topic: {topicName}");
            }

            var queueClient = new QueueClient(_options.ConnectionString, queueName);
            queueClient.CreateIfNotExists();
            
            return queueClient;
        }
    }
} 