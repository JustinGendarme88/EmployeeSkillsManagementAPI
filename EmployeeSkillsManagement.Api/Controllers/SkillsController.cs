using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;
using EmployeeSkillsManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeSkillsManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillsController : ControllerBase
{
    private readonly ISkillService _skillService;

    public SkillsController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    // GET: api/skills
    // GET: api/skills?name=csharp
    // GET: api/skills?category=programming
    // GET: api/skills?page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult> GetSkills(
        [FromQuery] string? name,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1)
        {
            return BadRequest(
                "Page must be greater than or equal to 1.");
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(
                "Page size must be between 1 and 100.");
        }

        var result = await _skillService.GetSkillsAsync(
            name,
            category,
            page,
            pageSize);

        return Ok(result);
    }

    // GET: api/skills/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Skill>> GetSkill(int id)
    {
        var skill = await _skillService.GetSkillByIdAsync(id);

        if (skill is null)
        {
            return NotFound();
        }

        return Ok(skill);
    }

    // POST: api/skills
    [HttpPost]
    public async Task<ActionResult<Skill>> CreateSkill(
        CreateSkillDto dto)
    {
        var result = await _skillService.CreateSkillAsync(dto);

        if (result.Status ==
            SkillServiceStatus.NameAlreadyExists)
        {
            return Conflict(
                "A skill with this name already exists.");
        }

        return CreatedAtAction(
            nameof(GetSkill),
            new { id = result.Data!.Id },
            result.Data);
    }

    // PUT: api/skills/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSkill(
        int id,
        UpdateSkillDto dto)
    {
        var result = await _skillService.UpdateSkillAsync(
            id,
            dto);

        if (result.Status == SkillServiceStatus.NotFound)
        {
            return NotFound();
        }

        if (result.Status ==
            SkillServiceStatus.NameAlreadyExists)
        {
            return Conflict(
                "A skill with this name already exists.");
        }

        return NoContent();
    }

    // DELETE: api/skills/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var result = await _skillService.DeleteSkillAsync(id);

        if (result.Status == SkillServiceStatus.NotFound)
        {
            return NotFound();
        }

        return NoContent();
    }
}