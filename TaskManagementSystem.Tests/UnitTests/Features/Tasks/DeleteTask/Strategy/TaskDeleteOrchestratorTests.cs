using Moq;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Tasks.DeleteTask.Interfaces;
using TaskManagementSystem.Features.Tasks.DeleteTask.Strategy;

namespace TaskManagementSystem.Tests.UnitTests.Features.Tasks.DeleteTask.Strategy;

public sealed class TaskDeleteOrchestratorTests
{
    private readonly List<Mock<ITaskDeleteStrategy>> _strategyMocks;
    private readonly ITaskDeleteOrchestrator _taskDeleteOrchestrator;

    private readonly Mock<TaskManagementSystemDbContext> _dbContext;

    public TaskDeleteOrchestratorTests()
    {
        _dbContext = new Mock<TaskManagementSystemDbContext>();
        _strategyMocks = new List<Mock<ITaskDeleteStrategy>>
        {
            new Mock<ITaskDeleteStrategy>()
        };

        var strategies = _strategyMocks.Select(m => m.Object);
        _taskDeleteOrchestrator = new TaskDeleteOrchestrator(strategies);
    }

    #region ExecuteDeletionAsync

    [Fact]
    public async Task ExecuteDeletionAsync_SelectsExecutableStrategy_DelegatesDeletion()
    {
        // Arrange
        var strategy = _strategyMocks[0];

        strategy.Setup(x => x.CanExecute(It.IsAny<TaskEntity>()))
            .Returns(true);

        var deleteAction = DeleteAction.Removed;
        strategy.Setup(x => x.DeleteAsync(It.IsAny<TaskEntity>(), It.IsAny<TaskManagementSystemDbContext>()))
            .ReturnsAsync(deleteAction);

        var taskEntity = new TaskEntity()
        {
            Title = "Test",
        };

        // Act
        var result = await _taskDeleteOrchestrator.ExecuteDeletionAsync(taskEntity, _dbContext.Object);

        // Assert
        Assert.Equal(deleteAction, result);

        strategy.Verify(x => x.CanExecute(taskEntity), Times.Once);
        strategy.Verify(x => x.DeleteAsync(taskEntity, _dbContext.Object), Times.Once);
    }

    [Fact]
    public async Task ExecuteDeletionAsync_DeletionCanNotBeExecuted_ThrowsInvalidOperationException()
    {
        // Arrange
        var taskEntity = new TaskEntity()
        {
            Title = "Test",
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _taskDeleteOrchestrator.ExecuteDeletionAsync(taskEntity, _dbContext.Object));
        Assert.Equal(ErrorMessageConstants.NoStrategyFound, exception.Message);
    }

    #endregion
}