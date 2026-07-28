using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeSkillsManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeSkillsController : ControllerBase
{
    private readonly IEmployeeSkillService
        _employeeSkillService;

    public EmployeeSkillsController(
        IEmployeeSkillService employeeSkillService)
    {
        _employeeSkillService = employeeSkillService;
    }

    // GET: api/employeeskills
    [HttpGet]
    public async Task<
        ActionResult<IEnumerable<EmployeeSkillDto>>>
        GetEmployeeSkills()
    {
        var employeeSkills =
            await _employeeSkillService
                .GetEmployeeSkillsAsync();

        return Ok(employeeSkills);
    }

    // GET: api/employeeskills/1/2
    [HttpGet("{employeeId:int}/{skillId:int}")]
    public async Task<ActionResult<EmployeeSkillDto>>
        GetEmployeeSkill(
            int employeeId,
            int skillId)
    {
        var employeeSkill =
            await _employeeSkillService
                .GetEmployeeSkillAsync(
                    employeeId,
                    skillId);

        if (employeeSkill is null)
        {
            return NotFound();
        }

        return Ok(employeeSkill);
    }

    // POST: api/employeeskills
    [HttpPost]
    public async Task<ActionResult<EmployeeSkillDto>>
        CreateEmployeeSkill(
            CreateEmployeeSkillDto dto)
    {
        var result =
            await _employeeSkillService
                .CreateEmployeeSkillAsync(dto);

        if (result.Status ==
            EmployeeSkillServiceStatus.EmployeeNotFound)
        {
            return BadRequest(
                "The specified employee does not exist.");
        }

        if (result.Status ==
            EmployeeSkillServiceStatus.SkillNotFound)
        {
            return BadRequest(
                "The specified skill does not exist.");
        }

        if (result.Status ==
            EmployeeSkillServiceStatus.AlreadyExists)
        {
            return Conflict(
                "This skill is already assigned to this employee.");
        }

        return CreatedAtAction(
            nameof(GetEmployeeSkill),
            new
            {
                employeeId = result.Data!.EmployeeId,
                skillId = result.Data.SkillId
            },
            result.Data);
    }

    // PUT: api/employeeskills/1/2
    [HttpPut("{employeeId:int}/{skillId:int}")]
    public async Task<IActionResult>
        UpdateEmployeeSkill(
            int employeeId,
            int skillId,
            UpdateEmployeeSkillDto dto)
    {
        var result =
            await _employeeSkillService
                .UpdateEmployeeSkillAsync(
                    employeeId,
                    skillId,
                    dto);

        if (result.Status ==
            EmployeeSkillServiceStatus.NotFound)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/employeeskills/1/2
    [HttpDelete("{employeeId:int}/{skillId:int}")]
    public async Task<IActionResult>
        DeleteEmployeeSkill(
            int employeeId,
            int skillId)
    {
        var result =
            await _employeeSkillService
                .DeleteEmployeeSkillAsync(
                    employeeId,
                    skillId);

        if (result.Status ==
            EmployeeSkillServiceStatus.NotFound)
        {
            return NotFound();
        }

        return NoContent();
    }
}