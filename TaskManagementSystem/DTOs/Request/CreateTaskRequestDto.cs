using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagementSystem.Attributes;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.DTOs.Request;

public class CreateTaskRequestDto
{
    [Required]
    [MaxLength(EntityFiledConstants.TitleMaxLength)]
    public required string Title { get; set; }

    [MaxLength(EntityFiledConstants.DescriptionMaxLength)]
    public string? Description { get; set; }

    [EnsureUtc]
    [EnsureFutureDate]
    public DateTime DueDate { get; set; }

    [Range(0, 2, ErrorMessage = ErrorMessageConstants.TaskPriorityMustRepresentValidValues)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Priority? Priority { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskKind Kind { get; set; }

    [Range(1, 5, ErrorMessage = ErrorMessageConstants.SeverityMustRepresentValidValues)]
    public int? Severity { get; set; }

    [Range(1, 100, ErrorMessage = ErrorMessageConstants.StoryPointsMustRepresentValidValues)]
    public int? StoryPoints { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}
