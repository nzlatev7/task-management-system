using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Features.Categories.Shared;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.UnitTests.Features.Categories.CreateCategory;

public sealed class CreateCategoryLoggingHandlerTests
{
    private readonly Mock<ILogger<CreateCategoryLoggingHandler>> _loggerMock;
    private readonly CreateCategoryLoggingHandler _loggingHandler;

    public CreateCategoryLoggingHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CreateCategoryLoggingHandler>>();
        _loggingHandler = new CreateCategoryLoggingHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_LogsCreateInformation_ReturnsCreatedCategory()
    {
        // Arrange
        var categoryForCreate = new CreateCategoryCommand
        {
            Name = "test",
        };

        var expectedCategory = new CategoryResponseDto()
        {
            Name = categoryForCreate.Name,
        };

        var requestHandlerMock = new Mock<RequestHandlerDelegate<CategoryResponseDto>>();
        requestHandlerMock.Setup(handler => handler())
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _loggingHandler.Handle(categoryForCreate, requestHandlerMock.Object, new CancellationToken());

        // Assert
        Assert.Equal(expectedCategory, result);

        var message = string.Format(LoggingMessageConstants.CategoryCreatedSuccessfully, result.Id);
        _loggerMock.VerifyLogInformationMessage(message);
    }
}
