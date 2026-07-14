using System.ComponentModel.DataAnnotations;

namespace EmployeeSkillsManagement.Api.DTOs;

public class CreateDepartmentDto
{
    [Required(ErrorMessage = "Department name is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Department name must contain between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;
}