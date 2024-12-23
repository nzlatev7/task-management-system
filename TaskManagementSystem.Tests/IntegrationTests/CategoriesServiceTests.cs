using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Repositories;
using TaskManagementSystem.Services;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests;

public sealed class CategoriesServiceTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly TestDataManager _dataGenerator;
    private readonly CategoryRepository _categoryRepository;
    private readonly CategoriesSerivce _categoriesService;

    private List<CategoryEntity> categories = new();

    public CategoriesServiceTests(TestDatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _dataGenerator = new TestDataManager(_dbContext);
        _categoryRepository = new CategoryRepository(_dbContext);
        
        _categoriesService = new CategoriesSerivce(_dbContext, _categoryRepository);
    }

    public async Task InitializeAsync()
    {
        categories = await _dataGenerator.InsertCategoriesAsync(2);
    }

    public async Task DisposeAsync()
    {
        await _dataGenerator.ClearTestDataAsync();
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
        var resultCategory = await _categoriesService.CreateCategoryAsync(categoryDto);

        // Assert
        var expectedCategory = TestResultBuilder.GetExpectedCategory(resultCategory.Id, categoryDto);
        Assert.Equivalent(expectedCategory, resultCategory, strict: true);

        var savedCategory = await _dbContext.Categories
            .FirstOrDefaultAsync(t => t.Id == resultCategory.Id);

        Assert.NotNull(savedCategory);
        expectedCategory.Tasks = new List<TaskResponseDto>();
        Assert.Equivalent(expectedCategory, savedCategory);
    }

    #endregion

    #region GetAllCategories

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        // Act
        var resultCategories = await _categoriesService.GetAllCategoriesAsync();

        // Assert
        var expectedResultCategories = TestResultBuilder.GetExpectedCategories(categories);
        Assert.Equivalent(expectedResultCategories, resultCategories, strict: true);
    }

    #endregion

    #region GetCategoryById

    [Fact]
    public async Task GetCategoryByIdAsync_ReturnsExpectedCategory()
    {
        // Arrange
        var targetCategory = categories[0];

        // Act
        var resultCategory = await _categoriesService.GetCategoryByIdAsync(targetCategory.Id);

        // Assert            
        Assert.NotNull(resultCategory);
        var expectedCategory = TestResultBuilder.GetExpectedCategory(targetCategory);
        Assert.Equivalent(expectedCategory, resultCategory, strict: true);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_CategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _categoriesService.GetCategoryByIdAsync(categoryId: 1000));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);
    }

    #endregion

    #region UdateCategory

    [Fact]
    public async Task UdateCategoryAsync_CategoryExists_ReturnsUpdatedCategory()
    {
        // Arrange
        var targetCategory = categories[0];

        var categoryDto = new CategoryRequestDto
        {
            Name = "Updated",
            Description = targetCategory.Description,
        };

        // Act
        var resultCategory = await _categoriesService.UpdateCategoryAsync(targetCategory.Id, categoryDto);

        // Assert
        var expectedResultCategory = TestResultBuilder.GetExpectedCategory(targetCategory);
        Assert.Equivalent(expectedResultCategory, resultCategory);

        var updatedCategory = await _dbContext.Categories
            .FirstOrDefaultAsync(t => t.Id == resultCategory.Id);

        expectedResultCategory.Tasks = new List<TaskResponseDto>();
        Assert.Equivalent(expectedResultCategory, updatedCategory);
    }

    [Fact]
    public async Task UdateCategoryAsync_CategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var categoryDto = new CategoryRequestDto
        {
            Name = "Updated",
            Description = nameof(CategoryRequestDto.Description),
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _categoriesService.UpdateCategoryAsync(categoryId: 1000, categoryDto));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);
    }

    #endregion

    #region DeleteCategory

    [Fact]
    public async Task DeleteCategoryAsync_CategoryExists_CategoryDeleted()
    {
        // Arrange
        var targetCategoryId = categories[0].Id;

        // Act
        await _categoriesService.DeleteCategoryAsync(targetCategoryId);

        // Assert            
        var count = await _dbContext.Categories.CountAsync();
        Assert.Equal(categories.Count - 1, count);

        var deletedCategory = await _dbContext.Categories
            .FirstOrDefaultAsync(x => x.Id == targetCategoryId);

        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task DeleteCategoryAsync_CategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _categoriesService.DeleteCategoryAsync(categoryId: 1000));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);
    }

    [Fact]
    public async Task DeleteCategoryAsync_AssociatedTasksToCategory_ThrowsConflictException()
    {
        // Arrange
        var targetCategoryId = categories[0].Id;
        await _dataGenerator.InsertTasksAsync(count: 1, targetCategoryId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _categoriesService.DeleteCategoryAsync(targetCategoryId));
        Assert.Equal(ErrorMessageConstants.AssociatedTasksToCategory, exception.Message);
    }

    #endregion

    #region GetTasksByCategory

    [Fact]
    public async Task GetTasksByCategoryAsync_ReturnsTasksForThisCategory()
    {
        // Arrange
        var targetCategoryId = categories[0].Id;

        var targetTasks = await _dataGenerator.InsertTasksAsync(count: 2, targetCategoryId);
        await _dataGenerator.InsertTasksAsync(count: 2, categoryId: categories[1].Id);

        // Act
        var resultTasks = await _categoriesService.GetTasksByCategoryAsync(targetCategoryId);

        // Assert
        var expectedResultTasks = TestResultBuilder.GetExpectedTasks(targetTasks);
        Assert.Equivalent(expectedResultTasks, resultTasks, strict: true);
    }

    [Fact]
    public async Task GetTasksByCategoryAsync_CategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _categoriesService.GetTasksByCategoryAsync(categoryId: 1000));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);
    }

    #endregion

    #region GetCompletionStatusForCategory

    [Fact]
    public async Task GetCompletionStatusForCategoryAsync_CategoryExists_ReturnsCategoryCompletionStatus()
    {
        // Arrange
        var targetCategoryId = categories[0].Id;

        var targetCompletedTasks = await _dataGenerator.InsertTasksAsync(count: 2, targetCategoryId, tasksStatus: Status.Completed);
        var targetInProgressTasks = await _dataGenerator.InsertTasksAsync(count: 1, targetCategoryId, tasksStatus: Status.InProgress);
        var targetLockedTasks = await _dataGenerator.InsertTasksAsync(count: 1, targetCategoryId, tasksStatus: Status.Locked);

        await _dataGenerator.InsertTasksAsync(count: 2, categoryId: categories[1].Id);

        // Act
        var resultCompletionStatus = await _categoriesService.GetCompletionStatusForCategoryAsync(targetCategoryId);

        // Assert
        short expectedCompletionStatus = (short)(targetCompletedTasks.Count / (double)(targetInProgressTasks.Count + targetCompletedTasks.Count) * 100);
        Assert.Equal(expectedCompletionStatus, resultCompletionStatus.CompletionPercentage);

        var expectedStatusStatistics = new StatusStatisticsDto()
        {
            NumberOfPendingTasks = 0,
            NumberOfInProgressTasks = targetInProgressTasks.Count,
            NumberOfCompletedTasks = targetCompletedTasks.Count,
            NumberOfArchivedTasks = 0,
            NumberOfLockedTasks = targetLockedTasks.Count
        };

        Assert.Equivalent(expectedStatusStatistics, resultCompletionStatus.CompletionStatusStats, strict: true);

        var updatedCompletionPercentage = await _dbContext.Categories
            .Where(x => x.Id == targetCategoryId)
            .Select(x => x.CompletionPercentage)
            .FirstOrDefaultAsync();

        Assert.Equal(expectedCompletionStatus, updatedCompletionPercentage);
    }

    [Fact]
    public async Task GetCompletionStatusForCategoryAsync_CategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _categoriesService.GetCompletionStatusForCategoryAsync(categoryId: 1000));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);
    }

    [Fact]
    public async Task GetCompletionStatusForCategoryAsync_NoTasksForCategory_ReturnsCategoryCompletionStatus_WithZeroStatatistics()
    {
        var targetCategoryId = categories[0].Id;

        // Act
        var resultCompletionStatus = await _categoriesService.GetCompletionStatusForCategoryAsync(targetCategoryId);

        // Assert
        short expectedCompletionStatus = 0;
        Assert.Equal(expectedCompletionStatus, resultCompletionStatus.CompletionPercentage);

        var expectedStatusStatistics = new StatusStatisticsDto()
        {
            NumberOfPendingTasks = 0,
            NumberOfInProgressTasks = 0,
            NumberOfCompletedTasks = 0,
            NumberOfArchivedTasks = 0,
            NumberOfLockedTasks = 0,
        };

        Assert.Equivalent(expectedStatusStatistics, resultCompletionStatus.CompletionStatusStats, strict: true);

        var updatedCompletionPercentage = await _dbContext.Categories
            .Where(x => x.Id == targetCategoryId)
            .Select(x => x.CompletionPercentage)
            .FirstOrDefaultAsync();

        Assert.Equal(expectedCompletionStatus, updatedCompletionPercentage);
    }

    #endregion
}