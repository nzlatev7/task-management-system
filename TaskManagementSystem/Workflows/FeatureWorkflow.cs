using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Workflows;

public class FeatureWorkflow : ITaskWorkflow
{
    public void Validate(CreateTaskRequestDto dto)
    {
        if (!dto.StoryPoints.HasValue)
            throw new ArgumentException("Feature requires StoryPoints.");

        if (dto.StoryPoints.GetValueOrDefault() <= 0)
            throw new ArgumentException("StoryPoints must be a positive integer.");
    }
    public TaskEntity Build(CreateTaskRequestDto dto)
    {
        var dueDate = dto.DueDate != default ? dto.DueDate : DateTime.UtcNow.AddDays(14);
        var priority = dto.Priority ?? Priority.Medium;

        return new TaskEntity
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dueDate,
            Priority = priority,
            StoryPoints = dto.StoryPoints,
            IsCompleted = false,
            Status = Status.Pending,
            CategoryId = dto.CategoryId,
            Kind = TaskKind.Feature
        };
    }
}