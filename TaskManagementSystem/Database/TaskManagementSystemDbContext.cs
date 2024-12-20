using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database.Models;

using TaskManagementSystem.Extensions;

namespace TaskManagementSystem.Database
{
    public class TaskManagementSystemDbContext : DbContext
    {
        public TaskManagementSystemDbContext(DbContextOptions<TaskManagementSystemDbContext> options) : base(options) { }

        public DbSet<TaskEntity> Tasks { get; set; } = default!;
        public DbSet<CategoryEntity> Categories { get; set; } = default!;
        public DbSet<DeletedTaskEntity> DeletedTasks { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskEntity>().ConfigureTaskEntity();
            modelBuilder.Entity<DeletedTaskEntity>().ConfigureDeletedTaskEntity();
        }
    }
}