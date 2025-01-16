using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Tests.TestUtilities;

public static class MockVerificationHelper
{
    public static void VerifyLogInformationMessage<T>(this Mock<ILogger<T>> loggerMock, string message)
    {
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, t) => state.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    public static void VerifyCategoryExistsCall(this Mock<ICategoryChecker> categoryCheckerMock)
    {
        categoryCheckerMock.Verify(c => c.CategoryExistsAsync(It.IsAny<int>()), Times.Once);
    }
}
