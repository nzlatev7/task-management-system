using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.DTOs.Request;

public sealed class UnlockTaskRequestDto
{
    [Range(0, 2, ErrorMessage = ErrorMessageConstants.UnlockTaskStatusMustRepresentValidValues)]
    public Status Status { get; set; }
}
