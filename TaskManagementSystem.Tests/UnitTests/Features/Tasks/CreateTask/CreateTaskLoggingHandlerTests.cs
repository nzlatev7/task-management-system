using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Features.Shared.DTOs;
using TaskManagementSystem.Features.Tasks;
using TaskManagementSystem.Tests.TestUtilities;

namespace TaskManagementSystem.Tests.UnitTests.Features.Tasks.CreateTask;

public sealed class CreateTaskLoggingHandlerTests
{
    private readonly Mock<ILogger<CreateTaskLoggingHandler>> _loggerMock;
    private readonly CreateTaskLoggingHandler _loggingHandler;

    public CreateTaskLoggingHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CreateTaskLoggingHandler>>();
        _loggingHandler = new CreateTaskLoggingHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_LogsCreateInformation_ReturnsCreatedTask()
    {
        // Arrange
        var request = new CreateTaskCommand
        {
            Title = "test",
            CategoryId = 1
        };

        var expectedTask = new TaskResponseDto()
        {
            Title = request.Title,
            CategoryId = request.CategoryId
        };

        var requestHandlerMock = new Mock<RequestHandlerDelegate<TaskResponseDto>>();
        requestHandlerMock.Setup(x => x())
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _loggingHandler.Handle(request, requestHandlerMock.Object, new CancellationToken());

        // Assert
        Assert.Equal(expectedTask, result);

        requestHandlerMock.Verify(x => x(), Times.Once);

        var message = string.Format(LoggingMessageConstants.TaskCreatedSuccessfully, result.Id, result.CategoryId);
        _loggerMock.VerifyLogInformationMessage(message);
    }
}