using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;

namespace EmployeeSkillsManagement.Api.Services;

public interface ISkillService
{
    Task<PagedResult<Skill>> GetSkillsAsync(
        string? name,
        string? category,
        int page,
        int pageSize);

    Task<Skill?> GetSkillByIdAsync(int id);

    Task<SkillServiceResult<Skill>> CreateSkillAsync(
        CreateSkillDto dto);

    Task<SkillServiceResult<bool>> UpdateSkillAsync(
        int id,
        UpdateSkillDto dto);

    Task<SkillServiceResult<bool>> DeleteSkillAsync(int id);
}