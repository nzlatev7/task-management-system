using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.Database.Models;

[Table("task")]
public class TaskEntity
{
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    [MaxLength(EntityFiledValidation.TitleMaxLength)]
    public required string Title { get; set; }

    [Column("description")]
    [MaxLength(EntityFiledValidation.DescriptionMaxLength)]
    public string? Description { get; set; }

    [Column("due_date")]
    public DateTime DueDate { get; set; }

    [Column("is_completed")]
    public bool IsCompleted { get; set; }

    [Column("category_id")]
    public int CategoryId { get; set; }

    public virtual CategoryEntity Category { get; set; } = null!;
}
