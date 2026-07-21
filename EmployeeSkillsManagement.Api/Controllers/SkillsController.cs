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

        var query = _context.Skills
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalizedName = name.Trim().ToLower();

            query = query.Where(skill =>
                skill.Name.ToLower().Contains(normalizedName));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalizedCategory = category.Trim().ToLower();

            query = query.Where(skill =>
                skill.Category.ToLower().Contains(normalizedCategory));
        }

        var totalItems = await query.CountAsync();

        var skills = await query
            .OrderBy(skill => skill.Category)
            .ThenBy(skill => skill.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(
            totalItems / (double)pageSize);

        var result = new PagedResult<Skill>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Items = skills
        };

        return Ok(result);
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