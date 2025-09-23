using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Workflows;
using Xunit;

namespace TaskManagementSystem.Tests.UnitTests.Workflows;

public sealed class BugWorkflowTests
{
    private readonly BugWorkflow _workflow = new();

    [Fact]
    public void Build_WithHighSeverity_UsesHighPriorityAndDefaultDueDate()
    {
        // Arrange
        var dto = new CreateTaskRequestDto
        {
            Title = "Critical bug",
            Description = "Fix immediately",
            DueDate = default,
            CategoryId = 7,
            Kind = TaskKind.Bug,
            Severity = 5
        };

        var before = DateTime.UtcNow;

        // Act
        var task = _workflow.Build(dto);

        var after = DateTime.UtcNow;

        // Assert
        Assert.Equal(Priority.High, task.Priority);
        Assert.Equal(TaskKind.Bug, task.Kind);
        Assert.Equal(dto.Title, task.Title);
        Assert.Equal(dto.Description, task.Description);
        Assert.Equal(dto.CategoryId, task.CategoryId);
        Assert.Equal(Status.Pending, task.Status);
        Assert.False(task.IsCompleted);
        Assert.InRange(task.DueDate, before.AddDays(2), after.AddDays(2));
    }

    [Fact]
    public void Build_WithLowSeverity_UsesProvidedDueDateAndLowPriority()
    {
        // Arrange
        var dueDate = DateTime.UtcNow.AddDays(5);
        var dto = new CreateTaskRequestDto
        {
            Title = "Minor bug",
            DueDate = dueDate,
            CategoryId = 2,
            Kind = TaskKind.Bug,
            Severity = 2
        };

        // Act
        var task = _workflow.Build(dto);

        // Assert
        Assert.Equal(Priority.Low, task.Priority);
        Assert.Equal(dueDate, task.DueDate);
    }

    [Fact]
    public void Validate_WithoutSeverity_ThrowsArgumentException()
    {
        // Arrange
        var dto = new CreateTaskRequestDto
        {
            Title = "Bug",
            DueDate = DateTime.UtcNow.AddDays(1),
            CategoryId = 1,
            Kind = TaskKind.Bug
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _workflow.Validate(dto));
    }
}
