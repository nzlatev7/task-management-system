using System;
using System.Collections.Generic;
using System.Threading;
using TaskManagementSystem.BacklogOrderings;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.Workflows;

namespace TaskManagementSystem.Factories;

public sealed class ScrumTaskArtifactsFactory : ITaskArtifactsFactory
{
    private static readonly Lazy<ITaskWorkflow> BugWorkflow = new(() => new BugWorkflow(), LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly Lazy<ITaskWorkflow> FeatureWorkflow = new(() => new FeatureWorkflow(), LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly Lazy<ITaskWorkflow> IncidentWorkflow = new(() => new IncidentWorkflow(), LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly IReadOnlyDictionary<TaskKind, Lazy<ITaskWorkflow>> Workflows = new Dictionary<TaskKind, Lazy<ITaskWorkflow>>
    {
        [TaskKind.Bug] = BugWorkflow,
        [TaskKind.Feature] = FeatureWorkflow,
        [TaskKind.Incident] = IncidentWorkflow,
        [TaskKind.Research] = FeatureWorkflow
    };

    private static readonly Lazy<IBacklogOrdering> BugBacklogOrdering = new(() => new BugBacklogOrdering(), LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly Lazy<IBacklogOrdering> FeatureBacklogOrdering = new(() => new FeatureBacklogOrdering(), LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly Lazy<IBacklogOrdering> IncidentBacklogOrdering = new(() => new IncidentBacklogOrdering(), LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly Lazy<IBacklogOrdering> DefaultBacklogOrdering = new(() => new DefaultBacklogOrdering(), LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly IReadOnlyDictionary<TaskKind, Lazy<IBacklogOrdering>> BacklogOrderings = new Dictionary<TaskKind, Lazy<IBacklogOrdering>>
    {
        [TaskKind.Bug] = BugBacklogOrdering,
        [TaskKind.Feature] = FeatureBacklogOrdering,
        [TaskKind.Incident] = IncidentBacklogOrdering,
        [TaskKind.Research] = FeatureBacklogOrdering
    };

    public ITaskWorkflow CreateWorkflow(TaskKind kind)
    {
        if (Workflows.TryGetValue(kind, out var workflow))
        {
            return workflow.Value;
        }

        return FeatureWorkflow.Value;
    }

    public IBacklogOrdering CreateBacklogOrdering(TaskKind kind)
    {
        if (BacklogOrderings.TryGetValue(kind, out var backlogOrdering))
        {
            return backlogOrdering.Value;
        }

        return DefaultBacklogOrdering.Value;
    }
}
