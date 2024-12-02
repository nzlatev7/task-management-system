using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Attributes;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.DTOs.Request;

public sealed class TaskRequestDto
{
    [Required]
    [MaxLength(EntityFiledValidation.TitleMaxLength)]
    public required string Title { get; set; }

    [MaxLength(EntityFiledValidation.DescriptionMaxLength)]
    public string? Description { get; set; }

    [EnsureUtc]
    public DateTime DueDate { get; set; }

    public bool IsCompleted { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}
