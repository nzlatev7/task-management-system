using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.TaskDeleteStategy;

public sealed class TaskDeleteFactory : ITaskDeleteFactory
{
    private readonly Dictionary<Priority, ITaskDeleteStategy> _strategies;

    public TaskDeleteFactory()
    {
        _strategies = new Dictionary<Priority, ITaskDeleteStategy>
        {
            { Priority.Low, new TaskRemovingDeleteStategy() },
            { Priority.Medium, new TaskMovingDeleteStategy() },
            { Priority.High, new TaskLockingDeleteStategy() }
        };
    }

    public ITaskDeleteStategy GetDeleteStrategy(Priority taskPriority)
    {
        return _strategies[taskPriority];
    }
}