using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Tasks;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.UnitTests.Features.Tasks.DeleteTask;

public sealed class DeleteTaskLoggingHandlerTests
{
    private readonly Mock<ILogger<DeleteTaskLoggingHandler>> _loggerMock;
    private readonly DeleteTaskLoggingHandler _loggingHandler;

    public DeleteTaskLoggingHandlerTests()
    {
        _loggerMock = new Mock<ILogger<DeleteTaskLoggingHandler>>();
        _loggingHandler = new DeleteTaskLoggingHandler(_loggerMock.Object);
    }

    [Theory]
    [InlineData(DeleteAction.Removed)]
    [InlineData(DeleteAction.Moved)]
    [InlineData(DeleteAction.Locked)]
    public async Task Handle_LogsDeleteInformation_ReturnsProperDeleteAction(DeleteAction deleteAction)
    {
        // Arrange
        var request = new DeleteTaskCommand(id: 1);

        var requestHandlerMock = new Mock<RequestHandlerDelegate<DeleteAction>>();
        requestHandlerMock.Setup(x => x())
            .ReturnsAsync(deleteAction);

        // Act
        var result = await _loggingHandler.Handle(request, requestHandlerMock.Object, new CancellationToken());

        // Assert
        Assert.Equal(deleteAction, result);

        requestHandlerMock.Verify(x => x(), Times.Once);

        var message = GetDeleteActionMessage(result, request.Id);
        _loggerMock.VerifyLogInformationMessage(message);
    }

    private string GetDeleteActionMessage(DeleteAction deleteAction, int taskId) => deleteAction switch
    {
        DeleteAction.Removed => string.Format(LoggingMessageConstants.TaskRemovedSuccessfully, taskId),
        DeleteAction.Moved => string.Format(LoggingMessageConstants.TaskMovedSuccessfully, taskId),
        DeleteAction.Locked => string.Format(LoggingMessageConstants.TaskLockedSuccessfully, taskId),
        _ => string.Empty
    };
}
