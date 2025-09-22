using TaskManagementSystem.Enums;
using TaskManagementSystem.Workflows;

namespace TaskManagementSystem.Interfaces;

public interface ITaskArtifactsFactory
{
    ITaskWorkflow CreateWorkflow(TaskKind kind);

    IBacklogOrdering CreateBacklogOrdering(TaskKind kind);
}
