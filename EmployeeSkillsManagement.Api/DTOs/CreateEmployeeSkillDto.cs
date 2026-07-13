namespace EmployeeSkillsManagement.Api.DTOs;

public class CreateEmployeeSkillDto
{
    public int EmployeeId { get; set; }

    public int SkillId { get; set; }

    public int ProficiencyLevel { get; set; }
}