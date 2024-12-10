using TaskManagementSystem.Database.Models;

namespace TaskManagementSystem.Extensions;

public static class EntityHelperExtensions
{
    public static IQueryable<TaskEntity> SortByPriority(this IQueryable<TaskEntity> tasks, bool ascending)
    {
        return ascending 
            ? tasks.OrderBy(x => x.Priority) 
            : tasks.OrderByDescending(x => x.Priority);
    }
}
