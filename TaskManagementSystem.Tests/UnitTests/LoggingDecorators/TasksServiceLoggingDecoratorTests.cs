using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.LoggingDecorators;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.UnitTests.LoggingDecorators;

public sealed class TasksServiceLoggingDecoratorTests
{
    private readonly Mock<ITasksService> _tasksServiceMock;
    private readonly Mock<ILogger<ITasksService>> _loggerMock;
    private readonly TasksServiceLoggingDecorator _tasksServiceLoggingDecorator;

    public TasksServiceLoggingDecoratorTests()
    {
        _tasksServiceMock = new Mock<ITasksService>();
        _loggerMock = new Mock<ILogger<ITasksService>>();

        _tasksServiceLoggingDecorator = new TasksServiceLoggingDecorator(_tasksServiceMock.Object, _loggerMock.Object);
    }

    #region CreateTask

    [Fact]
    public async Task CreateTaskAsync_LogsCreateInformation_ReturnsCreatedTask()
    {
        // Arrange
        var taskForCreate = new CreateTaskRequestDto
        {
            Title = "test",
            DueDate = DateTime.UtcNow.AddDays(2),
            Description = "test",
            Priority = Priority.Low,
            CategoryId = 1
        };

        var expectedTask = new TaskResponseDto()
        {
            Title = taskForCreate.Title,
            DueDate = taskForCreate.DueDate,
            Description = taskForCreate.Description,
            Priority = taskForCreate.Priority.Value,
            CategoryId = taskForCreate.CategoryId
        };

        _tasksServiceMock.Setup(service => service.CreateTaskAsync(taskForCreate))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _tasksServiceLoggingDecorator.CreateTaskAsync(taskForCreate);

        // Assert
        Assert.Equal(expectedTask, result);

        _tasksServiceMock.Verify(services => services.CreateTaskAsync(taskForCreate), Times.Once);

        var message = string.Format(LoggingMessageConstants.TaskCreatedSuccessfully, result.Id, result.CategoryId);
        _loggerMock.VerifyCallForLogInformationAndMessage(message);
    }

    #endregion

    #region GetAllTasks

    [Fact]
    public async Task GetAllTasksAsync_SortByInstructionsProvided_ReturnsAllTasks_OrderedBySortByInstructions()
    {
        // Arrange
        var expectedResponse = new List<TaskResponseDto>()
        {
            new TaskResponseDto() {Title = "1"},
            new TaskResponseDto() {Title = "2"}
        };

        var sortByInstructions = new GetAllTasksRequestDto()
        {
            Property = SortingTaskProperty.Title,
            IsAscending = true
        };

        _tasksServiceMock.Setup(service => service.GetAllTasksAsync(sortByInstructions))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _tasksServiceLoggingDecorator.GetAllTasksAsync(sortByInstructions);

        // Assert
        Assert.Equal(expectedResponse, result);

        _tasksServiceMock.Verify(services => services.GetAllTasksAsync(sortByInstructions), Times.Once);
    }

    #endregion

    #region GetTaskById

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsTheSpecifiedTask()
    {
        // Arrange
        var taskId = 1;
        var expectedTask = new TaskResponseDto()
        {
            Title = "1",
        };

        _tasksServiceMock.Setup(service => service.GetTaskByIdAsync(taskId))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _tasksServiceLoggingDecorator.GetTaskByIdAsync(taskId);

        // Assert
        Assert.Equal(expectedTask, result);

        _tasksServiceMock.Verify(services => services.GetTaskByIdAsync(taskId), Times.Once);
    }

    #endregion

    #region UpdateTask

    [Fact]
    public async Task UpdateTaskAsync_LogsUpdateInformation_ReturnsUpdatedTask()
    {
        // Arrange
        var taskId = 1;
        var taskForUpdate = new UpdateTaskRequestDto
        {
            Title = "test",
            Status = Status.Completed,
        };

        var expectedTask = new TaskResponseDto()
        {
            Id = taskId,
            Title = taskForUpdate.Title,
            Status = taskForUpdate.Status
        };

        _tasksServiceMock.Setup(service => service.UpdateTaskAsync(taskId, taskForUpdate))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _tasksServiceLoggingDecorator.UpdateTaskAsync(taskId, taskForUpdate);

        // Assert
        Assert.Equal(expectedTask, result);

        _tasksServiceMock.Verify(services => services.UpdateTaskAsync(taskId, taskForUpdate), Times.Once);

        var message = string.Format(LoggingMessageConstants.TaskUpdatedSuccessfully, taskId);
        _loggerMock.VerifyCallForLogInformationAndMessage(message);
    }

    #endregion

    #region UnLockTask

    [Fact]
    public async Task UnlockTaskAsync_LogsUnlockInformation()
    {
        // Arrange
        var taskId = 1;

        var unlockDto = new UnlockTaskRequestDto()
        {
            Status = Status.InProgress
        };

        // Act
        await _tasksServiceLoggingDecorator.UnlockTaskAsync(taskId, unlockDto);

        // Assert
        _tasksServiceMock.Verify(services => services.UnlockTaskAsync(taskId, unlockDto), Times.Once);

        var message = string.Format(LoggingMessageConstants.TaskUnlockedSuccessfully, taskId);
        _loggerMock.VerifyCallForLogInformationAndMessage(message);
    }

    #endregion

    #region DeleteTask

    [Theory]
    [InlineData(DeleteAction.Removed)]
    [InlineData(DeleteAction.Moved)]
    [InlineData(DeleteAction.Locked)]
    public async Task DeleteTaskAsync_LogsProperDeleteInformation_ReturnsDeleteAction(DeleteAction deleteAction)
    {
        // Arrange
        var taskId = 1;

        _tasksServiceMock.Setup(service => service.DeleteTaskAsync(taskId))
            .ReturnsAsync(deleteAction);

        // Act
        var result = await _tasksServiceLoggingDecorator.DeleteTaskAsync(taskId);

        // Assert
        Assert.Equal(deleteAction, result);

        _tasksServiceMock.Verify(services => services.DeleteTaskAsync(taskId), Times.Once);

        var properMessage = GetDeleteActionMessage(deleteAction, taskId);
        _loggerMock.VerifyCallForLogInformationAndMessage(properMessage);
    }

    #endregion

    private string GetDeleteActionMessage(DeleteAction deleteAction, int taskId) => deleteAction switch
    {
        DeleteAction.Removed => string.Format(LoggingMessageConstants.TaskRemovedSuccessfully, taskId),
        DeleteAction.Moved => string.Format(LoggingMessageConstants.TaskMovedSuccessfully, taskId),
        DeleteAction.Locked => string.Format(LoggingMessageConstants.TaskLockedSuccessfully, taskId),
        _ => string.Empty
    };
}
