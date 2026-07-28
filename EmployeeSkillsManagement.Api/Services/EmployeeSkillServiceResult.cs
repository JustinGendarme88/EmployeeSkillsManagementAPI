namespace EmployeeSkillsManagement.Api.Services;

public class EmployeeSkillServiceResult<T>
{
    public EmployeeSkillServiceStatus Status { get; set; }

    public T? Data { get; set; }
}