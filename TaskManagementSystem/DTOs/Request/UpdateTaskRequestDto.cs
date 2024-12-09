using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.DTOs.Request;

public sealed class UpdateTaskRequestDto : CreateTaskRequestDto
{
    [Range(1, 3, ErrorMessage = ErrorMessageConstants.TaskStatusMustRepresentValidValues)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Status Status { get; set; }
}
