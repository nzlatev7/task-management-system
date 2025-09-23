using TaskManagementSystem.Builders;
using TaskManagementSystem.Enums;
using Xunit;

namespace TaskManagementSystem.Tests.UnitTests.Builders;

public sealed class TaskEntityBuilderTests
{
    [Fact]
    public void Build_WithDefaultInitialization_SetsSharedFields()
    {
        // Arrange
        var dueDate = DateTime.UtcNow.AddDays(1);

        // Act
        var task = TaskEntityBuilder.Create("Title", "Description", dueDate, 5, TaskKind.Feature)
            .Build();

        // Assert
        Assert.Equal("Title", task.Title);
        Assert.Equal("Description", task.Description);
        Assert.Equal(dueDate, task.DueDate);
        Assert.Equal(5, task.CategoryId);
        Assert.Equal(TaskKind.Feature, task.Kind);
        Assert.Equal(Priority.Medium, task.Priority);
        Assert.Equal(Status.Pending, task.Status);
        Assert.False(task.IsCompleted);
        Assert.Null(task.StoryPoints);
    }

    [Fact]
    public void WithPriority_WhenCalled_OverridesPriority()
    {
        // Act
        var task = TaskEntityBuilder.Create("Title", null, DateTime.UtcNow, 1, TaskKind.Bug)
            .WithPriority(Priority.High)
            .Build();

        // Assert
        Assert.Equal(Priority.High, task.Priority);
    }

    [Fact]
    public void WithStatus_WhenCompleted_SetsCompletionFlag()
    {
        // Act
        var task = TaskEntityBuilder.Create("Title", null, DateTime.UtcNow, 1, TaskKind.Feature)
            .WithStatus(Status.Completed)
            .Build();

        // Assert
        Assert.Equal(Status.Completed, task.Status);
        Assert.True(task.IsCompleted);
    }

    [Fact]
    public void WithStoryPoints_WhenProvided_SetsStoryPoints()
    {
        // Act
        var task = TaskEntityBuilder.Create("Title", null, DateTime.UtcNow, 1, TaskKind.Feature)
            .WithStoryPoints(8)
            .Build();

        // Assert
        Assert.Equal(8, task.StoryPoints);
    }
}
