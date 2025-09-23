using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Workflows.Prototypes;
using Xunit;

namespace TaskManagementSystem.Tests.UnitTests.Prototypes;

public sealed class BugTaskPrototypeTests
{
    private readonly BugTaskPrototype _prototype = new();

    [Fact]
    public void Clone_WithoutOverrides_KeepsSourceDetailsAndAssignsDefaultDueDate()
    {
        // Arrange
        var sourceTask = new TaskEntity
        {
            Title = "Bug",
            Description = "Bug desc",
            DueDate = DateTime.UtcNow.AddDays(-1),
            Priority = Priority.Low,
            Status = Status.Completed,
            IsCompleted = true,
            CategoryId = 8,
            Kind = TaskKind.Bug
        };

        var before = DateTime.UtcNow;

        // Act
        var clone = _prototype.Clone(sourceTask);

        var after = DateTime.UtcNow;

        // Assert
        Assert.Equal(sourceTask.Title, clone.Title);
        Assert.Equal(sourceTask.Description, clone.Description);
        Assert.Equal(sourceTask.CategoryId, clone.CategoryId);
        Assert.Equal(sourceTask.Priority, clone.Priority);
        Assert.Equal(Status.Pending, clone.Status);
        Assert.False(clone.IsCompleted);
        Assert.Null(clone.StoryPoints);
        Assert.InRange(clone.DueDate, before.AddDays(2), after.AddDays(2));
    }

    [Fact]
    public void Clone_WithSeverity_AdjustsPriority()
    {
        // Arrange
        var sourceTask = new TaskEntity
        {
            Title = "Bug",
            Description = "Bug desc",
            DueDate = DateTime.UtcNow,
            Priority = Priority.Low,
            Status = Status.Pending,
            CategoryId = 2,
            Kind = TaskKind.Bug
        };

        var request = new CloneTaskRequestDto
        {
            Severity = 5
        };

        // Act
        var clone = _prototype.Clone(sourceTask, request);

        // Assert
        Assert.Equal(Priority.High, clone.Priority);
        Assert.Equal(Status.Pending, clone.Status);
        Assert.Null(clone.StoryPoints);
    }
}
