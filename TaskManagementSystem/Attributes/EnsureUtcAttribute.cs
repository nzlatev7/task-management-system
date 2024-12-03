using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Attributes;

public sealed class EnsureUtcAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                return new ValidationResult("The date must be in UTC.");
            }

            if (dateTime <= DateTime.UtcNow)
            {
                return new ValidationResult("The date must be in the future");
            }
        }

        return ValidationResult.Success;
    }
}
