using TaskManagementSystem.Database;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Services;

public sealed class ScrumTasksService : TasksServiceBase
{
    public ScrumTasksService(
        TaskManagementSystemDbContext dbContext,
        ICategoryChecker categoryChecker,
        ITaskDeleteOrchestrator taskDeleteOrchestrator,
        ITaskArtifactsFactory taskArtifactsFactory)
        : base(dbContext, categoryChecker, taskDeleteOrchestrator, taskArtifactsFactory)
    {
    }
}
