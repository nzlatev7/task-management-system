using System.Linq.Expressions;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Tasks;

namespace TaskManagementSystem.Extensions;

public static class EntityHelperExtensions
{
    public static IQueryable<TaskEntity> SortBy(this IQueryable<TaskEntity> tasks, GetAllTasksQuery sortByInstructions)
    {
        switch (sortByInstructions.Property)
        {
            case SortingTaskProperty.Id:
                tasks = tasks.OrderBy(x => x.Id, sortByInstructions.IsAscending);
                break;
            case SortingTaskProperty.Title:
                tasks = tasks.OrderBy(x => x.Title, sortByInstructions.IsAscending);
                break;
            case SortingTaskProperty.DueDate:
                tasks = tasks.OrderBy(x => x.DueDate, sortByInstructions.IsAscending);
                break;
            case SortingTaskProperty.Priority:
                tasks = tasks.OrderBy(x => x.Priority, sortByInstructions.IsAscending);
                break;
            case SortingTaskProperty.Status:
                tasks = tasks.OrderBy(x => x.Status, sortByInstructions.IsAscending);
                break;
            case SortingTaskProperty.CategoryId:
                tasks = tasks.OrderBy(x => x.CategoryId, sortByInstructions.IsAscending);
                break;
            default:
                break;
        }

        return tasks;
    }

    private static IQueryable<TSource> OrderBy<TSource, TKey>(
        this IQueryable<TSource> source,
        Expression<Func<TSource, TKey>> keySelector,
        bool isAscending)
    { 
        return isAscending 
            ? source.OrderBy(keySelector) 
            : source.OrderByDescending(keySelector);
    }
}