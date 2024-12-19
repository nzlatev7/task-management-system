using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Services;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests;

public sealed class ReportsServiceTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly TestDataManager _dataGenerator;
    private readonly ReportsService _reportsService;

    private List<CategoryEntity> categories = new();

    public ReportsServiceTests(TestDatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _dataGenerator = new TestDataManager(_dbContext);

        _reportsService = new ReportsService(_dbContext);
    }

    public async Task InitializeAsync()
    {
        categories = await _dataGenerator.InsertCategoriesAsync(count: 2);
    }

    public async Task DisposeAsync()
    {
        await _dataGenerator.ClearTestDataAsync();
    }

    #region GetReportForTasks

    [Fact]
    public async Task GetReportForTasksAsync_AllFiltersProvided_ReturnsFilteredTasksGroupedByCategory()
    {
        // Arrange
        var notMatchingTask1 = await _dataGenerator.InsertTasksAsync(count: 1, categories[0].Id, tasksPriority: Priority.Medium, tasksStatus: Status.InProgress);
        var notMatchingTask2 = await _dataGenerator.InsertTasksAsync(count: 1, categories[0].Id, tasksPriority: Priority.Low, tasksStatus: Status.Pending);

        var targetTasks1 = await _dataGenerator.InsertTasksAsync(count: 4, categories[0].Id, tasksPriority: Priority.High, tasksStatus: Status.Locked);
        var targetTasks2 = await _dataGenerator.InsertTasksAsync(count: 4, categories[1].Id, tasksPriority: Priority.High, tasksStatus: Status.Locked);

        var filtersDto = new ReportTasksRequestDto
        {
            Status = targetTasks1[0].Status,
            Priority = targetTasks1[0].Priority,
            DueAfter = targetTasks1[0].DueDate,
            DueBefore = targetTasks1[3].DueDate
        };

        // Act
        var tasksReport = await _reportsService.GetReportForTasksAsync(filtersDto);

        // Assert
        var filteredTasks = notMatchingTask1
            .Concat(notMatchingTask2)
            .Concat(targetTasks1)
            .Concat(targetTasks2)
            .Where(x => x.Status == filtersDto.Status 
                && x.Priority == filtersDto.Priority 
                && x.DueDate > filtersDto.DueAfter 
                && x.DueDate < filtersDto.DueBefore)
            .ToList();

        var expectedTasksReport = TestResultBuilder.GetExpectedReport(categories, filteredTasks);
        Assert.Equivalent(expectedTasksReport, tasksReport);
    }

    [Fact]
    public async Task GetReportForTasksAsync_FiltersNotProvided_ReturnsAllTasksGroupedByCategory()
    {
        // Arrange
        var targetTasks1 = await _dataGenerator.InsertTasksAsync(count: 1, categories[0].Id, tasksPriority: Priority.Medium, tasksStatus: Status.InProgress);
        var targetTasks2 = await _dataGenerator.InsertTasksAsync(count: 2, categories[0].Id, tasksPriority: Priority.Low, tasksStatus: Status.InProgress);
        var targetTasks3 = await _dataGenerator.InsertTasksAsync(count: 2, categories[1].Id, tasksPriority: Priority.Low, tasksStatus: Status.Completed);
        var targetTasks4 = await _dataGenerator.InsertTasksAsync(count: 1, categories[1].Id, tasksPriority: Priority.High, tasksStatus: Status.Locked);

        // Act
        var tasksReport = await _reportsService.GetReportForTasksAsync(new ReportTasksRequestDto());

        // Assert
        var allTasks = targetTasks1
            .Concat(targetTasks2)
            .Concat(targetTasks3)
            .Concat(targetTasks4)
            .ToList();

        var expectedTasksReport = TestResultBuilder.GetExpectedReport(categories, allTasks);
        Assert.Equivalent(expectedTasksReport, tasksReport);
    }

    #endregion
}
