using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.DTOs.Request;

public sealed class GetAllTasksRequestDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Range(0, 5, ErrorMessage = ErrorMessageConstants.SortingTaskPropertyMustRepresentValidValues)]
    public SortingTaskProperty Property { get; set; }

    public bool IsAscending { get; set; }
}
