namespace TaskManagementSystem.Checkers;

public interface ICategoryChecker
{
    Task<bool> CategoryExistsAsync(int categoryId);
}