﻿using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.Models;

public sealed class Category
{
    public int Id { get; set; }

    [MaxLength(EntityConstants.NameMaxLength)]
    public required string Name { get; set; }

    public string? Description { get; set; }

    public ICollection<TaskEntity> Tasks { get; } = new List<TaskEntity>();
}
