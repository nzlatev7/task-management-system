using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
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

    public static void VerifyCallForCategoryExists(this Mock<ICategoryChecker> categoryCheckerMock)
    {
        categoryCheckerMock.Verify(c => c.CategoryExistsAsync(It.IsAny<int>()), Times.Once);
    }

    public static void VerifyTaskCallForGetDeleteStrategy(this Mock<ITaskDeleteFactory> taskDeleteFactoryMock)
    {
        taskDeleteFactoryMock.Verify(c => c.GetDeleteStrategy(It.IsAny<Priority>()), Times.Once);
    }

    public static void VerifyTaskStrategyCallForDelete(this Mock<ITaskDeleteStategy> taskDeleteStrategy)
    {
        taskDeleteStrategy.Verify(c => c.DeleteAsync(It.IsAny<TaskEntity>(), It.IsAny<TaskManagementSystemDbContext>()), Times.Once);
    }
}
