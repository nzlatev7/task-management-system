using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database.Models;

namespace TaskManagementSystem.Database
{
    public class TaskManagementSystemDbContext : DbContext
    {
        public TaskManagementSystemDbContext(DbContextOptions<TaskManagementSystemDbContext> options) : base(options) { }

        public DbSet<TaskEntity> Tasks { get; set; } = default!;
        public DbSet<CategoryEntity> Categories { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskEntity>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}