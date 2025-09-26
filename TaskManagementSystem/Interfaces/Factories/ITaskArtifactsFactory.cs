using TaskManagementSystem.Enums;
using TaskManagementSystem.Workflows;
using TaskManagementSystem.Workflows.Prototypes;

namespace TaskManagementSystem.Interfaces;

public interface ITaskArtifactsFactory
{
    ITaskWorkflow CreateWorkflow(TaskKind kind);

    IBacklogOrdering CreateBacklogOrdering(TaskKind kind);

    ITaskPrototype CreatePrototype(TaskKind kind);
}
