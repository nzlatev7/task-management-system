using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.Features.Categories.Shared;

public sealed class CategoryRequestDto
{
    [MaxLength(EntityFiledConstants.NameMaxLength)]
    public required string Name { get; set; }

    [MaxLength(EntityFiledConstants.DescriptionMaxLength)]
    public string? Description { get; set; }
}
