using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Tasks;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Tasks.GetAllTasks;

[Collection(nameof(TaskHandlerTestCollection))]
public sealed class GetAllTasksHandlerTests : IAsyncLifetime
{
    private readonly TestDataManager _dataGenerator;
    private int[] _categoryIds = new int[0];

    private readonly GetAllTasksHandler _handler;

    public GetAllTasksHandlerTests(TestDatabaseFixture fixture)
    {
        _dataGenerator = new TestDataManager(fixture.DbContext);
        _handler = new GetAllTasksHandler(fixture.DbContext);
    }

    public async Task InitializeAsync()
    {
        var categories = await _dataGenerator.InsertCategoriesAsync(count: 2);
        _categoryIds = categories.Select(x => x.Id).ToArray();
    }

    public async Task DisposeAsync()
    {
        await _dataGenerator.ClearTestDataAsync();
    }

    [Theory]
    [MemberData(nameof(SortingTaskPropertyTestData))]
    public async Task Handle_SortBySortingTaskProperty_ReturnsAllTasks_OrderedBySortingTaskPropertyCorrectly(SortingTaskProperty property, bool isAscending)
    {
        // Arrange
        var baseDueDate = new DateTime(2024, 1, 1, 1, 1, 1, DateTimeKind.Utc);
        var task1 = await _dataGenerator.InsertTaskAsync(title: "abv", baseDueDate.AddDays(2), priority: Priority.Low, status: Status.Completed, _categoryIds[0]);
        var task2 = await _dataGenerator.InsertTaskAsync(title: "bvg", baseDueDate.AddDays(1), priority: Priority.High, status: Status.InProgress, _categoryIds[1]);

        var request = new GetAllTasksQuery()
        {
            Property = property,
            IsAscending = isAscending,
        };

        // Act
        var result = await _handler.Handle(request, new CancellationToken());

        // Assert
        var tasks = new List<TaskEntity>
        {
            task1,
            task2
        };

        var expectedTasks = TestResultBuilder.GetExpectedTasks(tasks);
        var orderedExpectedTasks = TestResultBuilder.GetOrderedTasks(expectedTasks, request);
        Assert.Equivalent(orderedExpectedTasks, result, strict: true);

        var expectedIds = orderedExpectedTasks.Select(x => x.Id).ToList();
        var actualIds = result.Select(x => x.Id).ToList();
        Assert.Equal(expectedIds, actualIds);
    }

    public static IEnumerable<object[]> SortingTaskPropertyTestData
    => Enum.GetValues(typeof(SortingTaskProperty))
        .Cast<SortingTaskProperty>()
        .SelectMany(property => new[] { true, false }
            .Select(isAscending => new object[] { property, isAscending }));
}
