using EmployeeSkillsManagement.Api.Data;
using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSkillsManagement.Api.Services;

public class EmployeeSkillService : IEmployeeSkillService
{
    private readonly ApplicationDbContext _context;

    public EmployeeSkillService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeSkillDto>>
        GetEmployeeSkillsAsync()
    {
        return await _context.EmployeeSkills
            .AsNoTracking()
            .Include(employeeSkill =>
                employeeSkill.Employee)
            .Include(employeeSkill =>
                employeeSkill.Skill)
            .OrderBy(employeeSkill =>
                employeeSkill.Employee!.LastName)
            .ThenBy(employeeSkill =>
                employeeSkill.Employee!.FirstName)
            .ThenBy(employeeSkill =>
                employeeSkill.Skill!.Name)
            .Select(employeeSkill =>
                new EmployeeSkillDto
                {
                    EmployeeId =
                        employeeSkill.EmployeeId,

                    EmployeeName =
                        employeeSkill.Employee!.FirstName +
                        " " +
                        employeeSkill.Employee.LastName,

                    SkillId =
                        employeeSkill.SkillId,

                    SkillName =
                        employeeSkill.Skill!.Name,

                    ProficiencyLevel =
                        employeeSkill.ProficiencyLevel
                })
            .ToListAsync();
    }

    public async Task<EmployeeSkillDto?>
        GetEmployeeSkillAsync(
            int employeeId,
            int skillId)
    {
        return await _context.EmployeeSkills
            .AsNoTracking()
            .Where(employeeSkill =>
                employeeSkill.EmployeeId == employeeId &&
                employeeSkill.SkillId == skillId)
            .Select(employeeSkill =>
                new EmployeeSkillDto
                {
                    EmployeeId =
                        employeeSkill.EmployeeId,

                    EmployeeName =
                        employeeSkill.Employee!.FirstName +
                        " " +
                        employeeSkill.Employee.LastName,

                    SkillId =
                        employeeSkill.SkillId,

                    SkillName =
                        employeeSkill.Skill!.Name,

                    ProficiencyLevel =
                        employeeSkill.ProficiencyLevel
                })
            .FirstOrDefaultAsync();
    }

    public async Task<EmployeeSkillServiceResult<EmployeeSkillDto>>
        CreateEmployeeSkillAsync(
            CreateEmployeeSkillDto dto)
    {
        var employeeExists = await _context.Employees
            .AnyAsync(employee =>
                employee.Id == dto.EmployeeId);

        if (!employeeExists)
        {
            return new EmployeeSkillServiceResult<EmployeeSkillDto>
            {
                Status =
                    EmployeeSkillServiceStatus.EmployeeNotFound
            };
        }

        var skillExists = await _context.Skills
            .AnyAsync(skill =>
                skill.Id == dto.SkillId);

        if (!skillExists)
        {
            return new EmployeeSkillServiceResult<EmployeeSkillDto>
            {
                Status =
                    EmployeeSkillServiceStatus.SkillNotFound
            };
        }

        var employeeSkillExists =
            await _context.EmployeeSkills
                .AnyAsync(employeeSkill =>
                    employeeSkill.EmployeeId ==
                    dto.EmployeeId &&
                    employeeSkill.SkillId ==
                    dto.SkillId);

        if (employeeSkillExists)
        {
            return new EmployeeSkillServiceResult<EmployeeSkillDto>
            {
                Status =
                    EmployeeSkillServiceStatus.AlreadyExists
            };
        }

        var employeeSkill = new EmployeeSkill
        {
            EmployeeId = dto.EmployeeId,
            SkillId = dto.SkillId,
            ProficiencyLevel = dto.ProficiencyLevel
        };

        _context.EmployeeSkills.Add(employeeSkill);
        await _context.SaveChangesAsync();

        var createdEmployeeSkill =
            await GetEmployeeSkillAsync(
                employeeSkill.EmployeeId,
                employeeSkill.SkillId);

        return new EmployeeSkillServiceResult<EmployeeSkillDto>
        {
            Status = EmployeeSkillServiceStatus.Success,
            Data = createdEmployeeSkill
        };
    }

    public async Task<EmployeeSkillServiceResult<bool>>
        UpdateEmployeeSkillAsync(
            int employeeId,
            int skillId,
            UpdateEmployeeSkillDto dto)
    {
        var employeeSkill =
            await _context.EmployeeSkills
                .FirstOrDefaultAsync(employeeSkill =>
                    employeeSkill.EmployeeId == employeeId &&
                    employeeSkill.SkillId == skillId);

        if (employeeSkill is null)
        {
            return new EmployeeSkillServiceResult<bool>
            {
                Status =
                    EmployeeSkillServiceStatus.NotFound
            };
        }

        employeeSkill.ProficiencyLevel =
            dto.ProficiencyLevel;

        await _context.SaveChangesAsync();

        return new EmployeeSkillServiceResult<bool>
        {
            Status = EmployeeSkillServiceStatus.Success,
            Data = true
        };
    }

    public async Task<EmployeeSkillServiceResult<bool>>
        DeleteEmployeeSkillAsync(
            int employeeId,
            int skillId)
    {
        var employeeSkill =
            await _context.EmployeeSkills
                .FirstOrDefaultAsync(employeeSkill =>
                    employeeSkill.EmployeeId == employeeId &&
                    employeeSkill.SkillId == skillId);

        if (employeeSkill is null)
        {
            return new EmployeeSkillServiceResult<bool>
            {
                Status =
                    EmployeeSkillServiceStatus.NotFound
            };
        }

        _context.EmployeeSkills.Remove(employeeSkill);
        await _context.SaveChangesAsync();

        return new EmployeeSkillServiceResult<bool>
        {
            Status = EmployeeSkillServiceStatus.Success,
            Data = true
        };
    }
}