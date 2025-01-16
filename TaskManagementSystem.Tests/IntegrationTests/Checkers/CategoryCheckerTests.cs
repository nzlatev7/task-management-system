using TaskManagementSystem.Checkers;
using TaskManagementSystem.Database;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Checkers;

public sealed class CategoryCheckerTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly TestDataManager _dataGenerator;

    private readonly ICategoryChecker _categoryChecker;

    public CategoryCheckerTests(TestDatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _dataGenerator = new TestDataManager(_dbContext);

        _categoryChecker = new CategoryChecker(_dbContext);
    }

    #region CategoryExists

    [Fact]
    public async Task CategoryExistsAsync_CategoryExists_ReturnsTrue()
    {
        // Arrange
        var categoryId = await _dataGenerator.InsertCategoryAsync();

        // Act
        var result = await _categoryChecker.CategoryExistsAsync(categoryId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CategoryExistsAsync_CategoryDoesNotExist_ReturnsFalse()
    {
        // Arrange
        await _dataGenerator.InsertCategoryAsync();

        // Act
        var result = await _categoryChecker.CategoryExistsAsync(categoryId: 1000);

        // Assert
        Assert.False(result);
    }

    #endregion
}
