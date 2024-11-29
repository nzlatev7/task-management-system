using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.Services;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.Services;

public sealed class TasksServiceTests : IDisposable
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly Mock<ICategoriesService> _categoriesServiceMock;
    private readonly TestDataGenerator _dataGenerator;

    private readonly TasksService _tasksService;

    public TasksServiceTests()
    {
        var options = new DbContextOptionsBuilder<TaskManagementSystemDbContext>()
            .UseInMemoryDatabase("TestDb-TasksServiceTests")
            .Options;

        _dbContext = new TaskManagementSystemDbContext(options);
        _dataGenerator = new TestDataGenerator(_dbContext);

        _categoriesServiceMock = new Mock<ICategoriesService>();
        _tasksService = new TasksService(_dbContext, _categoriesServiceMock.Object);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    #region CreateTask

    [Fact]
    public async Task CreateTaskAsync_CategoryExists_TaskCreated_ReturnsTask()
    {
        // Arrange
        var taskDto = new TaskRequestDto
        {
            Title = "New Task",
            Description = "Task description",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false,
            CategoryId = 1
        };

        _categoriesServiceMock.Setup(c => c.CategoryExistsAsync(taskDto.CategoryId))
            .ReturnsAsync(true);

        // Act
        var result = await _tasksService.CreateTaskAsync(taskDto);

        // Assert
        Assert.Equal(taskDto.Title, result.Title);
        Assert.Equal(taskDto.Description, result.Description);
        Assert.Equal(taskDto.DueDate, result.DueDate);
        Assert.Equal(taskDto.IsCompleted, result.IsCompleted);

        var savedTask = await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == result.Id);

        Assert.NotNull(savedTask);
        Assert.Equal(taskDto.Title, savedTask.Title);
        Assert.Equal(taskDto.Description, savedTask.Description);
        Assert.Equal(taskDto.DueDate, savedTask.DueDate);
        Assert.Equal(taskDto.IsCompleted, savedTask.IsCompleted);

        _categoriesServiceMock.Verify(c => c.CategoryExistsAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task CreateTaskAsync_CategoryDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var taskDto = new TaskRequestDto
        {
            Title = "New Task",
            Description = "Task description",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false,
            CategoryId = 10000
        };

        _categoriesServiceMock.Setup(c => c.CategoryExistsAsync(taskDto.CategoryId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _tasksService.CreateTaskAsync(taskDto));
        Assert.Equal(ValidationMessages.CategoryDoesNotExist, exception.Message);
    }

    #endregion

    #region GetAllTasks

    [Fact]
    public async Task GetAllTasksAsync_ReturnsAllTasks()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasks(count: 2, categoryId: 1);

        // Act
        var result = await _tasksService.GetAllTasksAsync();

        // Assert
        var expectedTasks = TestResultBuilder.GetExpectedTasks(tasks);
        Assert.Equivalent(expectedTasks, result, strict: true);
    }

    #endregion

    #region GetTaskById

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsExpectedTask()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasks(count: 2, categoryId: 1);
        var expectedTask = tasks[0];

        // Act
        var result = await _tasksService.GetTaskByIdAsync(expectedTask.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTask.Id, result.Id);
        Assert.Equal(expectedTask.Title, result.Title);
        Assert.Equal(expectedTask.Description, result.Description);
        Assert.Equal(expectedTask.DueDate, result.DueDate);
        Assert.Equal(expectedTask.IsCompleted, result.IsCompleted);
    }

    [Fact]
    public async Task GetTaskByIdAsync_TaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasks(count: 2, categoryId: 1);

        // Act
        var result = await _tasksService.GetTaskByIdAsync(taskId: 1000);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region UpdateTask

    [Fact]
    public async Task UpdateTaskAsync_TaskExists_CategoryExists_TaskUpdated_ReturnsTask()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasks(count: 2, categoryId: 1);
        var targetTask = tasks[0];

        targetTask.Title = "Updated Title";

        var taskDto = new TaskRequestDto()
        {
            Title = targetTask.Title,
            Description = targetTask.Description,
            DueDate = targetTask.DueDate,
            IsCompleted = targetTask.IsCompleted,
        };

        _categoriesServiceMock.Setup(c => c.CategoryExistsAsync(taskDto.CategoryId))
            .ReturnsAsync(true);

        // Act
        var result = await _tasksService.UpdateTaskAsync(targetTask.Id, taskDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetTask.Id, result.Id);
        Assert.Equal(targetTask.Title, result.Title);
        Assert.Equal(targetTask.Description, result.Description);
        Assert.Equal(targetTask.DueDate, result.DueDate);
        Assert.Equal(targetTask.IsCompleted, result.IsCompleted);

        var updatedTask = await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == result.Id);

        Assert.NotNull(updatedTask);
        Assert.Equal(taskDto.Title, updatedTask.Title);
        Assert.Equal(taskDto.Description, updatedTask.Description);
        Assert.Equal(taskDto.DueDate, updatedTask.DueDate);
        Assert.Equal(taskDto.IsCompleted, updatedTask.IsCompleted);

        _categoriesServiceMock.Verify(c => c.CategoryExistsAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_TaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistingTaskId = 1000;

        var tasks = await _dataGenerator.InsertTasks(count: 2, categoryId: 1);
        var targetTask = tasks[0];

        targetTask.Title = "Updated Title";

        var taskDto = new TaskRequestDto()
        {
            Title = targetTask.Title,
            Description = targetTask.Description,
            DueDate = targetTask.DueDate,
            IsCompleted = targetTask.IsCompleted,
        };

        // Act
        var result = await _tasksService.UpdateTaskAsync(nonExistingTaskId, taskDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateTaskAsync_TaskExists_CategoryDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasks(count: 2, categoryId: 1);
        var targetTask = tasks[0];

        targetTask.Title = "Updated Title";

        var taskDto = new TaskRequestDto()
        {
            Title = targetTask.Title,
            Description = targetTask.Description,
            DueDate = targetTask.DueDate,
            IsCompleted = targetTask.IsCompleted,
        };

        _categoriesServiceMock.Setup(c => c.CategoryExistsAsync(taskDto.CategoryId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _tasksService.UpdateTaskAsync(targetTask.Id, taskDto));
        Assert.Equal(ValidationMessages.CategoryDoesNotExist, exception.Message);
    }

    #endregion

    #region DeleteTask

    [Fact]
    public async Task DeleteTaskAsync_TaskExists_TaskDeleted_ReturnsTrue()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasks(count: 2, categoryId: 1);
        var targetTaskId = tasks[0].Id;

        // Act
        var result = await _tasksService.DeleteTaskAsync(targetTaskId);

        // Assert
        Assert.True(result);

        var count = _dbContext.Tasks.Count();
        var expectedCount = tasks.Count - 1;
        Assert.Equal(expectedCount, count);

        var deletedTask = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == targetTaskId);
        Assert.Null(deletedTask);
    }

    [Fact]
    public async Task DeleteTaskAsync_TaskDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var tasks = await _dataGenerator.InsertTasks(count: 2, categoryId: 1);

        // Act
        var result = await _tasksService.DeleteTaskAsync(taskId: 1000);

        // Assert
        Assert.False(result);

        var count = _dbContext.Tasks.Count();
        Assert.Equal(tasks.Count, count);
    }

    #endregion


}
