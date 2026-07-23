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

    Task<Employee?> GetEmployeeByIdAsync(int id);

    Task<EmployeeServiceResult<Employee>> CreateEmployeeAsync(
        CreateEmployeeDto dto);

    Task<EmployeeServiceResult<bool>> UpdateEmployeeAsync(
        int id,
        UpdateEmployeeDto dto);

    Task<EmployeeServiceResult<bool>> DeleteEmployeeAsync(int id);
}