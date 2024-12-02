using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Tests.Controllers;

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
        var taskForCreate = new TaskRequestDto 
        { 
            Title = "test",
            DueDate = DateTime.UtcNow.AddDays(2),
            CategoryId = 1
        };

        var expectedTask = new TaskResponseDto() 
        { 
            Title = taskForCreate.Title,
            DueDate = taskForCreate.DueDate,
            CategoryId = taskForCreate.CategoryId
        };

        _tasksServiceMock.Setup(service => service.CreateTaskAsync(taskForCreate))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _tasksController.CreateTask(taskForCreate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedTask, okResult.Value);
    }

    [Fact]
    public async Task CreateTask_WithNonExistingCategory_ReturnsBadRequestWithProperMessage()
    {
        // Arrange
        var taskForCreate = new TaskRequestDto
        {
            Title = "test",
            DueDate = DateTime.UtcNow.AddDays(2)
        };

        var expectedMessage = ValidationMessages.CategoryDoesNotExist;

        _tasksServiceMock.Setup(service => service.CreateTaskAsync(taskForCreate))
                .ThrowsAsync(new ArgumentException(expectedMessage));

        // Act
        var result = await _tasksController.CreateTask(taskForCreate);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(expectedMessage, badRequestResult.Value);
    }

    [Fact]
    public async Task CreateTask_UnexpectedExceptionOccurs_Throws()
    {
        // Arrange
        var taskForCreate = new TaskRequestDto
        {
            Title = "test",
            DueDate = DateTime.UtcNow.AddDays(2)
        };

        _tasksServiceMock
            .Setup(service => service.CreateTaskAsync(taskForCreate))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tasksController.CreateTask(taskForCreate));
    }

    #endregion

    #region GetAllTasks

    [Fact]
    public async Task GetAllTasks_ReturnsOkResultWithAllTasks()
    {
        // Arrange
        var expectedResponse = new List<TaskResponseDto>()
        {
            new TaskResponseDto() {Title = "1", DueDate = DateTime.UtcNow.AddDays(1) },
            new TaskResponseDto() {Title = "2", DueDate = DateTime.UtcNow.AddDays(2) }
        };

        _tasksServiceMock.Setup(service => service.GetAllTasksAsync())
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _tasksController.GetAllTasks();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    [Fact]
    public async Task GetAllTasks_UnexpectedExceptionOccurs_Throws()
    {
        // Arrange
        _tasksServiceMock
            .Setup(service => service.GetAllTasksAsync())
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tasksController.GetAllTasks());
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
            DueDate = DateTime.UtcNow.AddDays(1),
            CategoryId = 1
        };

        _tasksServiceMock.Setup(service => service.GetTaskByIdAsync(taskId))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _tasksController.GetTaskById(taskId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedTask, okResult.Value);
    }

    [Fact]
    public async Task GetTaskById_WithNonExistingTask_ReturnsNotFoundResult()
    {
        // Arrange
        var taskId = 1;
        _tasksServiceMock.Setup(service => service.GetTaskByIdAsync(taskId));

        // Act
        var result = await _tasksController.GetTaskById(taskId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetTaskById_UnexpectedExceptionOccurs_Throws()
    {
        // Arrange
        var taskId = 1;
        _tasksServiceMock
            .Setup(service => service.GetTaskByIdAsync(taskId))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tasksController.GetTaskById(taskId));
    }

    #endregion

    #region UpdateTask

    [Fact]
    public async Task UpdateTask_ReturnsOkResultWithUpdatedTask()
    {
        // Arrange
        var taskId = 1;
        var taskForUpdate = new TaskRequestDto
        {
            Title = "test",
            DueDate = DateTime.UtcNow.AddDays(2),
            CategoryId = 1
        };

        var expectedTask = new TaskResponseDto()
        {
            Title = taskForUpdate.Title,
            DueDate = taskForUpdate.DueDate,
            CategoryId = taskForUpdate.CategoryId
        };

        _tasksServiceMock.Setup(service => service.UpdateTaskAsync(taskId, taskForUpdate))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _tasksController.UpdateTask(taskId, taskForUpdate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedTask, okResult.Value);
    }

    [Fact]
    public async Task UpdateTask_WithNonExistingTask_ReturnsNotFoundResult()
    {
        // Arrange
        var taskId = 1;
        var taskForUpdate = new TaskRequestDto
        {
            Title = "test",
            DueDate = DateTime.UtcNow.AddDays(2),
            CategoryId = 1
        };

        _tasksServiceMock.Setup(service => service.UpdateTaskAsync(taskId, taskForUpdate));

        // Act
        var result = await _tasksController.UpdateTask(taskId, taskForUpdate);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateTask_WithNonExistingCategory_ReturnsBadRequestWithProperMessage()
    {
        // Arrange
        int taskId = 1;
        var taskForUpdate = new TaskRequestDto
        {
            Title = "test",
            DueDate = DateTime.UtcNow.AddDays(2)
        };

        var expectedMessage = ValidationMessages.CategoryDoesNotExist;

        _tasksServiceMock.Setup(service => service.UpdateTaskAsync(taskId, taskForUpdate))
                .ThrowsAsync(new ArgumentException(expectedMessage));

        // Act
        var result = await _tasksController.UpdateTask(taskId, taskForUpdate);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(expectedMessage, badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateTask_UnexpectedExceptionOccurs_Throws()
    {
        // Arrange
        int taskId = 1;
        var taskForUpdate = new TaskRequestDto
        {
            Title = "test",
            DueDate = DateTime.UtcNow.AddDays(2)
        };

        _tasksServiceMock
            .Setup(service => service.UpdateTaskAsync(taskId, taskForUpdate))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tasksController.UpdateTask(taskId, taskForUpdate));
    }

    #endregion

    #region DeleteTask

    [Fact]
    public async Task DeleteTask_ReturnsOkResult()
    {
        // Arrange
        var taskId = 1;

        _tasksServiceMock.Setup(service => service.DeleteTaskAsync(taskId))
            .ReturnsAsync(true);

        // Act
        var result = await _tasksController.DeleteTask(taskId);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteTask_WithNonExistingTask_ReturnsNotFoundResult()
    {
        // Arrange
        var taskId = 1;
        _tasksServiceMock.Setup(service => service.DeleteTaskAsync(taskId))
            .ReturnsAsync(false);

        // Act
        var result = await _tasksController.DeleteTask(taskId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteTask_UnexpectedExceptionOccurs_Throws()
    {
        // Arrange
        var taskId = 1;
        _tasksServiceMock
            .Setup(service => service.DeleteTaskAsync(taskId))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tasksController.DeleteTask(taskId));
    }

    #endregion
}
