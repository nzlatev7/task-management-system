namespace TaskManagementSystem.Interfaces;

public interface ICategoryRepository
{
    Task<bool> CategoryExistsAsync(int categoryId);
}