﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.Database.Models;

[Table("category")]
public class CategoryEntity
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [MaxLength(EntityFiledValidation.NameMaxLength)]
    public required string Name { get; set; }

    [Column("description")]
    [MaxLength(EntityFiledValidation.DescriptionMaxLength)]
    public string? Description { get; set; }

    public virtual ICollection<TaskEntity> Tasks { get; } = new List<TaskEntity>();
}
