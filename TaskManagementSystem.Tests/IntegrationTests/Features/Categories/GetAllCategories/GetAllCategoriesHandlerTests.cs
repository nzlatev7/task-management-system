using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Categories.GetAllCategories;

[Collection(nameof(CategoryHandlerTestCollection))]
public sealed class GetAllCategoriesHandlerTests : IAsyncLifetime
{
    private readonly TestDataManager _dataGenerator;
    private List<CategoryEntity> _categories = new();

    private readonly GetAllCategoriesHandler _handler;

    public GetAllCategoriesHandlerTests(TestDatabaseFixture fixture)
    {
        _dataGenerator = new TestDataManager(fixture.DbContext);
        _handler = new GetAllCategoriesHandler(fixture.DbContext);
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
    public async Task Handle_ReturnsAllCategories()
    {
        // Arrange
        var request = new GetAllCategoriesQuery();

        // Act
        var resultCategories = await _handler.Handle(request, new CancellationToken());

        // Assert
        var expectedResultCategories = TestResultBuilder.GetExpectedCategories(_categories);
        Assert.Equivalent(expectedResultCategories, resultCategories, strict: true);
    }
}
