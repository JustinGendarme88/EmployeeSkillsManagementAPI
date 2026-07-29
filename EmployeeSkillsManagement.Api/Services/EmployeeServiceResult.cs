namespace EmployeeSkillsManagement.Api.Services;

public class EmployeeServiceResult<T>
{
    public EmployeeServiceStatus Status { get; set; } 

    public T? Data { get; set; }
}