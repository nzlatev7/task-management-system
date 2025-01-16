using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManagementSystem.Checkers;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Tasks;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Tasks.CreateTask;

[Collection(nameof(TaskHandlerTestCollection))]
public sealed class CreateTaskHandlerTests : IAsyncLifetime
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly TestDataManager _dataGenerator;
    private readonly Mock<ICategoryChecker> _categoryCheckerMock;
    private int _targetCategoryId;

    private readonly CreateTaskHandler _handler;

    public CreateTaskHandlerTests(TestDatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _dataGenerator = new TestDataManager(_dbContext);
        _categoryCheckerMock = new Mock<ICategoryChecker>();

        _handler = new CreateTaskHandler(_dbContext, _categoryCheckerMock.Object);
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
    public async Task Handle_CategoryExists_TaskCreated_ReturnsCreatedTask()
    {
        // Arrange
        var request = new CreateTaskCommand
        {
            Title = "New Task",
            Description = "Task description",
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Low,
            CategoryId = _targetCategoryId
        };

        _categoryCheckerMock.Setup(c => c.CategoryExistsAsync(request.CategoryId))
            .ReturnsAsync(true);

        // Act
        var resultTask = await _handler.Handle(request, new CancellationToken());

        // Assert
        var expectedTask = TestResultBuilder.GetExpectedTask(resultTask.Id, request);
        Assert.Equivalent(expectedTask, resultTask, strict: true);

        var count = await _dbContext.Tasks.CountAsync();
        Assert.Equal(expected: 1, count);

        var savedTask = await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == resultTask.Id);

        Assert.NotNull(savedTask);
        Assert.Equivalent(expectedTask, savedTask);

        _categoryCheckerMock.VerifyCategoryExistsCall();
    }

    [Fact]
    public async Task Handle_CategoryDoesNotExist_ThrowsBadHttpRequestException()
    {
        // Arrange
        var request = new CreateTaskCommand
        {
            Title = "New Task",
            CategoryId = 10000
        };

        _categoryCheckerMock.Setup(c => c.CategoryExistsAsync(request.CategoryId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadHttpRequestException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);

        _categoryCheckerMock.VerifyCategoryExistsCall();
    }

    [Fact]
    public async Task Handle_CategoryExists_PriorityNotProvided_CreatesTaskWithDefaultPriority()
    {
        // Arrange
        var request = new CreateTaskCommand
        {
            Title = "New Task",
            CategoryId = _targetCategoryId
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
