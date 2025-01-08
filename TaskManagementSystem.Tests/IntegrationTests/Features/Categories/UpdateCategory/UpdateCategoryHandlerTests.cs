using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Features.Shared.DTOs;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Categories.UpdateCategory;

[Collection(nameof(CategoryHandlerTestCollection))]
public sealed class UpdateCategoryHandlerTests : IAsyncLifetime
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly TestDataManager _dataGenerator;
    private List<CategoryEntity> _categories = new();

    private readonly UpdateCategoryHandler _handler;

    public UpdateCategoryHandlerTests(TestDatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _dataGenerator = new TestDataManager(_dbContext);

        _handler = new UpdateCategoryHandler(_dbContext);
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
    public async Task Handle_CategoryExists_ReturnsUpdatedCategory()
    {
        // Arrange
        var targetCategory = _categories[0];
        var request = new UpdateCategoryCommand(id: targetCategory.Id)
        {
            Name = "Updated",
            Description = targetCategory.Description,
        };

        // Act
        var resultCategory = await _handler.Handle(request, new CancellationToken());

        // Assert
        var expectedResultCategory = TestResultBuilder.GetExpectedCategory(targetCategory);
        Assert.Equivalent(expectedResultCategory, resultCategory);

        var updatedCategory = await _dbContext.Categories
            .FirstOrDefaultAsync(t => t.Id == resultCategory.Id);

        expectedResultCategory.Tasks = new List<TaskResponseDto>();
        Assert.Equivalent(expectedResultCategory, updatedCategory);
    }

    [Fact]
    public async Task Handle_CategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var request = new UpdateCategoryCommand(id: 1000);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);
    }
}
