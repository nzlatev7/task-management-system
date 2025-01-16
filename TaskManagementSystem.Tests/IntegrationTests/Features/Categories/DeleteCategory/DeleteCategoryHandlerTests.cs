using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Database;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Exceptions;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Categories.DeleteCategory;

[Collection(nameof(CategoryHandlerTestCollection))]
public sealed class DeleteCategoryHandlerTests : IAsyncLifetime
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly TestDataManager _dataGenerator;
    private List<CategoryEntity> _categories = new();

    private readonly DeleteCategoryHandler _handler;

    public DeleteCategoryHandlerTests(TestDatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _dataGenerator = new TestDataManager(_dbContext);

        _handler = new DeleteCategoryHandler(_dbContext);
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
    public async Task Handle_CategoryExists_CategoryDeleted()
    {
        // Arrange
        var request = new DeleteCategoryCommand(id: _categories[0].Id);

        // Act
        await _handler.Handle(request, new CancellationToken());

        // Assert            
        var count = await _dbContext.Categories.CountAsync();
        Assert.Equal(_categories.Count - 1, count);

        var deletedCategory = await _dbContext.Categories
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task Handle_CategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var request = new DeleteCategoryCommand(id: 1000);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);
    }

    [Fact]
    public async Task Handle_AssociatedTasksToCategory_ThrowsConflictException()
    {
        // Arrange
        var request = new DeleteCategoryCommand(id: _categories[0].Id);
        await _dataGenerator.InsertTasksAsync(count: 1, request.Id);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.AssociatedTasksToCategory, exception.Message);
    }
}
