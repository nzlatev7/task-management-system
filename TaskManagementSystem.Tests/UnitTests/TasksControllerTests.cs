using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Tests.UnitTests;

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
        var result = await _tasksController.CreateTask(taskForCreate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedTask, okResult.Value);
        _tasksServiceMock.Verify(services => services.CreateTaskAsync(taskForCreate), Times.Once);
    }

    #endregion

    #region GetAllTasks

    [Fact]
    public async Task GetAllTasks_SortByPriorityAscendingEqualsTrue_ReturnsOkResultWithAllTasks_OrderedByPriorityAscending()
    {
        // Arrange
        var sortByPriorityAscending = true;
        var expectedResponse = new List<TaskResponseDto>()
        {
            new TaskResponseDto() {Title = "1"},
            new TaskResponseDto() {Title = "2"}
        };

        _tasksServiceMock.Setup(service => service.GetAllTasksAsync(sortByPriorityAscending))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _tasksController.GetAllTasks(sortByPriorityAscending);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);
        _tasksServiceMock.Verify(services => services.GetAllTasksAsync(sortByPriorityAscending), Times.Once);
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

    #region DeleteTask

    [Fact]
    public async Task DeleteTask_ReturnsOkResult()
    {
        // Arrange
        var taskId = 1;

        // Act
        var result = await _tasksController.DeleteTask(taskId);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        _tasksServiceMock.Verify(services => services.DeleteTaskAsync(taskId), Times.Once);
    }

    #endregion
}
