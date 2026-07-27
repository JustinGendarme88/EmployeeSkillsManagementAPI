using EmployeeSkillsManagement.Api.Data;
using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSkillsManagement.Api.Services;

public class SkillService : ISkillService
{
    private readonly ApplicationDbContext _context;

    public SkillService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Skill>> GetSkillsAsync(
        string? name,
        string? category,
        int page,
        int pageSize)
    {
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

        return new PagedResult<Skill>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Items = skills
        };
    }

    public async Task<Skill?> GetSkillByIdAsync(int id)
    {
        return await _context.Skills
            .AsNoTracking()
            .FirstOrDefaultAsync(skill => skill.Id == id);
    }

    public async Task<SkillServiceResult<Skill>> CreateSkillAsync(
        CreateSkillDto dto)
    {
        var normalizedName = dto.Name.Trim();
        var normalizedCategory = dto.Category.Trim();

        var skillExists = await _context.Skills
            .AnyAsync(skill =>
                skill.Name.ToLower() ==
                normalizedName.ToLower());

        if (skillExists)
        {
            return new SkillServiceResult<Skill>
            {
                Status = SkillServiceStatus.NameAlreadyExists
            };
        }

        var skill = new Skill
        {
            Name = normalizedName,
            Category = normalizedCategory
        };

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        return new SkillServiceResult<Skill>
        {
            Status = SkillServiceStatus.Success,
            Data = skill
        };
    }

    public async Task<SkillServiceResult<bool>> UpdateSkillAsync(
        int id,
        UpdateSkillDto dto)
    {
        var skill = await _context.Skills.FindAsync(id);

        if (skill is null)
        {
            return new SkillServiceResult<bool>
            {
                Status = SkillServiceStatus.NotFound
            };
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
            return new SkillServiceResult<bool>
            {
                Status = SkillServiceStatus.NameAlreadyExists
            };
        }

        skill.Name = normalizedName;
        skill.Category = normalizedCategory;

        await _context.SaveChangesAsync();

        return new SkillServiceResult<bool>
        {
            Status = SkillServiceStatus.Success,
            Data = true
        };
    }

    public async Task<SkillServiceResult<bool>> DeleteSkillAsync(
        int id)
    {
        var skill = await _context.Skills.FindAsync(id);

        if (skill is null)
        {
            return new SkillServiceResult<bool>
            {
                Status = SkillServiceStatus.NotFound
            };
        }

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();

        return new SkillServiceResult<bool>
        {
            Status = SkillServiceStatus.Success,
            Data = true
        };
    }
}