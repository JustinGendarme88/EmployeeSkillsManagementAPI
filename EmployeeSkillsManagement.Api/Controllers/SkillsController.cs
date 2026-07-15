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

    // GET: api/skills
    [HttpGet]
    public async Task<ActionResult<List<Skill>>> GetSkills()
    {
        var skills = await _context.Skills
            .AsNoTracking()
            .ToListAsync();

        return Ok(skills);
    }

    // GET: api/skills/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Skill>> GetSkill(int id)
    {
        var skill = await _context.Skills
            .AsNoTracking()
            .FirstOrDefaultAsync(skill => skill.Id == id);

        if (skill is null)
        {
            return NotFound();
        }

        return Ok(skill);
    }

    // POST: api/skills
    [HttpPost]
    public async Task<ActionResult<Skill>> CreateSkill(CreateSkillDto dto)
    {
        var normalizedName = dto.Name.Trim();
        var normalizedCategory = dto.Category.Trim();

        var skillExists = await _context.Skills
            .AnyAsync(skill =>
                skill.Name.ToLower() == normalizedName.ToLower());

        if (skillExists)
        {
            return Conflict(
                "A skill with this name already exists.");
        }

        var skill = new Skill
        {
            Name = normalizedName,
            Category = normalizedCategory
        };

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetSkill),
            new { id = skill.Id },
            skill);
    }

    // PUT: api/skills/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSkill(
        int id,
        UpdateSkillDto dto)
    {
        var skill = await _context.Skills.FindAsync(id);

        if (skill is null)
        {
            return NotFound();
        }

        var normalizedName = dto.Name.Trim();
        var normalizedCategory = dto.Category.Trim();

        var skillExists = await _context.Skills
            .AnyAsync(existingSkill =>
                existingSkill.Id != id &&
                existingSkill.Name.ToLower() ==
                normalizedName.ToLower());

        if (skillExists)
        {
            return Conflict(
                "A skill with this name already exists.");
        }

        skill.Name = normalizedName;
        skill.Category = normalizedCategory;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/skills/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var skill = await _context.Skills.FindAsync(id);

        if (skill is null)
        {
            return NotFound();
        }

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}