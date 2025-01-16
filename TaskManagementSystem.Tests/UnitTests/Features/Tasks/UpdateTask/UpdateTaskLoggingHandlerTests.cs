using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Features.Shared.DTOs;
using TaskManagementSystem.Features.Tasks;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.UnitTests.Features.Tasks.UpdateTask;

public sealed class UpdateTaskLoggingHandlerTests
{
    private readonly Mock<ILogger<UpdateTaskLoggingHandler>> _loggerMock;
    private readonly UpdateTaskLoggingHandler _loggingHandler;

    public UpdateTaskLoggingHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateTaskLoggingHandler>>();
        _loggingHandler = new UpdateTaskLoggingHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_LogsUpdateInformation()
    {
        // Arrange
        var request = new UpdateTaskCommand(id: 1)
        {
            Title = "test"
        };

        var expectedTask = new TaskResponseDto()
        {
            Id = request.Id,
            Title = request.Title
        };

        var requestHandlerMock = new Mock<RequestHandlerDelegate<TaskResponseDto>>();
        requestHandlerMock.Setup(x => x())
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _loggingHandler.Handle(request, requestHandlerMock.Object, new CancellationToken());

        // Assert
        requestHandlerMock.Verify(x => x(), Times.Once);

        var message = string.Format(LoggingMessageConstants.TaskUpdatedSuccessfully, request.Id);
        _loggerMock.VerifyLogInformationMessage(message);
    }
}
