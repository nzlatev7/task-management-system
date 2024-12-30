using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Interfaces;

public interface ITaskDeleteFactory
{
    public ITaskDeleteStategy GetDeleteStrategy(Priority taskPriority);
}