using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Workflows;
using Xunit;

namespace TaskManagementSystem.Tests.UnitTests.Workflows;

public sealed class IncidentWorkflowTests
{
    private readonly IncidentWorkflow _workflow = new();

    [Fact]
    public void Build_WhenDueDateNotProvided_UsesDefaults()
    {
        // Arrange
        var dto = new CreateTaskRequestDto
        {
            Title = "Incident",
            Description = "Service outage",
            DueDate = default,
            CategoryId = 10,
            Kind = TaskKind.Incident
        };

        var before = DateTime.UtcNow;

        // Act
        var task = _workflow.Build(dto);

        var after = DateTime.UtcNow;

        // Assert
        Assert.Equal(Priority.High, task.Priority);
        Assert.Equal(Status.InProgress, task.Status);
        Assert.False(task.IsCompleted);
        Assert.InRange(task.DueDate, before.AddHours(4), after.AddHours(4));
    }

    [Fact]
    public void Build_WhenDueDateProvided_UsesProvidedValue()
    {
        // Arrange
        var dueDate = DateTime.UtcNow.AddHours(1);
        var dto = new CreateTaskRequestDto
        {
            Title = "Incident",
            DueDate = dueDate,
            CategoryId = 3,
            Kind = TaskKind.Incident
        };

        // Act
        var task = _workflow.Build(dto);

        // Assert
        Assert.Equal(dueDate, task.DueDate);
    }
}
