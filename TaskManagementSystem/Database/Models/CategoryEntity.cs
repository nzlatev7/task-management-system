using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.Database.Models;

[Table("category")]
public class CategoryEntity
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [MaxLength(EntityFiledConstants.NameMaxLength)]
    public required string Name { get; set; }

    [Column("description")]
    [MaxLength(EntityFiledConstants.DescriptionMaxLength)]
    public string? Description { get; set; }

    [Column("completion_percentage")]
    public short CompletionPercentage { get; set; }

    public virtual ICollection<TaskEntity> Tasks { get; } = new List<TaskEntity>();
}
