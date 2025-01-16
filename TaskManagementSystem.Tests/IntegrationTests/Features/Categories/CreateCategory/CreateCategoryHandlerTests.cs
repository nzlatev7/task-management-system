using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Features.Shared.DTOs;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Categories.CreateCategory;

[Collection(nameof(CategoryHandlerTestCollection))]
public sealed class CreateCategoryHandlerTests : IAsyncLifetime
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly TestDataManager _dataGenerator;

    private readonly CreateCategoryHandler _handler;

    public CreateCategoryHandlerTests(TestDatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _dataGenerator = new TestDataManager(_dbContext);

        _handler = new CreateCategoryHandler(_dbContext);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _dataGenerator.ClearTestDataAsync();
    }

    [Fact]
    public async Task Handle_CategoryCreated_ReturnsTheCategory()
    {
        // Arrange
        var request = new CreateCategoryCommand
        {
            Name = "Test",
            Description = "Test Description",
        };

        // Act
        var resultCategory = await _handler.Handle(request, new CancellationToken());

        // Assert
        var expectedCategory = TestResultBuilder.GetExpectedCategory(resultCategory.Id, request);
        Assert.Equivalent(expectedCategory, resultCategory, strict: true);

        var savedCategory = await _dbContext.Categories
            .FirstOrDefaultAsync(t => t.Id == resultCategory.Id);

        Assert.NotNull(savedCategory);
        expectedCategory.Tasks = new List<TaskResponseDto>();
        Assert.Equivalent(expectedCategory, savedCategory);
    }
}
