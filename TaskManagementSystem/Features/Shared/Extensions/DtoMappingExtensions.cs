using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Features.Categories.Shared;
using TaskManagementSystem.Features.Shared.DTOs;

namespace TaskManagementSystem.Features.Shared.Extensions;

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
            DueDate = taskEntity.DueDate,
            Status = taskEntity.Status,
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