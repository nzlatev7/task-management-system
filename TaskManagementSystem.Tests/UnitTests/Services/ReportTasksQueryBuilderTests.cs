using System.Collections.Generic;
using System.Linq;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Reports;

namespace TaskManagementSystem.Tests.UnitTests.Services;

public sealed class ReportTasksQueryBuilderTests
{
    private static readonly List<TaskEntity> TaskSource =
    [
        new()
        {
            Id = 1,
            Title = "Todo task",
            CategoryId = 1,
            Status = Status.Todo,
            Priority = Priority.Low,
            DueDate = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc),
            Category = null!
        },
        new()
        {
            Id = 2,
            Title = "In progress task",
            CategoryId = 1,
            Status = Status.InProgress,
            Priority = Priority.High,
            DueDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            Category = null!
        },
        new()
        {
            Id = 3,
            Title = "Completed task",
            CategoryId = 2,
            Status = Status.Completed,
            Priority = Priority.Medium,
            DueDate = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc),
            Category = null!
        }
    ];

    private readonly ReportTasksQueryBuilder _sut = new();

    [Fact]
    public void Apply_WithStatusFilter_ReturnsOnlyMatchingTasks()
    {
        var filters = new ReportTasksRequestDto { Status = Status.InProgress };

        var result = _sut.Apply(CreateSource(), filters).Select(task => task.Id).ToArray();

        Assert.Equal(new[] { 2 }, result);
    }

    [Fact]
    public void Apply_WithPriorityFilter_ReturnsOnlyMatchingTasks()
    {
        var filters = new ReportTasksRequestDto { Priority = Priority.High };

        var result = _sut.Apply(CreateSource(), filters).Select(task => task.Id).ToArray();

        Assert.Equal(new[] { 2 }, result);
    }

    [Fact]
    public void Apply_WithDueAfterFilter_ReturnsTasksAfterProvidedDate()
    {
        var filters = new ReportTasksRequestDto
        {
            DueAfter = new DateTime(2024, 1, 12, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = _sut.Apply(CreateSource(), filters).Select(task => task.Id).ToArray();

        Assert.Equal(new[] { 2, 3 }, result);
    }

    [Fact]
    public void Apply_WithDueBeforeFilter_ReturnsTasksBeforeProvidedDate()
    {
        var filters = new ReportTasksRequestDto
        {
            DueBefore = new DateTime(2024, 1, 18, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = _sut.Apply(CreateSource(), filters).Select(task => task.Id).ToArray();

        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public void Apply_WithCombinedDueDateFilters_ReturnsTasksWithinRange()
    {
        var filters = new ReportTasksRequestDto
        {
            DueAfter = new DateTime(2024, 1, 12, 0, 0, 0, DateTimeKind.Utc),
            DueBefore = new DateTime(2024, 1, 18, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = _sut.Apply(CreateSource(), filters).Select(task => task.Id).ToArray();

        Assert.Equal(new[] { 2 }, result);
    }

    [Fact]
    public void Apply_WhenInvokedMultipleTimes_RemainsStateless()
    {
        var highPriorityFilters = new ReportTasksRequestDto { Priority = Priority.High };
        var completedStatusFilters = new ReportTasksRequestDto { Status = Status.Completed };

        var firstCall = _sut.Apply(CreateSource(), highPriorityFilters).Select(task => task.Id).ToArray();
        var secondCall = _sut.Apply(CreateSource(), highPriorityFilters).Select(task => task.Id).ToArray();
        var thirdCall = _sut.Apply(CreateSource(), completedStatusFilters).Select(task => task.Id).ToArray();

        Assert.Equal(firstCall, secondCall);
        Assert.Equal(new[] { 3 }, thirdCall);
    }

    private static IQueryable<TaskEntity> CreateSource()
    {
        return TaskSource.AsQueryable();
    }
}
