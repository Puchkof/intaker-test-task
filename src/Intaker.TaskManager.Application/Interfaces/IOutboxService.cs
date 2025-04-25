using System.Threading;
using System.Threading.Tasks;

namespace Intaker.TaskManager.Application.Interfaces
{
    public interface IOutboxService
    {
        Task SaveMessageAsync<T>(string messageType, T message, CancellationToken cancellationToken = default);
    }
}