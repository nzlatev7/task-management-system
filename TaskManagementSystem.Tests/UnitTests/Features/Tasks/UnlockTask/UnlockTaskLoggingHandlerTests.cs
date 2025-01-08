using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Features.Tasks;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.UnitTests.Features.Tasks.UnlockTask;

public sealed class UnlockTaskLoggingHandlerTests
{
    private readonly Mock<ILogger<UnlockTaskLoggingHandler>> _loggerMock;
    private readonly UnlockTaskLoggingHandler _loggingHandler;

    public UnlockTaskLoggingHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UnlockTaskLoggingHandler>>();
        _loggingHandler = new UnlockTaskLoggingHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_LogsUnlockInformation()
    {
        // Arrange
        var request = new UnlockTaskCommand(id: 1);
        var requestHandlerMock = new Mock<RequestHandlerDelegate<Unit>>();

        // Act
        await _loggingHandler.Handle(request, requestHandlerMock.Object, new CancellationToken());

        // Assert
        requestHandlerMock.Verify(x => x(), Times.Once);

        var message = string.Format(LoggingMessageConstants.TaskUnlockedSuccessfully, request.Id);
        _loggerMock.VerifyLogInformationMessage(message);
    }
}
