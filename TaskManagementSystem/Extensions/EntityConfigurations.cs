using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Extensions;

public static class EntityConfigurations
{
    private const Priority DefaultPriority = Priority.Medium;
    private const Status DefaultStatus = Status.Pending;

    public static void ConfigureTaskEntity(this EntityTypeBuilder<TaskEntity> taskEntity)
    {
        taskEntity.Property(t => t.Priority)
            .HasDefaultValue(DefaultPriority)
            .HasSentinel(DefaultPriority)
            .HasColumnType("smallint");

        taskEntity.Property(t => t.Status)
            .HasDefaultValue(DefaultStatus)
            .HasColumnType("smallint");

        taskEntity.HasIndex(t => t.Status)
            .HasDatabaseName("IX_task_status");

        taskEntity.HasOne(t => t.Category)
            .WithMany(c => c.Tasks)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public static void ConfigureDeletedTaskEntity(this EntityTypeBuilder<DeletedTaskEntity> deletedTaskEntity)
    {
        deletedTaskEntity.Property(e => e.TaskId)
              .ValueGeneratedNever();

        deletedTaskEntity.Property(t => t.Priority)
            .HasDefaultValue(DefaultPriority)
            .HasSentinel(DefaultPriority)
            .HasColumnType("smallint");

        deletedTaskEntity.Property(t => t.Status)
            .HasColumnType("smallint");

        deletedTaskEntity.HasOne(t => t.Category)
            .WithMany()
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
