using TaskManagementSystem.Database.Models;

namespace TaskManagementSystem.Features.Categories.CreateCategory.Extensions;

public static class CreateCategoryMappingExtensions
{
    public static CategoryEntity ToCategoryEntityForCreate(this CreateCategoryCommand request)
    {
        return new CategoryEntity()
        {
            Name = request.Name,
            Description = request.Description
        };
    }
}
