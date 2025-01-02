using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Database.Models;

[Table("deleted_task")]
public class DeletedTaskEntity
{
    [Key]
    [Column("task_id")]
    public int TaskId { get; set; }

    [Column("title")]
    [MaxLength(EntityFiledConstants.TitleMaxLength)]
    public required string Title { get; set; }

    [Column("description")]
    [MaxLength(EntityFiledConstants.DescriptionMaxLength)]
    public string? Description { get; set; }

    [Column("due_date")]
    public DateTime DueDate { get; set; }

    [Column("priority")]
    public Priority Priority { get; set; } = Priority.Medium;

    [Column("status")]
    public Status Status { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    public virtual CategoryEntity? Category { get; set; }
}