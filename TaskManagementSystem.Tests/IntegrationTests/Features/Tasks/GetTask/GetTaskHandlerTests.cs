using TaskManagementSystem.Constants;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Features.Tasks;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Tasks.GetTask;

[Collection(nameof(TaskHandlerTestCollection))]
public sealed class GetTaskHandlerTests : IAsyncLifetime
{
    private readonly TestDataManager _dataGenerator;
    private int _targetCategoryId;

    private readonly GertTaskByIdHandler _handler;

    public GetTaskHandlerTests(TestDatabaseFixture fixture)
    {
        _dataGenerator = new TestDataManager(fixture.DbContext);
        _handler = new GertTaskByIdHandler(fixture.DbContext);
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
    public async Task Handle_ReturnsExpectedTask()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksPriority: Priority.High);
        var targetTask = tasks[0];
        var request = new GetTaskByIdQuery(id: targetTask.Id);

        // Act
        var resultTask = await _handler.Handle(request, new CancellationToken());

        // Assert
        Assert.NotNull(resultTask);

        var expectedTask = TestResultBuilder.GetExpectedTask(targetTask);
        Assert.Equivalent(expectedTask, resultTask, strict: true);
    }

    [Fact]
    public async Task Handle_TaskDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 1, _targetCategoryId);
        var request = new GetTaskByIdQuery(id: 1000);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.TaskDoesNotExist, exception.Message);
    }
}
