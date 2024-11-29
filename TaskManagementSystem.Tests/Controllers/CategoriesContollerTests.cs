using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Tests.Controllers;

public sealed class CategoriesContollerTests
{
    private readonly Mock<ICategoriesService> _categoriesServiceMock;
    private readonly CategoriesController _categoriesController;

    public CategoriesContollerTests()
    {
        _categoriesServiceMock = new Mock<ICategoriesService>();
        _categoriesController = new CategoriesController(_categoriesServiceMock.Object);
    }

    #region CreateCategory

    [Fact]
    public async Task CreateCategory_ReturnsOkResultWithCreatedCategory()
    {
        // Arrange
        var categoryForCreate = new CategoryRequestDto
        {
            Name = "test"
        };

        var expectedCategory = new CategoryResponseDto()
        {
            Name = categoryForCreate.Name
        };

        _categoriesServiceMock.Setup(service => service.CreateCategoryAsync(categoryForCreate))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _categoriesController.CreateCategory(categoryForCreate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedCategory, okResult.Value);
    }

    [Fact]
    public async Task CreateCategory_UnexpectedExceptionOccurs_Throws()
    {
        // Arrange
        var categoryForCreate = new CategoryRequestDto
        {
            Name = "test"
        };

        _categoriesServiceMock
            .Setup(service => service.CreateCategoryAsync(categoryForCreate))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _categoriesController.CreateCategory(categoryForCreate));
    }

    #endregion

    #region GetAllCategories

    [Fact]
    public async Task GetAllCategories_ReturnsOkResultWithAllCategories()
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
        var result = await _categoriesController.GetAllCategories();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    [Fact]
    public async Task GetAllCateogries_UnexpectedExceptionOccurs_Throws()
    {
        // Arrange
        _categoriesServiceMock
            .Setup(service => service.GetAllCategoriesAsync())
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _categoriesController.GetAllCategories());
    }

    #endregion

    #region GetCategoryById

    [Fact]
    public async Task GetCategoryById_ReturnsOkResultWithTheSpecifiedCategory()
    {
        // Arrange
        var categoryId = 1;
        var expectedCategory = new CategoryResponseDto()
        {
            Name = "test1"
        };

        _categoriesServiceMock.Setup(service => service.GetCategoryByIdAsync(categoryId))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _categoriesController.GetCategoryById(categoryId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedCategory, okResult.Value);
    }

    [Fact]
    public async Task GetCategoryById_WithNonExistingCategory_ReturnsNotFoundResult()
    {
        // Arrange
        var categoryId = 1;

        _categoriesServiceMock.Setup(service => service.GetCategoryByIdAsync(categoryId));

        // Act
        var result = await _categoriesController.GetCategoryById(categoryId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetTaskById_UnexpectedExceptionOccurs_Throws()
    {
        // Arrange
        var categoryId = 1;
        _categoriesServiceMock
            .Setup(service => service.GetCategoryByIdAsync(categoryId))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _categoriesController.GetCategoryById(categoryId));
    }

    #endregion

    #region UpdateCategory

    [Fact]
    public async Task UpdateCategory_ReturnsOkResultWithUpdatedCategory()
    {
        // Arrange
        var categoryId = 1;
        var categoryForUpdate = new CategoryRequestDto()
        {
            Name = "test1"
        };

        var expectedCategory = new CategoryResponseDto()
        {
            Name = categoryForUpdate.Name
        };

        _categoriesServiceMock.Setup(service => service.UpdateCategoryAsync(categoryId, categoryForUpdate))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _categoriesController.UpdateCategory(categoryId, categoryForUpdate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedCategory, okResult.Value);
    }

    [Fact]
    public async Task UpdateCategory_WithNonExistingCategory_ReturnsNotFoundResult()
    {
        // Arrange
        var categoryId = 1;
        var categoryForUpdate = new CategoryRequestDto()
        {
            Name = "test1"
        };

        _categoriesServiceMock.Setup(service => service.UpdateCategoryAsync(categoryId, categoryForUpdate));

        // Act
        var result = await _categoriesController.UpdateCategory(categoryId, categoryForUpdate);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateCategory_UnexpectedExceptionOccurs_Throws()
    {
        // Arrange
        var categoryId = 1;
        var categoryForUpdate = new CategoryRequestDto()
        {
            Name = "test1"
        };

        _categoriesServiceMock
            .Setup(service => service.UpdateCategoryAsync(categoryId, categoryForUpdate))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _categoriesController.UpdateCategory(categoryId, categoryForUpdate));
    }

    #endregion

    #region DeleteCategory

    [Fact]
    public async Task DeleteCategory_ReturnsOkResult()
    {
        // Arrange
        var categoryId = 1;

        _categoriesServiceMock.Setup(service => service.DeleteCategoryAsync(categoryId))
            .ReturnsAsync(true);

        // Act
        var result = await _categoriesController.DeleteCategory(categoryId);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteCategory_WithNonExistingCategory_ReturnsNotFoundResult()
    {
        // Arrange
        var categoryId = 1;
        _categoriesServiceMock.Setup(service => service.DeleteCategoryAsync(categoryId))
            .ReturnsAsync(false);

        // Act
        var result = await _categoriesController.DeleteCategory(categoryId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteCategory_UnexpectedExceptionOccurs_Throws()
    {
        // Arrange
        var categoryId = 1;
        _categoriesServiceMock
            .Setup(service => service.DeleteCategoryAsync(categoryId))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _categoriesController.DeleteCategory(categoryId));
    }

    #endregion

    #region GetTasksByCategory

    [Fact]
    public async Task GetTasksByCategory_ReturnsOkResultWithAllTasksToCategory()
    {
        int categoryId = 1;

        var expectedResponse = new List<TaskResponseDto>()
        {
            new TaskResponseDto() { Title = "tast1", DueDate = DateTime.UtcNow.AddDays(1) },
            new TaskResponseDto() { Title = "tast2", DueDate = DateTime.UtcNow.AddDays(2) },
            new TaskResponseDto() { Title = "tast3", DueDate = DateTime.UtcNow.AddDays(3) },
        };

        _categoriesServiceMock.Setup(service => service.CategoryExistsAsync(categoryId))
            .ReturnsAsync(true);
        _categoriesServiceMock.Setup(service => service.GetTasksByCategoryAsync(categoryId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _categoriesController.GetTasksByCategory(categoryId);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okObjectResult.Value);
    }

    [Fact]
    public async Task GetTasksByCategory_WithNonExistingCategory_ReturnsNotFoundResult()
    {
        int categoryId = 1;
        _categoriesServiceMock.Setup(service => service.CategoryExistsAsync(categoryId))
            .ReturnsAsync(false);

        // Act
        var result = await _categoriesController.GetTasksByCategory(categoryId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetTasksByCategory_UnexpectedExceptionOccurs_Throws()
    {
        // Arrange
        var categoryId = 1;

        _categoriesServiceMock.Setup(service => service.CategoryExistsAsync(categoryId))
            .ReturnsAsync(true);
        _categoriesServiceMock
            .Setup(service => service.GetTasksByCategoryAsync(categoryId))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _categoriesController.GetTasksByCategory(categoryId));
    }

    #endregion
}