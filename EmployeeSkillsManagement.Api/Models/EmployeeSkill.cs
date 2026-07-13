namespace EmployeeSkillsManagement.Api.Models;

public class EmployeeSkill
{
    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    public int SkillId { get; set; }

    public Skill? Skill { get; set; }

    public int ProficiencyLevel { get; set; }
}