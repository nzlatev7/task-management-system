using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.TaskDeleteStategy;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.TaskDeleteStrategies;

public sealed class TaskDeleteStrategiesTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly TestDataManager _dataGenerator;

    private ITaskDeleteStrategy _taskDeleteStategy = null!;

    private int _targetCategoryId;

    public TaskDeleteStrategiesTests(TestDatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _dataGenerator = new TestDataManager(_dbContext);
    }

    public async Task InitializeAsync()
    {
        _targetCategoryId = await _dataGenerator.InsertCategoryAsync();
    }

    public async Task DisposeAsync()
    {
        await _dataGenerator.ClearTestDataAsync();
    }

    #region RemovingDeleteStategy

    [Fact]
    public async Task DeleteAsync_RemovesTask_ReturnsRemovedDeleteAction()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksPriority: Priority.Low);
        var targetTask = tasks[0];

        _taskDeleteStategy = new TaskRemovingDeleteStrategy();

        // Act
        var result = await _taskDeleteStategy.DeleteAsync(targetTask, _dbContext);

        // Assert
        Assert.Equal(DeleteAction.Removed, result);

        var expectedCount = tasks.Count - 1;
        var tasksCount = _dbContext.Tasks.Count();
        Assert.Equal(expectedCount, tasksCount);

        var removedTask = await _dbContext.Tasks.FindAsync(targetTask.Id);
        Assert.Null(removedTask);
    }

    [Fact]
    public void CanExecute_LowPriorityTask_ReturnsTrue()
    {
        // Arrange
        var taskEntity = new TaskEntity()
        {
            Title = "Test",
            Priority = Priority.Low,
        };

        _taskDeleteStategy = new TaskRemovingDeleteStrategy();

        // Act
        var result = _taskDeleteStategy.CanExecute(taskEntity);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(Priority.Medium)]
    [InlineData(Priority.High)]
    public void CanExecute_Removing_InvalidPriorityTask_ReturnsFalse(Priority priority)
    {
        // Arrange
        var taskEntity = new TaskEntity()
        {
            Title = "Test",
            Priority = priority,
        };

        _taskDeleteStategy = new TaskRemovingDeleteStrategy();

        // Act
        var result = _taskDeleteStategy.CanExecute(taskEntity);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region MovingDeleteStategy

    [Fact]
    public async Task DeleteAsync_RemovesTask_MovesTaskToDeletedTasks_ReturnsMovedDeleteAction()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksPriority: Priority.Medium);
        var targetTask = tasks[0];

        var deletedTasksCount = _dbContext.DeletedTasks.Count();

        _taskDeleteStategy = new TaskMovingDeleteStrategy();

        // Act
        var result = await _taskDeleteStategy.DeleteAsync(targetTask, _dbContext);

        // Assert
        Assert.Equal(DeleteAction.Moved, result);

        var expectedCount = tasks.Count - 1;
        var tasksCount = _dbContext.Tasks.Count();
        Assert.Equal(expectedCount, tasksCount);

        var removedTask = await _dbContext.Tasks.FindAsync(targetTask.Id);
        Assert.Null(removedTask);

        var expectedDeletedTasksCount = deletedTasksCount + 1;
        deletedTasksCount = _dbContext.DeletedTasks.Count();
        Assert.Equal(expectedDeletedTasksCount, deletedTasksCount);

        var insertedDeletedTask = await _dbContext.DeletedTasks.FindAsync(targetTask.Id);
        Assert.NotNull(insertedDeletedTask);

        var expectedDeletedTask = TestResultBuilder.GetExpectedDeletedTask(targetTask);
        insertedDeletedTask.Category = null;
        Assert.Equivalent(expectedDeletedTask, insertedDeletedTask, strict: true);
    }

    [Fact]
    public void CanExecute_MediumPriorityTask_ReturnsTrue()
    {
        // Arrange
        var taskEntity = new TaskEntity()
        {
            Title = "Test",
            Priority = Priority.Medium,
        };

        _taskDeleteStategy = new TaskMovingDeleteStrategy();

        // Act
        var result = _taskDeleteStategy.CanExecute(taskEntity);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(Priority.Low)]
    [InlineData(Priority.High)]
    public void CanExecute_Moving_InvalidPriorityTask_ReturnsFalse(Priority priority)
    {
        // Arrange
        var taskEntity = new TaskEntity()
        {
            Title = "Test",
            Priority = priority,
        };

        _taskDeleteStategy = new TaskMovingDeleteStrategy();

        // Act
        var result = _taskDeleteStategy.CanExecute(taskEntity);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region LockingDeleteStategy

    [Fact]
    public async Task DeleteAsync_DoesNotDeleteTask_ChangesTaskStatusToLocked_ReturnsLockedDeleteAction()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksStatus: Status.InProgress, tasksPriority: Priority.High);
        var targetTask = tasks[0];

        _taskDeleteStategy = new TaskLockingDeleteStrategy();

        // Act
        var result = await _taskDeleteStategy.DeleteAsync(targetTask, _dbContext);

        // Assert
        Assert.Equal(DeleteAction.Locked, result);

        var updatedTask = await _dbContext.Tasks.FindAsync(targetTask.Id);
        Assert.NotNull(updatedTask);
        Assert.Equal(Status.Locked, updatedTask.Status);
    }

    [Fact]
    public void CanExecute_HighPriorityTask_ReturnsTrue()
    {
        // Arrange
        var taskEntity = new TaskEntity()
        {
            Title = "Test",
            Priority = Priority.High,
        };

        _taskDeleteStategy = new TaskLockingDeleteStrategy();

        // Act
        var result = _taskDeleteStategy.CanExecute(taskEntity);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(Priority.Low)]
    [InlineData(Priority.Medium)]
    public void CanExecute_Locking_InvalidPriorityTask_ReturnsFalse(Priority priority)
    {
        // Arrange
        var taskEntity = new TaskEntity()
        {
            Title = "Test",
            Priority = priority,
        };

        _taskDeleteStategy = new TaskLockingDeleteStrategy();

        // Act
        var result = _taskDeleteStategy.CanExecute(taskEntity);

        // Assert
        Assert.False(result);
    }

    #endregion
}