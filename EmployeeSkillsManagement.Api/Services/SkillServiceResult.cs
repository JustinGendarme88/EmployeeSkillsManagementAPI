namespace EmployeeSkillsManagement.Api.Services;

public class SkillServiceResult<T>
{
    public SkillServiceStatus Status { get; set; }

    public T? Data { get; set; }
}