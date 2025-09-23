using TaskManagementSystem.Builders;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Workflows;

public sealed class FeatureWorkflow : ITaskWorkflow
{
    public void Validate(CreateTaskRequestDto dto)
    {
        if (!dto.StoryPoints.HasValue)
            throw new ArgumentException("Feature requires StoryPoints.");
    }

    public TaskEntity Build(CreateTaskRequestDto dto)
    {
        var dueDate = dto.DueDate != default ? dto.DueDate : DateTime.UtcNow.AddDays(14);
        var priority = dto.Priority ?? Priority.Medium;

        return TaskEntityBuilder.Create(
                dto.Title,
                dto.Description,
                dueDate,
                dto.CategoryId,
                TaskKind.Feature)
            .WithPriority(priority)
            .WithStoryPoints(dto.StoryPoints!.Value)
            .Build();
    }
}
