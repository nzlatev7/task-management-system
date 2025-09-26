using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Workflows.Prototypes;
using Xunit;

namespace TaskManagementSystem.Tests.UnitTests.Prototypes;

public sealed class IncidentTaskPrototypeTests
{
    private readonly IncidentTaskPrototype _prototype = new();

    [Fact]
    public void Clone_WithoutOverrides_SetsIncidentDefaults()
    {
        // Arrange
        var sourceTask = new TaskEntity
        {
            Title = "Incident",
            Description = "Incident desc",
            DueDate = DateTime.UtcNow.AddHours(-2),
            Priority = Priority.Low,
            Status = Status.Completed,
            IsCompleted = true,
            CategoryId = 9,
            Kind = TaskKind.Incident
        };

        var before = DateTime.UtcNow;

        // Act
        var clone = _prototype.Clone(sourceTask);

        var after = DateTime.UtcNow;

        // Assert
        Assert.Equal(sourceTask.Title, clone.Title);
        Assert.Equal(sourceTask.Description, clone.Description);
        Assert.Equal(sourceTask.CategoryId, clone.CategoryId);
        Assert.Equal(Priority.High, clone.Priority);
        Assert.Equal(Status.InProgress, clone.Status);
        Assert.False(clone.IsCompleted);
        Assert.Null(clone.StoryPoints);
        Assert.InRange(clone.DueDate, before.AddHours(4), after.AddHours(4));
    }

    [Fact]
    public void Clone_WithOverrides_AppliesOverrides()
    {
        // Arrange
        var sourceTask = new TaskEntity
        {
            Title = "Incident",
            Description = "Incident desc",
            DueDate = DateTime.UtcNow,
            Priority = Priority.High,
            Status = Status.InProgress,
            CategoryId = 1,
            Kind = TaskKind.Incident
        };

        var request = new CloneTaskRequestDto
        {
            Priority = Priority.Low,
            Status = Status.Pending,
            DueDate = DateTime.UtcNow.AddHours(8),
            CategoryId = 5
        };

        // Act
        var clone = _prototype.Clone(sourceTask, request);

        // Assert
        Assert.Equal(request.Priority, clone.Priority);
        Assert.Equal(request.Status, clone.Status);
        Assert.Equal(request.DueDate, clone.DueDate);
        Assert.Equal(request.CategoryId, clone.CategoryId);
    }
}
