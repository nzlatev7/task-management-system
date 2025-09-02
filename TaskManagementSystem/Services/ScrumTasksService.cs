using TaskManagementSystem.Database;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.Workflows;

namespace TaskManagementSystem.Services;

public sealed class ScrumTasksService : TasksServiceBase
{
    public ScrumTasksService(
        TaskManagementSystemDbContext dbContext,
        ICategoryChecker categoryChecker,
        ITaskDeleteOrchestrator taskDeleteOrchestrator)
        : base(dbContext, categoryChecker, taskDeleteOrchestrator)
    {
    }

    protected override ITaskWorkflow CreateWorkflow(TaskKind kind) => kind switch
    {
        TaskKind.Bug => new BugWorkflow(),
        TaskKind.Feature => new FeatureWorkflow(),
        TaskKind.Incident => new IncidentWorkflow(),
        TaskKind.Research => new FeatureWorkflow(),
        _ => new FeatureWorkflow()
    };
}
