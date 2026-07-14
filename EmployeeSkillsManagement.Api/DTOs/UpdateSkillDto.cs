using System.ComponentModel.DataAnnotations;

namespace EmployeeSkillsManagement.Api.DTOs;

public class UpdateSkillDto
{
    [Required(ErrorMessage = "Skill name is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Skill name must contain between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Skill category is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Skill category must contain between 2 and 100 characters.")]
    public string Category { get; set; } = string.Empty;
}