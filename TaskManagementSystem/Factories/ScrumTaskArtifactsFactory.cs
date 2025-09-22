using TaskManagementSystem.BacklogOrderings;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.Workflows;

namespace TaskManagementSystem.Factories;

public sealed class ScrumTaskArtifactsFactory : ITaskArtifactsFactory
{
    public ITaskWorkflow CreateWorkflow(TaskKind kind) => kind switch
    {
        TaskKind.Bug => new BugWorkflow(),
        TaskKind.Feature => new FeatureWorkflow(),
        TaskKind.Incident => new IncidentWorkflow(),
        TaskKind.Research => new FeatureWorkflow(),
        _ => new FeatureWorkflow()
    };

    public IBacklogOrdering CreateBacklogOrdering(TaskKind kind) => kind switch
    {
        TaskKind.Bug => new BugBacklogOrdering(),
        TaskKind.Feature => new FeatureBacklogOrdering(),
        TaskKind.Incident => new IncidentBacklogOrdering(),
        TaskKind.Research => new FeatureBacklogOrdering(),
        _ => new DefaultBacklogOrdering()
    };
}
