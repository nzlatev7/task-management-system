using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Features.Tasks;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Tasks.UnlockTask;

[Collection(nameof(TaskHandlerTestCollection))]
public sealed class UnlockTaskHandlerTests : IAsyncLifetime
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly TestDataManager _dataGenerator;
    private int _targetCategoryId;

    private readonly UnlockTaskHandler _handler;

    public UnlockTaskHandlerTests(TestDatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _dataGenerator = new TestDataManager(_dbContext);

        _handler = new UnlockTaskHandler(_dbContext);
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
    public async Task Handle_TaskExists_TaskLocked_ChangesTaskStatus()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksStatus: Status.Locked);

        var request = new UnlockTaskCommand(id: tasks[0].Id)
        {
            Status = Status.Completed,
        };

        // Act
        await _handler.Handle(request, new CancellationToken());

        // Assert
        var updatedStatus = await _dbContext.Tasks
            .Where(x => x.Id == request.Id)
            .Select(x => x.Status)
            .FirstOrDefaultAsync();

        Assert.Equal(request.Status, updatedStatus);
    }

    [Fact]
    public async Task Handle_TaskDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksStatus: Status.Locked);

        var request = new UnlockTaskCommand(id: 1000);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.LockedTaskWithIdDoesNotExist, exception.Message);
    }

    [Fact]
    public async Task Handle_TaskExists_TaskNotLocked_ThrowsNotFoundException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksStatus: Status.Pending);

        var request = new UnlockTaskCommand(id: tasks[0].Id)
        {
            Status = Status.Completed
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.LockedTaskWithIdDoesNotExist, exception.Message);
    }
}
