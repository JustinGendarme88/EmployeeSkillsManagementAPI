using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;

namespace EmployeeSkillsManagement.Api.Services;

public interface IEmployeeService
{
    Task<PagedResult<Employee>> GetEmployeesAsync(
        string? name,
        string? email,
        int? departmentId,
        int page,
        int pageSize);
}