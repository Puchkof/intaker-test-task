using System.Threading.Tasks;

namespace Intaker.TaskManager.Application.Interfaces
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(string topicName, T message);
    }
} 