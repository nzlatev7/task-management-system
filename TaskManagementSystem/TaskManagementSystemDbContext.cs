using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TaskManagementSystem.Models;

namespace TaskManagementSystem
{
    public class TaskManagementSystemDbContext : DbContext
    {
        public TaskManagementSystemDbContext(DbContextOptions<TaskManagementSystemDbContext> options) : base(options) { }

        public DbSet<TaskEntity> Tasks { get; } = null!;
        public DbSet<Category> Categories { get; } = null!;
    }
}
