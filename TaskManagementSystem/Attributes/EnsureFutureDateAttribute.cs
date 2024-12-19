using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.Attributes;

public sealed class EnsureFutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime <= DateTime.UtcNow)
            {
                return new ValidationResult(ErrorMessageConstants.DateInFuture);
            }
        }

        return ValidationResult.Success;
    }
}