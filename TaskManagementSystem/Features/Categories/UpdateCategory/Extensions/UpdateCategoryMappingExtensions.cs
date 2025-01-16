using TaskManagementSystem.Database.Models;

namespace TaskManagementSystem.Features.Categories.UpdateCategory.Extensions;

public static class UpdateCategoryMappingExtensions
{
    public static void UpdateCategoryEntity(this UpdateCategoryCommand request, CategoryEntity category)
    {
        category.Name = request.Name;
        category.Description = request.Description;
    }
}
