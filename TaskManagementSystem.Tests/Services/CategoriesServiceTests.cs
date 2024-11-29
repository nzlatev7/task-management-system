using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Services;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.Services;

public sealed class CategoriesServiceTests : IDisposable
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly TestDataGenerator _dataGenerator;
    private readonly CategoriesSerivce _categoriesService;

    public CategoriesServiceTests()
    {
        var options = new DbContextOptionsBuilder<TaskManagementSystemDbContext>()
            .UseInMemoryDatabase("TestDb-CategoriesServiceTests")
            .Options;

        _dbContext = new TaskManagementSystemDbContext(options);
        _dataGenerator = new TestDataGenerator(_dbContext);
        _categoriesService = new CategoriesSerivce(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    #region CreateCategory

    [Fact]
    public async Task CreateCategoryAsync_CategoryCreated_ReturnsTheCategory()
    {
        // Arrange
        var categoryDto = new CategoryRequestDto
        {
            Name = "Test",
            Description = "Test Description",
        };

        // Act
        var result = await _categoriesService.CreateCategoryAsync(categoryDto);

        // Assert
        AssertHelper.Equal(categoryDto, result);

        var savedCategory = await _dbContext.Categories
            .FirstOrDefaultAsync(t => t.Id == result.Id);

        Assert.NotNull(savedCategory);
        AssertHelper.Equal(categoryDto, savedCategory);
    }

    #endregion

    #region GetAllCategories

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        // Arrange
        var categories = await _dataGenerator.InsertCategories(count: 2);

        // Act
        var result = await _categoriesService.GetAllCategoriesAsync();

        // Assert            
        var expectedResult = TestResultBuilder.GetExpectedCategories(categories);
        Assert.Equivalent(expectedResult, result, strict: true);
    }

    #endregion

    #region GetCategoryById

    [Fact]
    public async Task GetCategoryByIdAsync_ReturnsExpectedCategory()
    {
        // Arrange
        var categories = await _dataGenerator.InsertCategories(count: 2);
        var targetCategory = categories[0];

        // Act
        var result = await _categoriesService.GetCategoryByIdAsync(targetCategory.Id);

        // Assert            
        Assert.NotNull(result);
        AssertHelper.Equal(targetCategory, result);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_CategoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistingCategoryId = 1000;
        var categories = await _dataGenerator.InsertCategories(count: 2);

        // Act
        var result = await _categoriesService.GetCategoryByIdAsync(nonExistingCategoryId);

        // Assert            
        Assert.Null(result);
    }

    #endregion

    #region UdateCategory

    [Fact]
    public async Task UdateCategoryAsync_CategoryExists_ReturnsUpdatedCategory()
    {
        // Arrange
        var categories = await _dataGenerator.InsertCategories(count: 2);
        var targetCategory = categories[0];

        var categoryDto = new CategoryRequestDto
        {
            Name = "Updated",
            Description = targetCategory.Description,
        };

        // Act
        var result = await _categoriesService.UpdateCategoryAsync(targetCategory.Id, categoryDto);

        // Assert            
        Assert.NotNull(result);
        AssertHelper.Equal(targetCategory, result);

        var updatedCategory = await _dbContext.Categories
            .FirstOrDefaultAsync(t => t.Id == result.Id);

        Assert.NotNull(updatedCategory);
        AssertHelper.Equal(targetCategory, updatedCategory);
    }

    [Fact]
    public async Task UdateCategoryAsync_CategoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistingCategoryId = 1000;
        var categories = await _dataGenerator.InsertCategories(count: 2);

        var categoryDto = new CategoryRequestDto
        {
            Name = "Updated",
            Description = nameof(CategoryRequestDto.Description),
        };

        // Act
        var result = await _categoriesService.UpdateCategoryAsync(nonExistingCategoryId, categoryDto);

        // Assert            
        Assert.Null(result);
    }

    #endregion

    #region DeleteCategory

    [Fact]
    public async Task DeleteCategoryAsync_CategoryExists_CategoryDeleted_ReturnsTrue()
    {
        // Arrange
        var categories = await _dataGenerator.InsertCategories(count: 2);
        var targetCategoryId = categories[0].Id;

        // Act
        var result = await _categoriesService.DeleteCategoryAsync(targetCategoryId);

        // Assert            
        Assert.True(result);

        var count = await _dbContext.Categories.CountAsync();
        var expectedCount = categories.Count - 1;

        Assert.Equal(expectedCount, count);

        var deletedCategory = await _dbContext.Categories
            .FirstOrDefaultAsync(x => x.Id == targetCategoryId);

        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task DeleteCategoryAsync_CategoryDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var nonExistingCategoryId = 1000;
        var categories = await _dataGenerator.InsertCategories(count: 2);

        var categoryDto = new CategoryRequestDto
        {
            Name = "Updated",
            Description = nameof(CategoryRequestDto.Description),
        };

        // Act
        var result = await _categoriesService.DeleteCategoryAsync(nonExistingCategoryId);

        // Assert            
        Assert.False(result);

        var count = await _dbContext.Categories.CountAsync();
        Assert.Equal(categories.Count, count);
    }

    #endregion

    #region GetTasksByCategory

    [Fact]
    public async Task GetTasksByCategoryAsync_ReturnsTasksForThisCategory()
    {
        // Arrange
        var categories = await _dataGenerator.InsertCategories(count: 2);
        var targetCategoryId = categories[0].Id;

        var targetTasks = await _dataGenerator.InsertTasks(count: 2, targetCategoryId);
        await _dataGenerator.InsertTasks(count: 2, categoryId: categories[1].Id);

        // Act
        var result = await _categoriesService.GetTasksByCategoryAsync(targetCategoryId);

        // Assert            
        var expectedResult = TestResultBuilder.GetExpectedTasks(targetTasks);
        Assert.Equivalent(expectedResult, result);
    }

    #endregion

    #region CategoryExists

    [Fact]
    public async Task CategoryExistsAsync_CategoryExists_ReturnsTrue()
    {
        // Arrange
        var categories = await _dataGenerator.InsertCategories(count: 1);
        var targetCategoryId = categories[0].Id;

        // Act
        var result = await _categoriesService.CategoryExistsAsync(targetCategoryId);

        // Assert            
        Assert.True(result);
    }

    [Fact]
    public async Task CategoryExistsAsync_CategoryDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var categories = await _dataGenerator.InsertCategories(count: 1);

        // Act
        var result = await _categoriesService.CategoryExistsAsync(categoryId: 9999);

        // Assert            
        Assert.False(result);
    }

    #endregion
}