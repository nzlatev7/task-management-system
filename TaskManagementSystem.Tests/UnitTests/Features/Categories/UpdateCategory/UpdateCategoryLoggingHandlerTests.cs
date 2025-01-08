using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Features.Categories.Shared;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.UnitTests.Features.Categories.UpdateCategory;

public sealed class UpdateCategoryLoggingHandlerTests
{
    private readonly Mock<ILogger<UpdateCategoryLoggingHandler>> _loggerMock;
    private readonly UpdateCategoryLoggingHandler _loggingHandler;

    public UpdateCategoryLoggingHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateCategoryLoggingHandler>>();
        _loggingHandler = new UpdateCategoryLoggingHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_LogsUpdateInformation_ReturnsUpdatedCategory()
    {
        // Arrange
        var request = new UpdateCategoryCommand(id: 1)
        {
            Name = "test1"
        };

        var expectedCategory = new CategoryResponseDto()
        {
            Id = request.Id,
            Name = request.Name
        };

        var requestHandlerMock = new Mock<RequestHandlerDelegate<CategoryResponseDto>>();
        requestHandlerMock.Setup(x => x())
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _loggingHandler.Handle(request, requestHandlerMock.Object, new CancellationToken());

        // Assert
        Assert.Equal(expectedCategory, result);

        var message = string.Format(LoggingMessageConstants.CategoryUpdatedSuccessfully, result.Id);
        _loggerMock.VerifyLogInformationMessage(message);
    }
}
