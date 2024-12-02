using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.DTOs.Request;

public sealed class CategoryRequestDto
{
    [MaxLength(EntityFiledValidation.NameMaxLength)]
    public required string Name { get; set; }

    [MaxLength(EntityFiledValidation.DescriptionMaxLength)]
    public string? Description { get; set; }
}
