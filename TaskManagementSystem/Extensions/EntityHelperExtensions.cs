using TaskManagementSystem.Database.Models;

namespace TaskManagementSystem.Extensions;

public static class EntityHelperExtensions
{
    public static IQueryable<TaskEntity> SortByPriority(this IQueryable<TaskEntity> tasks, bool ascending)
    {
        if (ascending)
            return tasks.OrderBy(x => x.Priority);
        else
            return tasks.OrderByDescending(x => x.Priority);
    }
}
