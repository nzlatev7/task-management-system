using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.Models;

public sealed class TaskEntity
{
    public int Id { get; set; }

    [MaxLength(EntityConstants.TitleMaxLength)]
    public required string Title { get; set; }

    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public int CategoryId { get; set; }

    public required Category Category { get; set; }
}
