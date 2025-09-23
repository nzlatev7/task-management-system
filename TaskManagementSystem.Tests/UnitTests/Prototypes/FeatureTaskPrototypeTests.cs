using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Workflows.Prototypes;
using Xunit;

namespace TaskManagementSystem.Tests.UnitTests.Prototypes;

public sealed class FeatureTaskPrototypeTests
{
    private readonly FeatureTaskPrototype _prototype = new();

    [Fact]
    public void Clone_WithoutOverrides_UsesSourceDetailsAndResetsDefaults()
    {
        // Arrange
        var sourceTask = new TaskEntity
        {
            Title = "Feature",
            Description = "Implement", 
            DueDate = DateTime.UtcNow.AddDays(-3),
            Priority = Priority.High,
            Status = Status.Completed,
            IsCompleted = true,
            CategoryId = 4,
            Kind = TaskKind.Feature,
            StoryPoints = 5
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
        Assert.Equal(sourceTask.StoryPoints, clone.StoryPoints);
        Assert.Equal(TaskKind.Feature, clone.Kind);
        Assert.InRange(clone.DueDate, before.AddDays(14), after.AddDays(14));
    }

    [Fact]
    public void Clone_WithOverrides_AppliesOverrides()
    {
        // Arrange
        var sourceTask = new TaskEntity
        {
            Title = "Feature",
            Description = "Implement",
            DueDate = DateTime.UtcNow,
            Priority = Priority.Medium,
            Status = Status.Pending,
            IsCompleted = false,
            CategoryId = 7,
            Kind = TaskKind.Feature,
            StoryPoints = 3
        };

        var request = new CloneTaskRequestDto
        {
            Title = "Feature copy",
            Description = "Cloned",
            DueDate = DateTime.UtcNow.AddDays(2),
            CategoryId = 11,
            Priority = Priority.Low,
            Status = Status.Completed,
            StoryPoints = 13
        };

        // Act
        var clone = _prototype.Clone(sourceTask, request);

        // Assert
        Assert.Equal(request.Title, clone.Title);
        Assert.Equal(request.Description, clone.Description);
        Assert.Equal(request.CategoryId, clone.CategoryId);
        Assert.Equal(request.Priority, clone.Priority);
        Assert.Equal(request.Status, clone.Status);
        Assert.True(clone.IsCompleted);
        Assert.Equal(request.StoryPoints, clone.StoryPoints);
        Assert.Equal(request.DueDate, clone.DueDate);
    }
}
