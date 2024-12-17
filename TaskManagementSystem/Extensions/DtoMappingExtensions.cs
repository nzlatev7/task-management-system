using Microsoft.EntityFrameworkCore;
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
            DueDate = taskEntity.DueDate,
            IsCompleted = taskEntity.IsCompleted,
            Status = taskEntity.Status,
            CategoryId = taskEntity.CategoryId
        };

        return result;
    }

    public static async Task<IEnumerable<TaskResponseDto>> ToOutDtos(this IQueryable<TaskEntity> tasks)
    {
        return await tasks.Select(x => new TaskResponseDto
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            DueDate = x.DueDate,
            Priority = x.Priority,
            IsCompleted = x.IsCompleted,
            Status= x.Status,
            CategoryId = x.CategoryId
        }).ToListAsync();
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

    public static async Task<IEnumerable<CategoryResponseDto>> ToOutDtos(this IQueryable<CategoryEntity> categories)
    {
        return await categories.Select(x => new CategoryResponseDto
        {
            Id = x.Id,
            Name = x.Name,
            Description= x.Description
        }).ToListAsync();
    }
}
