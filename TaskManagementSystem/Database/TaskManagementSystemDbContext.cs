using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Database
{
    public class TaskManagementSystemDbContext : DbContext
    {
        public TaskManagementSystemDbContext(DbContextOptions<TaskManagementSystemDbContext> options) : base(options) { }

        public DbSet<TaskEntity> Tasks { get; set; } = default!;
        public DbSet<CategoryEntity> Categories { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<TaskEntity>();

            entity.Property(t => t.Priority)
                .HasDefaultValue(Priority.Medium)
                .HasSentinel(Priority.Medium)
                .HasColumnType("smallint");

            entity.Property(t => t.Status)
                .HasDefaultValue(Status.Pending)
                .HasColumnType("smallint");

            entity.HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}