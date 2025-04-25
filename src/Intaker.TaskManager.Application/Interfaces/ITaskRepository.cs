using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intaker.TaskManager.Domain;

namespace Intaker.TaskManager.Application.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default);
        Task<TaskItem> UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);
    }
} 