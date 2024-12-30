using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.TaskDeleteStategy;

namespace TaskManagementSystem.Tests.UnitTests.TaskDeleteStrategy;

public sealed class TaskDeleteFactoryTests
{
    private readonly ITaskDeleteFactory _taskDeleteFactory;

    public TaskDeleteFactoryTests()
    {
        _taskDeleteFactory = new TaskDeleteFactory();
    }

    #region GetDeleteStrategy

    [Theory]
    [InlineData(Priority.Low)]
    [InlineData(Priority.Medium)]
    [InlineData(Priority.High)]
    public void GetDeleteStrategy_ReturnsProperStrategy_ForGivenPriority(Priority priority)
    {
        // Act
        var result = _taskDeleteFactory.GetDeleteStrategy(priority);

        // Assert
        var expectedStrategyType = GetExpectedStrategyType(priority);
        Assert.IsType(expectedStrategyType, result);
    }

    #endregion

    private Type GetExpectedStrategyType(Priority priority) => priority switch
    {
        Priority.Low => typeof(TaskRemovingDeleteStategy),
        Priority.Medium => typeof(TaskMovingDeleteStategy),
        Priority.High => typeof(TaskLockingDeleteStategy),
        _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, null)
    };
}