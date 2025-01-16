namespace TaskManagementSystem.Interfaces;

public interface ICategoryChecker
{
    Task<bool> CategoryExistsAsync(int categoryId);
}