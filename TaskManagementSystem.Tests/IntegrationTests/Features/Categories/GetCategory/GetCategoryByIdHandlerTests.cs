using TaskManagementSystem.Constants;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Categories.GetCategory;

[Collection(nameof(CategoryHandlerTestCollection))]
public sealed class GetCategoryByIdHandlerTests : IAsyncLifetime
{
    private readonly TestDataManager _dataGenerator;
    private List<CategoryEntity> _categories = new();

    private readonly GetCategoryByIdHandler _handler;

    public GetCategoryByIdHandlerTests(TestDatabaseFixture fixture)
    {
        _dataGenerator = new TestDataManager(fixture.DbContext);
        _handler = new GetCategoryByIdHandler(fixture.DbContext);
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
    public async Task Handle_ReturnsExpectedCategory()
    {
        // Arrange
        var targetCategory = _categories[0];
        var request = new GetCategoryByIdQuery(targetCategory.Id);

        // Act
        var resultCategory = await _handler.Handle(request, new CancellationToken());

        // Assert            
        var expectedCategory = TestResultBuilder.GetExpectedCategory(targetCategory);
        Assert.Equivalent(expectedCategory, resultCategory, strict: true);
    }

    [Fact]
    public async Task Handle_CategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var request = new GetCategoryByIdQuery(id: 1000);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);
    }
}
