namespace EmployeeSkillsManagement.Api.DTOs
{
    public class UpdateSkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
