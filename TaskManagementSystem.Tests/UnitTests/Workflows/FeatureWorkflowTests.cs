using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Workflows;
using Xunit;

namespace TaskManagementSystem.Tests.UnitTests.Workflows;

public sealed class FeatureWorkflowTests
{
    private readonly FeatureWorkflow _workflow = new();

    [Fact]
    public void Build_WhenPriorityProvided_UsesPriorityAndDefaultDueDate()
    {
        // Arrange
        var dto = new CreateTaskRequestDto
        {
            Title = "Feature",
            Description = "New capability",
            DueDate = default,
            Priority = Priority.High,
            StoryPoints = 3,
            CategoryId = 9,
            Kind = TaskKind.Feature
        };

        var before = DateTime.UtcNow;

        // Act
        var task = _workflow.Build(dto);

        var after = DateTime.UtcNow;

        // Assert
        Assert.Equal(Priority.High, task.Priority);
        Assert.Equal(TaskKind.Feature, task.Kind);
        Assert.Equal(3, task.StoryPoints);
        Assert.InRange(task.DueDate, before.AddDays(14), after.AddDays(14));
    }

    [Fact]
    public void Validate_WithoutStoryPoints_ThrowsArgumentException()
    {
        // Arrange
        var dto = new CreateTaskRequestDto
        {
            Title = "Feature",
            DueDate = DateTime.UtcNow.AddDays(1),
            CategoryId = 3,
            Kind = TaskKind.Feature
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _workflow.Validate(dto));
    }

    [Fact]
    public void Build_WhenPriorityNotProvided_UsesMediumPriority()
    {
        // Arrange
        var dueDate = DateTime.UtcNow.AddDays(3);
        var dto = new CreateTaskRequestDto
        {
            Title = "Feature",
            DueDate = dueDate,
            StoryPoints = 5,
            CategoryId = 4,
            Kind = TaskKind.Feature
        };

        // Act
        var task = _workflow.Build(dto);

        // Assert
        Assert.Equal(Priority.Medium, task.Priority);
        Assert.Equal(dueDate, task.DueDate);
    }
}
