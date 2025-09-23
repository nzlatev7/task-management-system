using TaskManagementSystem.BacklogOrderings;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.Workflows;
using TaskManagementSystem.Workflows.Prototypes;

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

    public ITaskPrototype CreatePrototype(TaskKind kind) => kind switch
    {
        TaskKind.Bug => new BugTaskPrototype(),
        TaskKind.Feature => new FeatureTaskPrototype(),
        TaskKind.Incident => new IncidentTaskPrototype(),
        TaskKind.Research => new FeatureTaskPrototype(),
        _ => new FeatureTaskPrototype()
    };
}
