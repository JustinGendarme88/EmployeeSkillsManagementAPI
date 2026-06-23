using EmployeeSkillsManagement.Api.Data;
using EmployeeSkillsManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeSkillsManagement.Api.DTOs;

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

    [HttpGet]
    public async Task<ActionResult<List<Department>>> GetDepartments()
    {
        return await _context.Departments.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Department>> CreateDepartment(CreateDepartmentDto dto)
    {
        var department = new Department
        {
            Name = dto.Name
        };

        _context.Departments.Add(department);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetDepartments),
            new { id = department.Id },
            department);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Department>> GetDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
        {
            return NotFound();
        }

        return department;
    }
}