using EmployeeSkillsManagement.Api.DTOs;

namespace EmployeeSkillsManagement.Api.Services;

public interface IEmployeeSkillService
{
    Task<List<EmployeeSkillDto>> GetEmployeeSkillsAsync();

    Task<EmployeeSkillDto?> GetEmployeeSkillAsync(
        int employeeId,
        int skillId);

    Task<EmployeeSkillServiceResult<EmployeeSkillDto>>
        CreateEmployeeSkillAsync(
            CreateEmployeeSkillDto dto);

    Task<EmployeeSkillServiceResult<bool>>
        UpdateEmployeeSkillAsync(
            int employeeId,
            int skillId,
            UpdateEmployeeSkillDto dto);

    Task<EmployeeSkillServiceResult<bool>>
        DeleteEmployeeSkillAsync(
            int employeeId,
            int skillId);
}