using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Features.Categories.Shared;
using TaskManagementSystem.Features.Shared.DTOs;
using TaskManagementSystem.Features.Categories.DTOs;

namespace TaskManagementSystem.Tests.UnitTests.Controllers;

public sealed class CategoriesContollerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CategoriesController _categoriesController;

    public CategoriesContollerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _categoriesController = new CategoriesController(_mediatorMock.Object);
    }

    #region CreateCategory

    [Fact]
    public async Task CreateCategory_ReturnsOkResultWithCreatedCategory()
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

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateCategoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _categoriesController.CreateCategory(categoryForCreate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedCategory, okResult.Value);

        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateCategoryCommand>(), It.IsAny<CancellationToken>()), Times.Once);
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

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllCategoriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _categoriesController.GetAllCategories();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);

        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllCategoriesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
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
            Id = categoryId,
            Name = "test1",
            Description = "test1"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _categoriesController.GetCategoryById(categoryId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedCategory, okResult.Value);

        _mediatorMock.Verify(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
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
            Id = categoryId,
            Name = categoryForUpdate.Name
        };


        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateCategoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _categoriesController.UpdateCategory(categoryId, categoryForUpdate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedCategory, okResult.Value);

        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateCategoryCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteCategory

    [Fact]
    public async Task DeleteCategory_ReturnsOkResult()
    {
        // Arrange
        var categoryId = 1;

        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteCategoryCommand>(), It.IsAny<CancellationToken>()));

        // Act
        var result = await _categoriesController.DeleteCategory(categoryId);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);

        _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteCategoryCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetTasksByCategory

    [Fact]
    public async Task GetTasksByCategory_ReturnsOkResultWithAllTasksToCategory()
    {
        int categoryId = 1;

        var expectedResponse = new List<TaskResponseDto>()
        {
            new TaskResponseDto() { Title = "tast1" },
            new TaskResponseDto() { Title = "tast2" },
            new TaskResponseDto() { Title = "tast3" },
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetTasksForCategoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _categoriesController.GetTasksByCategory(categoryId);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okObjectResult.Value);

        _mediatorMock.Verify(m => m.Send(It.IsAny<GetTasksForCategoryQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetCompletionStatusForCategory

    [Fact]
    public async Task GetCompletionStatusForCategory_ReturnsOkResultWithCategoryCompletionStatus()
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

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCompletionStatusForCategoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _categoriesController.GetCompletionStatusForCategory(categoryId);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okObjectResult.Value);

        _mediatorMock.Verify(m => m.Send(It.IsAny<GetCompletionStatusForCategoryQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}