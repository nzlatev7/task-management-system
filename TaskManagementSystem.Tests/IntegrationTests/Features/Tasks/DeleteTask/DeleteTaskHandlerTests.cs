using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Features.Tasks;
using TaskManagementSystem.Features.Tasks.DeleteTask.Interfaces;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Tasks.DeleteTask;

[Collection(nameof(TaskHandlerTestCollection))]
public sealed class DeleteTaskHandlerTests : IAsyncLifetime
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly Mock<ITaskDeleteOrchestrator> _taskDeleteOrchestratorMock;
    private readonly TestDataManager _dataGenerator;
    private int _targetCategoryId;

    private readonly DeleteTaskHandler _handler;

    public DeleteTaskHandlerTests(TestDatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _dataGenerator = new TestDataManager(_dbContext);
        _taskDeleteOrchestratorMock = new Mock<ITaskDeleteOrchestrator>();

        _handler = new DeleteTaskHandler(_dbContext, _taskDeleteOrchestratorMock.Object);
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
    public async Task Handle_TaskExists_ValidTaskStatus_ReturnsProperDeleteAction()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksPriority: Priority.Low, tasksStatus: Status.InProgress);
        var request = new DeleteTaskCommand(tasks[0].Id);

        var deleteAction = DeleteAction.Removed;
        _taskDeleteOrchestratorMock.Setup(x => x.ExecuteDeletionAsync(It.IsAny<TaskEntity>(), It.IsAny<TaskManagementSystemDbContext>()))
            .ReturnsAsync(deleteAction);

        // Act
        var result = await _handler.Handle(request, new CancellationToken());

        // Assert
        Assert.Equal(deleteAction, result);

        _taskDeleteOrchestratorMock.Verify(c => c.ExecuteDeletionAsync(It.IsAny<TaskEntity>(), It.IsAny<TaskManagementSystemDbContext>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TaskDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId);
        var request = new DeleteTaskCommand(id: 1000);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.TaskDoesNotExist, exception.Message);
    }

    [Fact]
    public async Task Handle_TaskExists_AlreadyLocked_ThrowsConflictException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksStatus: Status.Locked);
        var request = new DeleteTaskCommand(tasks[0].Id);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.TaskAlreadyLocked, exception.Message);
    }

    [Fact]
    public async Task Handle_TaskExists_ArchivedTask_ThrowsConflictException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksStatus: Status.Archived);
        var request = new DeleteTaskCommand(tasks[0].Id);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.ArchivedTaskCanNotBeDeleted, exception.Message);
    }
}
