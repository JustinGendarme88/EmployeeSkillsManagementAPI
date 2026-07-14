using System.ComponentModel.DataAnnotations;

namespace EmployeeSkillsManagement.Api.DTOs;

public class UpdateEmployeeSkillDto
{
    [Range(1, 5, ErrorMessage = "Proficiency level must be between 1 and 5.")]
    public int ProficiencyLevel { get; set; }
}