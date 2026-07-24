namespace EmployeeSkillsManagement.Api.Services;

public class DepartmentServiceResult<T>
{
    public DepartmentServiceStatus Status { get; set; }

    public T? Data { get; set; }
}