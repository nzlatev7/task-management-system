using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.Attributes;

public sealed class EnsureUtcAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                return new ValidationResult(ErrorMessageConstants.DateInUtc);
            }
        }

        return ValidationResult.Success;
    }
}
