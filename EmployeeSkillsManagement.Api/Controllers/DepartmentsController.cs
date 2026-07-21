using EmployeeSkillsManagement.Api.Data;
using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSkillsManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DepartmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/departments
    // GET: api/departments?name=it
    // GET: api/departments?page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult> GetDepartments(
        [FromQuery] string? name,
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

        var query = _context.Departments
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalizedName = name.Trim().ToLower();

            query = query.Where(department =>
                department.Name.ToLower().Contains(normalizedName));
        }

        var totalItems = await query.CountAsync();

        var departments = await query
            .OrderBy(department => department.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(
            totalItems / (double)pageSize);

        var result = new PagedResult<Department>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Items = departments
        };

        return Ok(result);
    }

    // GET: api/departments/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Department>> GetDepartment(int id)
    {
        var department = await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(department => department.Id == id);

        if (department is null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    // POST: api/departments
    [HttpPost]
    public async Task<ActionResult<Department>> CreateDepartment(
        CreateDepartmentDto dto)
    {
        var normalizedName = dto.Name.Trim();

        var departmentExists = await _context.Departments
            .AnyAsync(department =>
                department.Name.ToLower() == normalizedName.ToLower());

        if (departmentExists)
        {
            return Conflict(
                "A department with this name already exists.");
        }

        var department = new Department
        {
            Name = normalizedName
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetDepartment),
            new { id = department.Id },
            department);
    }

    // PUT: api/departments/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDepartment(
        int id,
        UpdateDepartmentDto dto)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department is null)
        {
            return NotFound();
        }

        var normalizedName = dto.Name.Trim();

        var departmentExists = await _context.Departments
            .AnyAsync(existingDepartment =>
                existingDepartment.Id != id &&
                existingDepartment.Name.ToLower() ==
                normalizedName.ToLower());

        if (departmentExists)
        {
            return Conflict(
                "A department with this name already exists.");
        }

        department.Name = normalizedName;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/departments/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department is null)
        {
            return NotFound();
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}