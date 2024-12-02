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

    public static CategoryResponseDto ToOutDto(this Category category)
    {
        var result = new CategoryResponseDto()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Tasks = category.Tasks.Select(x => new TaskResponseDto()
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                DueDate = x.DueDate,
                IsCompleted = x.IsCompleted,
                CategoryId = x.CategoryId
            }).ToList()
        };

        return result;
    }
}
