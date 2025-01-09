using System.Text.Json.Serialization;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Features.Shared.DTOs;

public sealed class TaskResponseDto
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Priority Priority { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Status Status { get; set; }

    public int CategoryId { get; set; }
}