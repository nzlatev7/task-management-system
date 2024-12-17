using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.DTOs.Request;

public sealed class CategoryRequestDto
{
    [MaxLength(EntityFiledConstants.NameMaxLength)]
    public required string Name { get; set; }

    [MaxLength(EntityFiledConstants.DescriptionMaxLength)]
    public string? Description { get; set; }
}
