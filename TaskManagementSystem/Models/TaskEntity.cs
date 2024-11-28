using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.Models;

public sealed class TaskEntity
{
    public int Id { get; set; }

    [MaxLength(EntityFiledValidation.TitleMaxLength)]
    public required string Title { get; set; }

    [MaxLength(EntityFiledValidation.DescriptionMaxLength)]
    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;
}
