using System.Collections.Generic;
using System.Linq;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.BacklogOrderings;

public sealed class FeatureBacklogOrdering : IBacklogOrdering
{
    public IEnumerable<TaskResponseDto> Order(IEnumerable<TaskResponseDto> backlog)
    {
        return backlog
            .OrderBy(task => task.StoryPoints ?? int.MaxValue)
            .ThenBy(task => task.DueDate)
            .ThenByDescending(task => task.Priority)
            .ThenBy(task => task.Id)
            .ToList();
    }
}
