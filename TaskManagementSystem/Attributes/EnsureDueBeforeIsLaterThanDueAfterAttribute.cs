using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Constants;
using TaskManagementSystem.DTOs.Request;

namespace TaskManagementSystem.Attributes;

public class EnsureDueBeforeIsLaterThanDueAfterAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var dueAfterProperty = validationContext.ObjectType.GetProperty(nameof(ReportTasksRequestDto.DueAfter));
        if (dueAfterProperty != null)
        {
            var dueAfter = (DateTime?)dueAfterProperty.GetValue(validationContext.ObjectInstance);
            var dueBefore = (DateTime?)value;

            if (dueAfter >= dueBefore)
                return new ValidationResult(ErrorMessageConstants.DueBeforeEarlierThanDueAfter);
        }

        return ValidationResult.Success;
    }
}