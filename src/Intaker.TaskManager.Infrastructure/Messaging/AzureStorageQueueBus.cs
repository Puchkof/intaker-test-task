using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Intaker.TaskManager.Application.Interfaces;
using Intaker.TaskManager.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Intaker.TaskManager.Infrastructure.Messaging
{
    public class AzureStorageQueueBus : IMessageBus
    {
        private readonly ILogger<AzureStorageQueueBus> _logger;
        private readonly IQueueClientFactory _queueClientFactory;
        
        public AzureStorageQueueBus(
            IQueueClientFactory queueClientFactory,
            ILogger<AzureStorageQueueBus> logger)
        {
            _queueClientFactory = queueClientFactory;
            _logger = logger;
        }
        
        public async Task PublishAsync<T>(string topicName, T message)
        {
            var queueClient = _queueClientFactory.CreateQueueClient(topicName);
            
            var messageJson = JsonSerializer.Serialize(message);
            
            var base64Message = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(messageJson));
            
            await queueClient.SendMessageAsync(base64Message);
            
            _logger.LogInformation("Message published to queue '{QueueName}': {MessageJson}", queueClient.Name, messageJson);
        }
    }
} 