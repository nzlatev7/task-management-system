using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.Services;
using TaskManagementSystem.Tests.Fixtures;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.IntegrationTests.Services;

public sealed class ScrumTasksServiceTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly TestDataManager _dataGenerator;

    private readonly Mock<ITaskDeleteOrchestrator> _taskDeleteOrchestrator;
    private readonly Mock<ICategoryChecker> _categoryCheckerMock;

    private readonly ITasksService _tasksService;

    private int _targetCategoryId;
    private int[] _categoryIds = new int[0];

    public ScrumTasksServiceTests(TestDatabaseFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _dataGenerator = new TestDataManager(_dbContext);

        _taskDeleteOrchestrator = new Mock<ITaskDeleteOrchestrator>();
        _categoryCheckerMock = new Mock<ICategoryChecker>();

        _tasksService = new ScrumTasksService(_dbContext, _categoryCheckerMock.Object, _taskDeleteOrchestrator.Object);
    }

    public async Task InitializeAsync()
    {
        var categories = await _dataGenerator.InsertCategoriesAsync(count: 2);
        _categoryIds = categories.Select(x => x.Id).ToArray();
        _targetCategoryId = _categoryIds[0];
    }

    public async Task DisposeAsync()
    {
        await _dataGenerator.ClearTestDataAsync();
    }

    #region CreateTask

    [Fact]
    public async Task CreateTaskAsync_CategoryExists_TaskCreated_ReturnsCreatedTask()
    {
        // Arrange
        var taskDto = new CreateTaskRequestDto
        {
            Title = "New Task",
            Description = "Task description",
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Low,
            CategoryId = _targetCategoryId,
            Kind = TaskKind.Feature,
            StoryPoints = 3
        };

        _categoryCheckerMock.Setup(c => c.CategoryExistsAsync(taskDto.CategoryId))
            .ReturnsAsync(true);

        // Act
        var resultTask = await _tasksService.CreateTaskAsync(taskDto);

        // Assert
        var expectedTask = TestResultBuilder.GetExpectedTask(resultTask.Id, taskDto);
        Assert.Equivalent(expectedTask, resultTask, strict: true);

        var count = await _dbContext.Tasks.CountAsync();
        Assert.Equal(expected: 1, count);

        var savedTask = await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == resultTask.Id);

        Assert.NotNull(savedTask);
        Assert.Equivalent(expectedTask, savedTask);

        _categoryCheckerMock.VerifyCategoryExistsCall();
    }

    [Fact]
    public async Task CreateTaskAsync_CategoryDoesNotExist_ThrowsBadHttpRequestException()
    {
        // Arrange
        var taskDto = new CreateTaskRequestDto
        {
            Title = "New Task",
            CategoryId = 10000,
            Kind = TaskKind.Feature,
            StoryPoints = 3
        };

        _categoryCheckerMock.Setup(c => c.CategoryExistsAsync(taskDto.CategoryId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadHttpRequestException>(() => _tasksService.CreateTaskAsync(taskDto));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);

        _categoryCheckerMock.VerifyCategoryExistsCall();
    }

    [Fact]
    public async Task CreateTaskAsync_CategoryExists_PriorityNotProvided_CreatesTaskWithDefaultPriority()
    {
        // Arrange
        var taskDto = new CreateTaskRequestDto
        {
            Title = "New Task",
            CategoryId = _targetCategoryId,
            Kind = TaskKind.Feature,
            StoryPoints = 3
        };

        _categoryCheckerMock.Setup(c => c.CategoryExistsAsync(taskDto.CategoryId))
            .ReturnsAsync(true);

        // Act
        var resultTask = await _tasksService.CreateTaskAsync(taskDto);

        // Assert
        var defaultPriority = Priority.Medium;
        Assert.Equal(defaultPriority, resultTask.Priority);
    }

    [Fact]
    public async Task CreateTaskAsync_BugTask_DoesNotExposeStoryPoints()
    {
        // Arrange
        var taskDto = new CreateTaskRequestDto
        {
            Title = "Bug Task",
            CategoryId = _targetCategoryId,
            Kind = TaskKind.Bug,
            Severity = 5
        };

        _categoryCheckerMock.Setup(c => c.CategoryExistsAsync(taskDto.CategoryId))
            .ReturnsAsync(true);

        // Act
        var resultTask = await _tasksService.CreateTaskAsync(taskDto);

        // Assert
        Assert.Equal(TaskKind.Bug, resultTask.Kind);
        Assert.Null(resultTask.StoryPoints);
    }

    #endregion

    #region GetAllTasks

    [Theory]
    [MemberData(nameof(SortingTaskPropertyTestData))]
    public async Task GetAllTasksAsync_SortBySortingTaskProperty_ReturnsAllTasks_OrderedBySortingTaskPropertyCorrectly(SortingTaskProperty property, bool isAscending)
    {
        // Arrange
        var baseDueDate = new DateTime(2024, 1, 1, 1, 1, 1, DateTimeKind.Utc);
        var task1 = await _dataGenerator.InsertTaskAsync(title: "abv", baseDueDate.AddDays(2), priority: Priority.Low, status: Status.Completed, _targetCategoryId);
        var task2 = await _dataGenerator.InsertTaskAsync(title: "bvg", baseDueDate.AddDays(1), priority: Priority.High, status: Status.InProgress, _categoryIds[1]);

        var sortByDto = new GetAllTasksRequestDto()
        {
            Property = property,
            IsAscending = isAscending,
        };

        // Act
        var result = await _tasksService.GetAllTasksAsync(sortByDto);

        // Assert
        var tasks = new List<TaskEntity>
        {
            task1,
            task2
        };

        var expectedTasks = TestResultBuilder.GetExpectedTasks(tasks);
        var orderedExpectedTasks = TestResultBuilder.GetOrderedTasks(expectedTasks, sortByDto);
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

    #endregion

    #region GetTaskById

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsExpectedTask()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksPriority: Priority.High);
        var targetTask = tasks[0];

        // Act
        var resultTask = await _tasksService.GetTaskByIdAsync(targetTask.Id);

        // Assert
        Assert.NotNull(resultTask);

        var expectedTask = TestResultBuilder.GetExpectedTask(targetTask);
        Assert.Equivalent(expectedTask, resultTask, strict: true);
    }

    [Fact]
    public async Task GetTaskByIdAsync_TaskDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _tasksService.GetTaskByIdAsync(taskId: 1000));
        Assert.Equal(ErrorMessageConstants.TaskDoesNotExist, exception.Message);
    }

    #endregion

    #region UpdateTask

    [Fact]
    public async Task UpdateTaskAsync_TaskExists_ValidTaskStatus_CategoryExists_TaskUpdated_ReturnsUpdatedTask()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksStatus: Status.InProgress);
        var targetTask = tasks[0];

        var taskDto = new UpdateTaskRequestDto()
        {
            Title = "Updated Title",
            Description = targetTask.Description,
            DueDate = targetTask.DueDate,
            Priority = targetTask.Priority,
            Status = targetTask.Status,
            CategoryId = _targetCategoryId
        };

        _categoryCheckerMock.Setup(c => c.CategoryExistsAsync(taskDto.CategoryId))
            .ReturnsAsync(true);

        // Act
        var resultTask = await _tasksService.UpdateTaskAsync(targetTask.Id, taskDto);

        // Assert
        var expectedTask = TestResultBuilder.GetExpectedTask(targetTask.Id, taskDto);
        Assert.Equivalent(expectedTask, resultTask, strict: true);

        var count = await _dbContext.Tasks.CountAsync();
        Assert.Equal(tasks.Count, count);

        var updatedTask = await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == resultTask.Id);

        Assert.NotNull(updatedTask);
        Assert.Equivalent(expectedTask, updatedTask);

        _categoryCheckerMock.VerifyCategoryExistsCall();
    }

    [Fact]
    public async Task UpdateTaskAsync_TaskDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId);

        var taskDto = new UpdateTaskRequestDto()
        {
            Title = "Updated Title",
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _tasksService.UpdateTaskAsync(taskId: 1000, taskDto));
        Assert.Equal(ErrorMessageConstants.TaskDoesNotExist, exception.Message);
    }

    [Fact]
    public async Task UpdateTaskAsync_TaskExists_TargetedArchivedTask_ThrowsConflictException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 1, _targetCategoryId, tasksStatus: Status.Archived);

        var taskDto = new UpdateTaskRequestDto()
        {
            Title = "Updated Title",
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _tasksService.UpdateTaskAsync(tasks[0].Id, taskDto));
        Assert.Equal(ErrorMessageConstants.ArchivedTaskCanNotBeEdited, exception.Message);
    }

    [Fact]
    public async Task UpdateTaskAsync_TaskExists_NotCompletedTask_MovedToArchived_ThrowsConflictException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 1, _targetCategoryId, tasksStatus: Status.Pending);
        var targetTask = tasks[0];

        var taskDto = new UpdateTaskRequestDto()
        {
            Title = targetTask.Title,
            Status = Status.Archived
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _tasksService.UpdateTaskAsync(tasks[0].Id, taskDto));
        Assert.Equal(ErrorMessageConstants.OnlyCompletedTaskCanBeArchived, exception.Message);
    }

    [Fact]
    public async Task UpdateTaskAsync_TaskExists_TargetedLockedTask_ThrowsConflictException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 1, _targetCategoryId, tasksStatus: Status.Locked);
        var targetTask = tasks[0];

        var taskDto = new UpdateTaskRequestDto()
        {
            Title = "Updated Title",
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _tasksService.UpdateTaskAsync(tasks[0].Id, taskDto));
        Assert.Equal(ErrorMessageConstants.LockedTaskCanNotBeEdited, exception.Message);
    }

    [Fact]
    public async Task UpdateTaskAsync_TaskExists_CategoryDoesNotExist_ThrowsBadHttpRequestException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 1, _targetCategoryId);
        var targetTask = tasks[0];

        var taskDto = new UpdateTaskRequestDto()
        {
            Title = targetTask.Title,
            CategoryId = 1000
        };

        _categoryCheckerMock.Setup(c => c.CategoryExistsAsync(taskDto.CategoryId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadHttpRequestException>(() => _tasksService.UpdateTaskAsync(targetTask.Id, taskDto));
        Assert.Equal(ErrorMessageConstants.CategoryDoesNotExist, exception.Message);

        _categoryCheckerMock.VerifyCategoryExistsCall();
    }

    [Fact]
    public async Task UpdateTaskAsync_TaskExists_PriorityNotProvided_UpdatesTaskWithDefaultPriority()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 1, _targetCategoryId);
        var targetTask = tasks[0];

        var taskDto = new UpdateTaskRequestDto
        {
            Title = targetTask.Title,
            CategoryId = targetTask.CategoryId
        };

        _categoryCheckerMock.Setup(c => c.CategoryExistsAsync(taskDto.CategoryId))
            .ReturnsAsync(true);

        // Act
        var resultTask = await _tasksService.UpdateTaskAsync(targetTask.Id, taskDto);

        // Assert
        var defaultPriority = Priority.Medium;
        Assert.Equal(defaultPriority, resultTask.Priority);
    }

    [Fact]
    public async Task UpdateTaskAsync_TaskExists_NotCompletedTask_MovedToCompleted_SetIsCompletedEqaulsTrue()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 1, _targetCategoryId);

        var targetTask = tasks[0];
        var expectedStatus = Status.Completed;

        var taskDto = new UpdateTaskRequestDto
        {
            Title = targetTask.Title,
            Status = expectedStatus,
            CategoryId = targetTask.CategoryId
        };

        _categoryCheckerMock.Setup(c => c.CategoryExistsAsync(taskDto.CategoryId))
            .ReturnsAsync(true);

        // Act
        var resultTask = await _tasksService.UpdateTaskAsync(targetTask.Id, taskDto);

        // Assert
        Assert.True(resultTask.IsCompleted);
        Assert.Equal(expectedStatus, resultTask.Status);
    }

    #endregion

    #region UnlockTask

    [Fact]
    public async Task UnlockTaskAsync_TaskExists_TaskLocked_ChangesTaskStatus()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksStatus: Status.Locked);
        var targetTaskId = tasks[0].Id;

        var expectedStatus = Status.Completed;
        var unlockDto = new UnlockTaskRequestDto()
        {
            Status = expectedStatus,
        };

        // Act
        await _tasksService.UnlockTaskAsync(targetTaskId, unlockDto);

        // Assert
        var updatedStatus = await _dbContext.Tasks
            .Where(x => x.Id == targetTaskId)
            .Select(x => x.Status)
            .FirstOrDefaultAsync();

        Assert.Equal(expectedStatus, updatedStatus);
    }

    [Fact]
    public async Task UnlockTaskAsync_TaskDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksStatus: Status.Locked);

        var unlockDto = new UnlockTaskRequestDto()
        {
            Status = Status.Completed
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _tasksService.UnlockTaskAsync(taskId: 1000, unlockDto));
        Assert.Equal(ErrorMessageConstants.LockedTaskWithIdDoesNotExist, exception.Message);
    }

    [Fact]
    public async Task UnlockTaskAsync_TaskExists_TaskNotLocked_ThrowsNotFoundException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksStatus: Status.Pending);
        var targetTaskId = tasks[0].Id;

        var unlockDto = new UnlockTaskRequestDto()
        {
            Status = Status.Completed
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _tasksService.UnlockTaskAsync(targetTaskId, unlockDto));
        Assert.Equal(ErrorMessageConstants.LockedTaskWithIdDoesNotExist, exception.Message);
    }

    #endregion

    #region DeleteTask

    [Fact]
    public async Task DeleteTaskAsync_TaskExists_ValidTaskStatus_ReturnsProperDeleteAction()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksPriority: Priority.Low, tasksStatus: Status.InProgress);
        var targetTask = tasks[0];

        var deleteAction = DeleteAction.Removed;
        _taskDeleteOrchestrator.Setup(x => x.ExecuteDeletionAsync(It.IsAny<TaskEntity>(), It.IsAny<TaskManagementSystemDbContext>()))
            .ReturnsAsync(deleteAction);

        // Act
        var result = await _tasksService.DeleteTaskAsync(targetTask.Id);

        // Assert
        Assert.Equal(deleteAction, result);

        _taskDeleteOrchestrator.Verify(c => c.ExecuteDeletionAsync(It.IsAny<TaskEntity>(), It.IsAny<TaskManagementSystemDbContext>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskAsync_TaskDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _tasksService.DeleteTaskAsync(taskId: 1000));
        Assert.Equal(ErrorMessageConstants.TaskDoesNotExist, exception.Message);
    }

    [Fact]
    public async Task DeleteTaskAsync_TaskExists_AlreadyLocked_ThrowsConflictException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksStatus: Status.Locked);
        var targetTaskId = tasks[0].Id;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _tasksService.DeleteTaskAsync(targetTaskId));
        Assert.Equal(ErrorMessageConstants.TaskAlreadyLocked, exception.Message);
    }

    [Fact]
    public async Task DeleteTaskAsync_TaskExists_ArchivedTask_ThrowsConflictException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasksAsync(count: 2, _targetCategoryId, tasksStatus: Status.Archived);
        var targetTaskId = tasks[0].Id;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _tasksService.DeleteTaskAsync(targetTaskId));
        Assert.Equal(ErrorMessageConstants.ArchivedTaskCanNotBeDeleted, exception.Message);
    }

    #endregion
}