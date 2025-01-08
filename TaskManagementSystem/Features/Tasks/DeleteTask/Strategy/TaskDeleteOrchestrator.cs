using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Tasks.DeleteTask.Interfaces;

namespace TaskManagementSystem.Features.Tasks.DeleteTask.Strategy;

public sealed class TaskDeleteOrchestrator : ITaskDeleteOrchestrator
{
    private readonly IEnumerable<ITaskDeleteStrategy> _strategies;

    public TaskDeleteOrchestrator(IEnumerable<ITaskDeleteStrategy> stategies)
    {
        _strategies = stategies;
    }

    public async Task<DeleteAction> ExecuteDeletionAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext)
    {
        var strategy = _strategies.FirstOrDefault(s => s.CanExecute(taskEntity))
            ?? throw new InvalidOperationException(ErrorMessageConstants.NoStrategyFound);

        var deleteAction = await strategy.DeleteAsync(taskEntity, dbContext);

        return deleteAction;
    }
}