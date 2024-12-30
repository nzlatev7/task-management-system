using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.LoggingDecorators;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.UnitTests.LoggingDecorators;

public sealed class CategoriesServiceLoggingDecoratorTests
{
    private readonly Mock<ICategoriesService> _categoriesServiceMock;
    private readonly Mock<ILogger<ICategoriesService>> _loggerMock;
    private readonly CategoriesServiceLoggingDecorator _categoriesServiceLoggingDecorator;

    public CategoriesServiceLoggingDecoratorTests()
    {
        _categoriesServiceMock = new Mock<ICategoriesService>();
        _loggerMock = new Mock<ILogger<ICategoriesService>>();

        _categoriesServiceLoggingDecorator = new CategoriesServiceLoggingDecorator(_categoriesServiceMock.Object, _loggerMock.Object);
    }

    #region CreateCategory

    [Fact]
    public async Task CreateCategoryAsync_LogsCreateInformation_ReturnsCreatedCategory()
    {
        // Arrange
        var categoryForCreate = new CategoryRequestDto
        {
            Name = "test",
            Description = "test"
        };

        var expectedCategory = new CategoryResponseDto()
        {
            Name = categoryForCreate.Name,
            Description = categoryForCreate.Description,
        };

        _categoriesServiceMock.Setup(service => service.CreateCategoryAsync(categoryForCreate))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _categoriesServiceLoggingDecorator.CreateCategoryAsync(categoryForCreate);

        // Assert
        Assert.Equal(expectedCategory, result);

        _categoriesServiceMock.Verify(service => service.CreateCategoryAsync(categoryForCreate), Times.Once);

        var message = string.Format(LoggingMessageConstants.CategoryCreatedSuccessfully, result.Id);
        _loggerMock.VerifyCallForLogInformationAndMessage(message);
    }

    #endregion

    #region GetAllCategories

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        // Arrange
        var expectedResponse = new List<CategoryResponseDto>()
        {
            new CategoryResponseDto() {Name = "test1" },
            new CategoryResponseDto() {Name = "test2" }
        };

        _categoriesServiceMock.Setup(service => service.GetAllCategoriesAsync())
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _categoriesServiceLoggingDecorator.GetAllCategoriesAsync();

        // Assert
        Assert.Equal(expectedResponse, result);

        _categoriesServiceMock.Verify(service => service.GetAllCategoriesAsync(), Times.Once);
    }

    #endregion

    #region GetCategoryById

    [Fact]
    public async Task GetCategoryByIdAsync_ReturnsTheSpecifiedCategory()
    {
        // Arrange
        var categoryId = 1;
        var expectedCategory = new CategoryResponseDto()
        {
            Id = categoryId,
            Name = "test1",
            Description = "test1"
        };

        _categoriesServiceMock.Setup(service => service.GetCategoryByIdAsync(categoryId))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _categoriesServiceLoggingDecorator.GetCategoryByIdAsync(categoryId);

        // Assert
        Assert.Equal(expectedCategory, result);

        _categoriesServiceMock.Verify(service => service.GetCategoryByIdAsync(categoryId), Times.Once);
    }

    #endregion

    #region UpdateCategory

    [Fact]
    public async Task UpdateCategoryAsync_LogsUpdateInformation_ReturnsUpdatedCategory()
    {
        // Arrange
        var categoryId = 1;
        var categoryForUpdate = new CategoryRequestDto()
        {
            Name = "test1"
        };

        var expectedCategory = new CategoryResponseDto()
        {
            Id = categoryId,
            Name = categoryForUpdate.Name
        };

        _categoriesServiceMock.Setup(service => service.UpdateCategoryAsync(categoryId, categoryForUpdate))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _categoriesServiceLoggingDecorator.UpdateCategoryAsync(categoryId, categoryForUpdate);

        // Assert
        Assert.Equal(expectedCategory, result);

        _categoriesServiceMock.Verify(service => service.UpdateCategoryAsync(categoryId, categoryForUpdate), Times.Once);

        var message = string.Format(LoggingMessageConstants.CategoryUpdatedSuccessfully, categoryId);
        _loggerMock.VerifyCallForLogInformationAndMessage(message);
    }

    #endregion

    #region DeleteCategory

    [Fact]
    public async Task DeleteCategoryAsync_LogsDeleteInformation()
    {
        // Arrange
        var categoryId = 1;

        // Act
        await _categoriesServiceLoggingDecorator.DeleteCategoryAsync(categoryId);

        // Assert
        _categoriesServiceMock.Verify(service => service.DeleteCategoryAsync(categoryId), Times.Once);

        var message = string.Format(LoggingMessageConstants.CategoryDeletedSuccessfully, categoryId);
        _loggerMock.VerifyCallForLogInformationAndMessage(message);
    }

    #endregion

    #region GetTasksByCategory

    [Fact]
    public async Task GetTasksByCategoryAsync_ReturnsAllTasksToCategory()
    {
        int categoryId = 1;

        var expectedResponse = new List<TaskResponseDto>()
        {
            new TaskResponseDto() { Title = "tast1" },
            new TaskResponseDto() { Title = "tast2" },
            new TaskResponseDto() { Title = "tast3" },
        };

        _categoriesServiceMock.Setup(service => service.GetTasksByCategoryAsync(categoryId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _categoriesServiceLoggingDecorator.GetTasksByCategoryAsync(categoryId);

        // Assert
        Assert.Equal(expectedResponse, result);

        _categoriesServiceMock.Verify(service => service.GetTasksByCategoryAsync(categoryId), Times.Once);
    }

    #endregion

    #region GetCompletionStatusForCategory

    [Fact]
    public async Task GetCompletionStatusForCategoryAsync_ReturnsCategoryCompletionStatus()
    {
        int categoryId = 1;

        var expectedResponse = new CategoryCompletionStatusResponseDto()
        {
            CompletionPercentage = 50,
            CompletionStatusStats = new StatusStatisticsDto()
            {
                NumberOfPendingTasks = 1,
                NumberOfInProgressTasks = 1,
                NumberOfCompletedTasks = 1,
                NumberOfArchivedTasks = 1,
                NumberOfLockedTasks = 1,
            }
        };

        _categoriesServiceMock.Setup(service => service.GetCompletionStatusForCategoryAsync(categoryId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _categoriesServiceLoggingDecorator.GetCompletionStatusForCategoryAsync(categoryId);

        // Assert
        Assert.Equal(expectedResponse, result);

        _categoriesServiceMock.Verify(service => service.GetCompletionStatusForCategoryAsync(categoryId), Times.Once);
    }

    #endregion
}
