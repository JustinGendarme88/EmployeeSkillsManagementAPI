using System.ComponentModel.DataAnnotations;

namespace EmployeeSkillsManagement.Api.DTOs;

public class UpdateEmployeeDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2,
        ErrorMessage = "First name must contain between 2 and 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2,
        ErrorMessage = "Last name must contain between 2 and 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    public string Email { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be greater than 0.")]
    public int DepartmentId { get; set; }
}