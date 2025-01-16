using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManagementSystem.Checkers;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Features.Categories.DTOs;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Categories.GetCompletionStatusForCategory;

[Collection(nameof(CategoryHandlerTestCollection))]
public sealed class GetCompletionStatusForCategoryHandlerTests : IAsyncLifetime
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly Mock<ICategoryChecker> _categoryCheckerMock;
    private readonly TestDataManager _dataGenerator;
    private List<CategoryEntity> _categories = new();

    private readonly GetCompletionStatusForCategoryHandler _handler;

    public GetCompletionStatusForCategoryHandlerTests(TestDatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _dataGenerator = new TestDataManager(_dbContext);
        _categoryCheckerMock = new Mock<ICategoryChecker>();

        _handler = new GetCompletionStatusForCategoryHandler(_dbContext, _categoryCheckerMock.Object);
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
    public async Task Handle_CategoryExists_ReturnsCategoryCompletionStatus()
    {
        // Arrange
        var request = new GetCompletionStatusForCategoryQuery(id: _categories[0].Id);

        var targetCompletedTasks = await _dataGenerator.InsertTasksAsync(count: 2, request.Id, tasksStatus: Status.Completed);
        var targetInProgressTasks = await _dataGenerator.InsertTasksAsync(count: 1, request.Id, tasksStatus: Status.InProgress);
        var targetLockedTasks = await _dataGenerator.InsertTasksAsync(count: 1, request.Id, tasksStatus: Status.Locked);

        await _dataGenerator.InsertTasksAsync(count: 2, categoryId: _categories[1].Id);

        _categoryCheckerMock.Setup(x => x.CategoryExistsAsync(request.Id))
            .ReturnsAsync(true);

        // Act
        var resultCompletionStatus = await _handler.Handle(request, new CancellationToken());

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
            .Where(x => x.Id == request.Id)
            .Select(x => x.CompletionPercentage)
            .FirstOrDefaultAsync();

        Assert.Equal(expectedCompletionStatus, updatedCompletionPercentage);

        _categoryCheckerMock.VerifyCategoryExistsCall();
    }

    [Fact]
    public async Task Handle_CategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var request = new GetCompletionStatusForCategoryQuery(id: 1000);

        _categoryCheckerMock.Setup(x => x.CategoryExistsAsync(request.Id))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, new CancellationToken()));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);

        _categoryCheckerMock.VerifyCategoryExistsCall();
    }

    [Fact]
    public async Task Handle_NoTasksForCategory_ReturnsCategoryCompletionStatus_WithZeroStatatistics()
    {
        var request = new GetCompletionStatusForCategoryQuery(id: _categories[0].Id);

        _categoryCheckerMock.Setup(x => x.CategoryExistsAsync(request.Id))
            .ReturnsAsync(true);

        // Act
        var resultCompletionStatus = await _handler.Handle(request, new CancellationToken());

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
            .Where(x => x.Id == request.Id)
            .Select(x => x.CompletionPercentage)
            .FirstOrDefaultAsync();

        Assert.Equal(expectedCompletionStatus, updatedCompletionPercentage);
    }
}
