using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Tests.TestUtilities;

public static class MockVerification
{
    public static void VerifyCallForLogInformationAndMessage<T>(this Mock<ILogger<T>> loggerMock, string message)
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

    public static void VerifyCallForCategoryExists(this Mock<ICategoryRepository> categoryRepositoryMock)
    {
        categoryRepositoryMock.Verify(c => c.CategoryExistsAsync(It.IsAny<int>()), Times.Once);
    }
}
