using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManagementSystem.Checkers;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Features.Tasks;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Tasks.UpdaterTask;

[Collection(nameof(TaskHandlerTestCollection))]
public sealed class UpdateTaskHandlerTests : IAsyncLifetime
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly TestDataManager _dataGenerator;
    private readonly Mock<ICategoryChecker> _categoryCheckerMock;
    private int _targetCategoryId;

    private readonly UpdateTaskHandler _handler;

    public UpdateTaskHandlerTests(TestDatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _dataGenerator = new TestDataManager(_dbContext);
        _categoryCheckerMock = new Mock<ICategoryChecker>();

        _handler = new UpdateTaskHandler(_dbContext, _categoryCheckerMock.Object);
    }

    public async Task InitializeAsync()
    {
        _targetCategoryId = await _dataGenerator.InsertCategoryAsync();
    }

    public async Task DisposeAsync()
    {
        await _dataGenerator.ClearTestDataAsync();
    }

    [Fact]
    public async Task Handle_TaskExists_ValidTaskStatus_CategoryExists_TaskUpdated_ReturnsUpdatedTask()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksStatus: Status.InProgress);
        var targetTask = tasks[0];

        var request = new UpdateTaskCommand(targetTask.Id)
        {
            Title = "Updated Title",
            Description = targetTask.Description,
            DueDate = targetTask.DueDate,
            Priority = targetTask.Priority,
            Status = targetTask.Status,
            CategoryId = _targetCategoryId
        };

        _categoryCheckerMock.Setup(c => c.CategoryExistsAsync(request.CategoryId))
            .ReturnsAsync(true);

        // Act
        var resultTask = await _handler.Handle(request, new CancellationToken());

        // Assert
        var expectedTask = TestResultBuilder.GetExpectedTask(targetTask.Id, request);
        Assert.Equivalent(expectedTask, resultTask, strict: true);

        var count = await _dbContext.Tasks.CountAsync();
        Assert.Equal(tasks.Count, count);

        var updatedTask = await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == resultTask.Id);

        Assert.NotNull(updatedTask);
        Assert.Equivalent(expectedTask, updatedTask);

        _categoryCheckerMock.VerifyCategoryExistsCall();
    }

    [Fact]
    public async Task Handle_TaskDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId);

        var request = new UpdateTaskCommand(id: 1000);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.TaskDoesNotExist, exception.Message);
    }

    [Fact]
    public async Task Handle_TaskExists_TargetedArchivedTask_ThrowsConflictException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 1, _targetCategoryId, tasksStatus: Status.Archived);

        var request = new UpdateTaskCommand(id: tasks[0].Id);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.ArchivedTaskCanNotBeEdited, exception.Message);
    }

    [Fact]
    public async Task Handle_TaskExists_NotCompletedTask_MovedToArchived_ThrowsConflictException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 1, _targetCategoryId, tasksStatus: Status.Pending);
        var targetTask = tasks[0];

        var request = new UpdateTaskCommand(id: tasks[0].Id)
        {
            Status = Status.Archived
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.OnlyCompletedTaskCanBeArchived, exception.Message);
    }

    [Fact]
    public async Task Handle_TaskExists_TargetedLockedTask_ThrowsConflictException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 1, _targetCategoryId, tasksStatus: Status.Locked);
        var targetTask = tasks[0];

        var request = new UpdateTaskCommand(id: tasks[0].Id);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.LockedTaskCanNotBeEdited, exception.Message);
    }

    [Fact]
    public async Task Handle_TaskExists_CategoryDoesNotExist_ThrowsBadHttpRequestException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 1, _targetCategoryId);

        var request = new UpdateTaskCommand(id: tasks[0].Id)
        {
            CategoryId = 1000
        };

        _categoryCheckerMock.Setup(c => c.CategoryExistsAsync(request.CategoryId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadHttpRequestException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);

        _categoryCheckerMock.VerifyCategoryExistsCall();
    }

    [Fact]
    public async Task Handle_TaskExists_CategoryExists_PriorityNotProvided_UpdatesTaskWithDefaultPriority()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 1, _targetCategoryId);
        var targetTask = tasks[0];
        var request = new UpdateTaskCommand(targetTask.Id)
        {
            Title = targetTask.Title,
            CategoryId = targetTask.CategoryId
        };

        _categoryCheckerMock.Setup(c => c.CategoryExistsAsync(request.CategoryId))
            .ReturnsAsync(true);

        // Act
        var resultTask = await _handler.Handle(request, new CancellationToken());

        // Assert
        var defaultPriority = Priority.Medium;
        Assert.Equal(defaultPriority, resultTask.Priority);
    }
}
