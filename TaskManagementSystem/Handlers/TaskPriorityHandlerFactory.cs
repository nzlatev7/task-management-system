using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces.Handlers;

namespace TaskManagementSystem.Handlers;

public sealed class TaskPriorityHandlerFactory
{
    private readonly Dictionary<Priority, ITaskPriorityHandler> _handlers;

    public TaskPriorityHandlerFactory()
    {
        _handlers = new Dictionary<Priority, ITaskPriorityHandler>
        {
            { Priority.Low, new LowPriorityHandler() },
            { Priority.Medium, new MediumPriorityHandler() },
            { Priority.High, new HighPriorityHandler() }
        };
    }

    public ITaskPriorityHandler GetHandler(Priority priority)
    {
        return _handlers[priority];
    }
}