using Moq;
using TaskManagementSystem.Checkers;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Categories.GetTasksForCategory;

[Collection(nameof(CategoryHandlerTestCollection))]
public sealed class GetTasksForCategoryHandlerTests : IAsyncLifetime
{
    private readonly Mock<ICategoryChecker> _categoryCheckerMock;
    private readonly TestDataManager _dataGenerator;
    private List<CategoryEntity> _categories = new();

    private readonly GetTasksForCategoryHandler _handler;

    public GetTasksForCategoryHandlerTests(TestDatabaseFixture fixture)
    {
        _dataGenerator = new TestDataManager(fixture.DbContext);
        _categoryCheckerMock = new Mock<ICategoryChecker>();

        _handler = new GetTasksForCategoryHandler(fixture.DbContext, _categoryCheckerMock.Object);
    }

    public async Task InitializeAsync()
    {
        _categories = await _dataGenerator.InsertCategoriesAsync(2);
    }

    public async Task DisposeAsync()
    {
        await _dataGenerator.ClearTestDataAsync();
    }

    [Fact]
    public async Task Handle_CategoryExists_ReturnsTasksForThisCategory()
    {
        // Arrange
        var request = new GetTasksForCategoryQuery(id: _categories[0].Id);

        var targetTasks = await _dataGenerator.InsertTasksAsync(count: 2, request.Id);
        await _dataGenerator.InsertTasksAsync(count: 2, categoryId: _categories[1].Id);

        _categoryCheckerMock.Setup(x => x.CategoryExistsAsync(request.Id))
            .ReturnsAsync(true);

        // Act
        var resultTasks = await _handler.Handle(request, new CancellationToken());

        // Assert
        var expectedResultTasks = TestResultBuilder.GetExpectedTasks(targetTasks);
        Assert.Equivalent(expectedResultTasks, resultTasks, strict: true);

        _categoryCheckerMock.VerifyCategoryExistsCall();
    }

    [Fact]
    public async Task Handle_CategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var request = new GetTasksForCategoryQuery(id: 1000);

        _categoryCheckerMock.Setup(x => x.CategoryExistsAsync(request.Id))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);

        _categoryCheckerMock.VerifyCategoryExistsCall();
    }
}