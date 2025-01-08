using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.UnitTests.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryLoggingHandlerTests
{
    private readonly Mock<ILogger<DeleteCategoryLoggingHandler>> _loggerMock;
    private readonly DeleteCategoryLoggingHandler _loggingHandler;

    public DeleteCategoryLoggingHandlerTests()
    {
        _loggerMock = new Mock<ILogger<DeleteCategoryLoggingHandler>>();
        _loggingHandler = new DeleteCategoryLoggingHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_LogsDeleteInformation()
    {
        // Arrange
        var request = new DeleteCategoryCommand(id: 1);
        var requestHandlerMock = new Mock<RequestHandlerDelegate<Unit>>();

        // Act
        await _loggingHandler.Handle(request, requestHandlerMock.Object, new CancellationToken());

        // Assert
        var message = string.Format(LoggingMessageConstants.CategoryDeletedSuccessfully, request.Id);
        _loggerMock.VerifyLogInformationMessage(message);
    }
}
