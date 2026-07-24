using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;

namespace EmployeeSkillsManagement.Api.Services;

public interface IDepartmentService
{
    Task<PagedResult<Department>> GetDepartmentsAsync(
        string? name,
        int page,
        int pageSize);

    Task<Department?> GetDepartmentByIdAsync(int id);

    Task<DepartmentServiceResult<Department>> CreateDepartmentAsync(
        CreateDepartmentDto dto);

    Task<DepartmentServiceResult<bool>> UpdateDepartmentAsync(
        int id,
        UpdateDepartmentDto dto);

    Task<DepartmentServiceResult<bool>> DeleteDepartmentAsync(int id);
}