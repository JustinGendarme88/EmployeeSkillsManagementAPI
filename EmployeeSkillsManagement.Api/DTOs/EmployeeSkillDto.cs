namespace EmployeeSkillsManagement.Api.DTOs;

public class EmployeeSkillDto
{
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public int SkillId { get; set; }

    public string SkillName { get; set; } = string.Empty;

    public int ProficiencyLevel { get; set; }
}