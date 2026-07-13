using EmployeeSkillsManagement.Api.Data;
using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSkillsManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeSkillsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EmployeeSkillsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/employeeskills
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeSkillDto>>> GetEmployeeSkills()
    {
        var employeeSkills = await _context.EmployeeSkills
            .Include(employeeSkill => employeeSkill.Employee)
            .Include(employeeSkill => employeeSkill.Skill)
            .Select(employeeSkill => new EmployeeSkillDto
            {
                EmployeeId = employeeSkill.EmployeeId,
                EmployeeName =
                    employeeSkill.Employee!.FirstName + " " +
                    employeeSkill.Employee.LastName,
                SkillId = employeeSkill.SkillId,
                SkillName = employeeSkill.Skill!.Name,
                ProficiencyLevel = employeeSkill.ProficiencyLevel
            })
            .ToListAsync();

        return Ok(employeeSkills);
    }

    // GET: api/employeeskills/1/2
    [HttpGet("{employeeId:int}/{skillId:int}")]
    public async Task<ActionResult<EmployeeSkillDto>> GetEmployeeSkill(
        int employeeId,
        int skillId)
    {
        var employeeSkill = await _context.EmployeeSkills
            .Include(es => es.Employee)
            .Include(es => es.Skill)
            .FirstOrDefaultAsync(es =>
                es.EmployeeId == employeeId &&
                es.SkillId == skillId);

        if (employeeSkill is null)
        {
            return NotFound();
        }

        var employeeSkillDto = new EmployeeSkillDto
        {
            EmployeeId = employeeSkill.EmployeeId,
            EmployeeName =
                employeeSkill.Employee!.FirstName + " " +
                employeeSkill.Employee.LastName,
            SkillId = employeeSkill.SkillId,
            SkillName = employeeSkill.Skill!.Name,
            ProficiencyLevel = employeeSkill.ProficiencyLevel
        };

        return Ok(employeeSkillDto);
    }

    // POST: api/employeeskills
    [HttpPost]
    public async Task<ActionResult<EmployeeSkillDto>> CreateEmployeeSkill(
        CreateEmployeeSkillDto createEmployeeSkillDto)
    {
        var employeeExists = await _context.Employees
            .AnyAsync(employee =>
                employee.Id == createEmployeeSkillDto.EmployeeId);

        if (!employeeExists)
        {
            return BadRequest("The specified employee does not exist.");
        }

        var skillExists = await _context.Skills
            .AnyAsync(skill =>
                skill.Id == createEmployeeSkillDto.SkillId);

        if (!skillExists)
        {
            return BadRequest("The specified skill does not exist.");
        }

        var employeeSkillExists = await _context.EmployeeSkills
            .AnyAsync(employeeSkill =>
                employeeSkill.EmployeeId ==
                createEmployeeSkillDto.EmployeeId &&
                employeeSkill.SkillId ==
                createEmployeeSkillDto.SkillId);

        if (employeeSkillExists)
        {
            return Conflict(
                "This skill is already assigned to this employee.");
        }

        var employeeSkill = new EmployeeSkill
        {
            EmployeeId = createEmployeeSkillDto.EmployeeId,
            SkillId = createEmployeeSkillDto.SkillId,
            ProficiencyLevel =
                createEmployeeSkillDto.ProficiencyLevel
        };

        _context.EmployeeSkills.Add(employeeSkill);
        await _context.SaveChangesAsync();

        var createdEmployeeSkill = await _context.EmployeeSkills
            .Include(es => es.Employee)
            .Include(es => es.Skill)
            .FirstAsync(es =>
                es.EmployeeId == employeeSkill.EmployeeId &&
                es.SkillId == employeeSkill.SkillId);

        var employeeSkillDto = new EmployeeSkillDto
        {
            EmployeeId = createdEmployeeSkill.EmployeeId,
            EmployeeName =
                createdEmployeeSkill.Employee!.FirstName + " " +
                createdEmployeeSkill.Employee.LastName,
            SkillId = createdEmployeeSkill.SkillId,
            SkillName = createdEmployeeSkill.Skill!.Name,
            ProficiencyLevel =
                createdEmployeeSkill.ProficiencyLevel
        };

        return CreatedAtAction(
            nameof(GetEmployeeSkill),
            new
            {
                employeeId = employeeSkill.EmployeeId,
                skillId = employeeSkill.SkillId
            },
            employeeSkillDto);
    }

    // PUT: api/employeeskills/1/2
    [HttpPut("{employeeId:int}/{skillId:int}")]
    public async Task<IActionResult> UpdateEmployeeSkill(
        int employeeId,
        int skillId,
        UpdateEmployeeSkillDto updateEmployeeSkillDto)
    {
        var employeeSkill = await _context.EmployeeSkills
            .FirstOrDefaultAsync(es =>
                es.EmployeeId == employeeId &&
                es.SkillId == skillId);

        if (employeeSkill is null)
        {
            return NotFound();
        }

        employeeSkill.ProficiencyLevel =
            updateEmployeeSkillDto.ProficiencyLevel;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/employeeskills/1/2
    [HttpDelete("{employeeId:int}/{skillId:int}")]
    public async Task<IActionResult> DeleteEmployeeSkill(
        int employeeId,
        int skillId)
    {
        var employeeSkill = await _context.EmployeeSkills
            .FirstOrDefaultAsync(es =>
                es.EmployeeId == employeeId &&
                es.SkillId == skillId);

        if (employeeSkill is null)
        {
            return NotFound();
        }

        _context.EmployeeSkills.Remove(employeeSkill);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}