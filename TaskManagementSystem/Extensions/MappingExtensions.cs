using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Helpers;

public static class MappingExtensions
{
    public static TaskResponseDto ToOutDto(this TaskEntity taskEntity)
    {
        var result = new TaskResponseDto()
        {
            Id = taskEntity.Id,
            Title = taskEntity.Title,
            Description = taskEntity.Description,
            DueDate = taskEntity.DueDate,
            IsCompleted = taskEntity.IsCompleted,
            CategoryId = taskEntity.CategoryId
        };

        return result;
    }
}
