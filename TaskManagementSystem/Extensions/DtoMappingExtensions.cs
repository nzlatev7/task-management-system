using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Response;

namespace TaskManagementSystem.Helpers;

public static class DtoMappingExtensions
{
    public static TaskResponseDto ToOutDto(this TaskEntity taskEntity)
    {
        var result = new TaskResponseDto()
        {
            Id = taskEntity.Id,
            Title = taskEntity.Title,
            Description = taskEntity.Description,
            Priority = taskEntity.Priority,
            StoryPoints = taskEntity.StoryPoints,
            DueDate = taskEntity.DueDate,
            IsCompleted = taskEntity.IsCompleted,
            Status = taskEntity.Status,
            Kind = taskEntity.Kind,
            CategoryId = taskEntity.CategoryId
        };

        return result;
    }

    public static CategoryResponseDto ToOutDto(this CategoryEntity category)
    {
        var result = new CategoryResponseDto()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        return result;
    }
}