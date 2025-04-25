using Intaker.TaskManager.Domain;
using Intaker.TaskManager.Domain.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Intaker.TaskManager.Infrastructure.Data
{
    public class TasksDbContext : DbContext
    {
        public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options)
        {
        }
        
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("Tasks");
                
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                
                entity.Property(e => e.Status)
                    .HasConversion<int>();
                
                entity.Property(e => e.CreatedAt).IsRequired();
            });
            
            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.ToTable("OutboxMessages");
                
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
} 