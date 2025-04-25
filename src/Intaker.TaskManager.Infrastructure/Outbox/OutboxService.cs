using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Intaker.TaskManager.Application.Interfaces;
using Intaker.TaskManager.Domain.Outbox;
using Intaker.TaskManager.Infrastructure.Data;

namespace Intaker.TaskManager.Infrastructure.Outbox
{
    public class OutboxService : IOutboxService
    {
        private readonly TasksDbContext _dbContext;

        public OutboxService(TasksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveMessageAsync<T>(string messageType, T message, CancellationToken cancellationToken = default)
        {
            var serializedMessage = JsonSerializer.Serialize(message);
            
            var outboxMessage = new OutboxMessage(messageType, serializedMessage);
            
            await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
} 