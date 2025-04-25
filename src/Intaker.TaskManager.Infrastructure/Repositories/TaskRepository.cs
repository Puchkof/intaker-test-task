using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intaker.TaskManager.Application.Interfaces;
using Intaker.TaskManager.Domain;
using Intaker.TaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Intaker.TaskManager.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TasksDbContext _dbContext;
        
        public TaskRepository(TasksDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<TaskItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks.FindAsync(new object[] { id }, cancellationToken);
        }
        
        public async Task<IEnumerable<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks.ToListAsync(cancellationToken);
        }
        
        public async Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return task;
        }
        
        public async Task<TaskItem> UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            _dbContext.Entry(task).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return task;
        }
    }
} 