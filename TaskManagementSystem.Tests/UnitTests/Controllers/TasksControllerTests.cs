using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Tests.UnitTests.Controllers;

public sealed class TasksControllerTests
{
    private readonly Mock<ITasksService> _tasksServiceMock;
    private readonly TasksController _tasksController;

    public TasksControllerTests()
    {
        _tasksServiceMock = new Mock<ITasksService>();
        _tasksController = new TasksController(_tasksServiceMock.Object);
    }

    #region CreateTask

    [Fact]
    public async Task CreateTask_ReturnsOkResultWithCreatedTask()
    {
        // Arrange
        var taskForCreate = new CreateTaskRequestDto
        {
            Title = "test",
            DueDate = DateTime.UtcNow.AddDays(2),
            Description = "test",
            Priority = Priority.Low,
            CategoryId = 1,
            Kind = TaskKind.Feature,
            StoryPoints = 5
        };

        var expectedTask = new TaskResponseDto()
        {
            Title = taskForCreate.Title,
            DueDate = taskForCreate.DueDate,
            Description = taskForCreate.Description,
            Priority = taskForCreate.Priority.Value,
            CategoryId = taskForCreate.CategoryId,
            StoryPoints = taskForCreate.StoryPoints,
            Kind = TaskKind.Feature
        };

        _tasksServiceMock.Setup(service => service.CreateTaskAsync(taskForCreate))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _tasksController.CreateTask(taskForCreate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedTask, okResult.Value);

        _tasksServiceMock.Verify(services => services.CreateTaskAsync(taskForCreate), Times.Once);
    }

    #endregion

    #region CloneTask

    [Fact]
    public async Task CloneTask_ReturnsOkResultWithClonedTask()
    {
        // Arrange
        var taskId = 12;
        var cloneRequest = new CloneTaskRequestDto
        {
            Title = "Copy"
        };

        var clonedTask = new TaskResponseDto
        {
            Id = 33,
            Title = cloneRequest.Title
        };

        _tasksServiceMock.Setup(service => service.CloneTaskAsync(taskId, cloneRequest))
            .ReturnsAsync(clonedTask);

        // Act
        var result = await _tasksController.CloneTask(taskId, cloneRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(clonedTask, okResult.Value);

        _tasksServiceMock.Verify(service => service.CloneTaskAsync(taskId, cloneRequest), Times.Once);
    }

    #endregion

    #region GetAllTasks

    [Fact]
    public async Task GetAllTasks_SortByInstructionsProvided_ReturnsOkResultWithAllTasks_OrderedBySortByInstructions()
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
        var result = await _tasksController.GetAllTasks(sortByInstructions);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);

        _tasksServiceMock.Verify(services => services.GetAllTasksAsync(sortByInstructions), Times.Once);
    }

    #endregion

    #region GetBacklog

    [Fact]
    public async Task GetBacklog_TaskKindProvided_ReturnsOkResultWithOrderedBacklog()
    {
        // Arrange
        var kind = TaskKind.Feature;
        var expectedResponse = new List<TaskResponseDto>()
        {
            new TaskResponseDto() { Title = "1" },
            new TaskResponseDto() { Title = "2" }
        };

        _tasksServiceMock.Setup(service => service.GetBacklogAsync(kind))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _tasksController.GetBacklog(kind);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);

        _tasksServiceMock.Verify(services => services.GetBacklogAsync(kind), Times.Once);
    }

    #endregion

    #region GetTaskById

    [Fact]
    public async Task GetTaskById_ReturnsOkResultWithTheSpecifiedTask()
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
        var result = await _tasksController.GetTaskById(taskId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedTask, okResult.Value);

        _tasksServiceMock.Verify(services => services.GetTaskByIdAsync(taskId), Times.Once);
    }

    #endregion

    #region UpdateTask

    [Fact]
    public async Task UpdateTask_ReturnsOkResultWithUpdatedTask()
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
        var result = await _tasksController.UpdateTask(taskId, taskForUpdate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedTask, okResult.Value);

        _tasksServiceMock.Verify(services => services.UpdateTaskAsync(taskId, taskForUpdate), Times.Once);
    }

    #endregion

    #region UnlockTask

    [Fact]
    public async Task UnlockTask_ReturnsNoContentResult()
    {
        // Arrange
        var taskId = 1;

        var unlockDto = new UnlockTaskRequestDto()
        {
            Status = Status.InProgress
        };


        // Act
        var result = await _tasksController.UnlockTask(taskId, unlockDto);

        // Assert
        var okResult = Assert.IsType<NoContentResult>(result);

        _tasksServiceMock.Verify(services => services.UnlockTaskAsync(taskId, unlockDto), Times.Once);
    }

    #endregion

    #region DeleteTask

    [Fact]
    public async Task DeleteTask_ReturnsOkResultWithDeleteAction()
    {
        // Arrange
        var taskId = 1;
        var expectedDeleteAction = DeleteAction.Locked;

        _tasksServiceMock.Setup(service => service.DeleteTaskAsync(taskId))
            .ReturnsAsync(expectedDeleteAction);

        // Act
        var result = await _tasksController.DeleteTask(taskId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedDeleteAction, okResult.Value);

        _tasksServiceMock.Verify(services => services.DeleteTaskAsync(taskId), Times.Once);
    }

    #endregion
}
