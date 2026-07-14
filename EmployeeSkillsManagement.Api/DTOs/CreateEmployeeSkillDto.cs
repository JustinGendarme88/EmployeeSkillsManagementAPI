using System.ComponentModel.DataAnnotations;

namespace EmployeeSkillsManagement.Api.DTOs;

public class CreateEmployeeSkillDto
{
    [Range(1, int.MaxValue, ErrorMessage = "EmployeeId must be greater than 0.")]
    public int EmployeeId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "SkillId must be greater than 0.")]
    public int SkillId { get; set; }

    [Range(1, 5, ErrorMessage = "Proficiency level must be between 1 and 5.")]
    public int ProficiencyLevel { get; set; }
}