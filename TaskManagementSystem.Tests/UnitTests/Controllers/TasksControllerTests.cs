using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Tasks;
using TaskManagementSystem.Features.Shared.DTOs;
using TaskManagementSystem.Features.Tasks.DTOs;

namespace TaskManagementSystem.Tests.UnitTests.Controllers;

public sealed class TasksControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly TasksController _tasksController;

    public TasksControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _tasksController = new TasksController(_mediatorMock.Object);
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

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateTaskCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _tasksController.CreateTask(taskForCreate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedTask, okResult.Value);

        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateTaskCommand>(), It.IsAny<CancellationToken>()), Times.Once);
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

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllTasksQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _tasksController.GetAllTasks(sortByInstructions);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);

        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllTasksQuery>(), It.IsAny<CancellationToken>()), Times.Once);
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

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetTaskByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _tasksController.GetTaskById(taskId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedTask, okResult.Value);

        _mediatorMock.Verify(m => m.Send(It.IsAny<GetTaskByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
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

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateTaskCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _tasksController.UpdateTask(taskId, taskForUpdate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedTask, okResult.Value);

        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateTaskCommand>(), It.IsAny<CancellationToken>()), Times.Once);
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

        _mediatorMock.Verify(m => m.Send(It.IsAny<UnlockTaskCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteTask

    [Fact]
    public async Task DeleteTask_ReturnsOkResultWithDeleteAction()
    {
        // Arrange
        var taskId = 1;
        var expectedDeleteAction = DeleteAction.Locked;

        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteTaskCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDeleteAction);

        // Act
        var result = await _tasksController.DeleteTask(taskId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedDeleteAction, okResult.Value);

        _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteTaskCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
