using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagementSystem.Attributes;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.DTOs.Request;

public sealed class TaskRequestDto
{
    [Required]
    [MaxLength(EntityFiledConstants.TitleMaxLength)]
    public required string Title { get; set; }

    [MaxLength(EntityFiledConstants.DescriptionMaxLength)]
    public string? Description { get; set; }    

    [EnsureUtc]
    public DateTime DueDate { get; set; }

    [Range(0, 2, ErrorMessage = ErrorMessageConstants.TaskPriority)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Priority? Priority { get; set; }

    public bool IsCompleted { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}
