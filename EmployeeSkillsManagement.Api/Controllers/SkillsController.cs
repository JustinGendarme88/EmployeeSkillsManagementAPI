using EmployeeSkillsManagement.Api.Data;
using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSkillsManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SkillsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Skill>>> GetSkills()
    {
        return await _context.Skills.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Skill>> GetSkill(int id)
    {
        var skill = await _context.Skills.FindAsync(id);

        if (skill == null)
        {
            return NotFound();
        }

        return skill;
    }

    [HttpPost]
    public async Task<ActionResult<Skill>> CreateSkill(CreateSkillDto dto)
    {
        var skill = new Skill
        {
            Name = dto.Name,
            Category = dto.Category
        };

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetSkill),
            new { id = skill.Id },
            skill);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSkill(int id, UpdateSkillDto dto)
    {
        var skill = await _context.Skills.FindAsync(id);

        if (skill == null)
        {
            return NotFound();
        }

        skill.Name = dto.Name;
        skill.Category = dto.Category;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var skill = await _context.Skills.FindAsync(id);

        if (skill == null)
        {
            return NotFound();
        }

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}