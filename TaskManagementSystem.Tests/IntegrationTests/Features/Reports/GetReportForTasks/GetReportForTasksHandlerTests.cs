using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;
using TaskManagementSystem.Features.Reports;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Reports.GetReportForTasks;

[Collection(nameof(ReportHandlerTestCollection))]
public sealed class GetReportForTasksHandlerTests : IAsyncLifetime
{
    private readonly TestDataManager _dataGenerator;
    private readonly GetReportForTasksHandler _handler;

    private List<CategoryEntity> _categories = new();

    public GetReportForTasksHandlerTests(TestDatabaseFixture fixture)
    {
        _dataGenerator = new TestDataManager(fixture.DbContext);

        _handler = new GetReportForTasksHandler(fixture.DbContext);
    }

    public async Task InitializeAsync()
    {
        _categories = await _dataGenerator.InsertCategoriesAsync(count: 2);
    }

    public async Task DisposeAsync()
    {
        await _dataGenerator.ClearTestDataAsync();
    }

    [Fact]
    public async Task Handle_AllFiltersProvided_ReturnsFilteredTasksGroupedByCategory()
    {
        // Arrange
        var notMatchingTask1 = await _dataGenerator.InsertTasksAsync(count: 1, _categories[0].Id, tasksPriority: Priority.Medium, tasksStatus: Status.InProgress);
        var notMatchingTask2 = await _dataGenerator.InsertTasksAsync(count: 1, _categories[0].Id, tasksPriority: Priority.Low, tasksStatus: Status.Pending);

        var targetTasks1 = await _dataGenerator.InsertTasksAsync(count: 4, _categories[0].Id, tasksPriority: Priority.High, tasksStatus: Status.Locked);
        var targetTasks2 = await _dataGenerator.InsertTasksAsync(count: 4, _categories[1].Id, tasksPriority: Priority.High, tasksStatus: Status.Locked);

        var request = new GetReportForTasksQuery()
        {
            Status = targetTasks1[0].Status,
            Priority = targetTasks1[0].Priority,
            DueAfter = targetTasks1[0].DueDate,
            DueBefore = targetTasks1[3].DueDate
        };

        // Act
        var tasksReport = await _handler.Handle(request, new CancellationToken());

        // Assert
        var filteredTasks = notMatchingTask1
            .Concat(notMatchingTask2)
            .Concat(targetTasks1)
            .Concat(targetTasks2)
            .Where(x => x.Status == request.Status
                && x.Priority == request.Priority
                && x.DueDate > request.DueAfter
                && x.DueDate < request.DueBefore)
            .ToList();

        var expectedTasksReport = TestResultBuilder.GetExpectedReport(_categories, filteredTasks);
        Assert.Equivalent(expectedTasksReport, tasksReport);
    }

    [Fact]
    public async Task Handle_FiltersNotProvided_ReturnsAllTasksGroupedByCategory()
    {
        // Arrange
        var targetTasks1 = await _dataGenerator.InsertTasksAsync(count: 1, _categories[0].Id, tasksPriority: Priority.Medium, tasksStatus: Status.InProgress);
        var targetTasks2 = await _dataGenerator.InsertTasksAsync(count: 2, _categories[0].Id, tasksPriority: Priority.Low, tasksStatus: Status.InProgress);
        var targetTasks3 = await _dataGenerator.InsertTasksAsync(count: 2, _categories[1].Id, tasksPriority: Priority.Low, tasksStatus: Status.Completed);
        var targetTasks4 = await _dataGenerator.InsertTasksAsync(count: 1, _categories[1].Id, tasksPriority: Priority.High, tasksStatus: Status.Locked);

        // Act
        var tasksReport = await _handler.Handle(new GetReportForTasksQuery(), new CancellationToken());

        // Assert
        var allTasks = targetTasks1
            .Concat(targetTasks2)
            .Concat(targetTasks3)
            .Concat(targetTasks4)
            .ToList();

        var expectedTasksReport = TestResultBuilder.GetExpectedReport(_categories, allTasks);
        Assert.Equivalent(expectedTasksReport, tasksReport);
    }
}
