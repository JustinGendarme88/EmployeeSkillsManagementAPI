using EmployeeSkillsManagement.Api.Data;
using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSkillsManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EmployeesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/employees
    [HttpGet]
    public async Task<ActionResult<List<Employee>>> GetEmployees()
    {
        var employees = await _context.Employees
            .AsNoTracking()
            .Include(employee => employee.Department)
            .ToListAsync();

        return Ok(employees);
    }

    // GET: api/employees/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Employee>> GetEmployee(int id)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .Include(employee => employee.Department)
            .FirstOrDefaultAsync(employee => employee.Id == id);

        if (employee is null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee(
        CreateEmployeeDto dto)
    {
        var departmentExists = await _context.Departments
            .AnyAsync(department => department.Id == dto.DepartmentId);

        if (!departmentExists)
        {
            return BadRequest("Department does not exist.");
        }

        var normalizedFirstName = dto.FirstName.Trim();
        var normalizedLastName = dto.LastName.Trim();
        var normalizedEmail = dto.Email.Trim().ToLower();

        var emailExists = await _context.Employees
            .AnyAsync(employee =>
                employee.Email.ToLower() == normalizedEmail);

        if (emailExists)
        {
            return Conflict(
                "An employee with this email already exists.");
        }

        var employee = new Employee
        {
            FirstName = normalizedFirstName,
            LastName = normalizedLastName,
            Email = normalizedEmail,
            DepartmentId = dto.DepartmentId
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEmployee),
            new { id = employee.Id },
            employee);
    }

    // PUT: api/employees/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEmployee(
        int id,
        UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return NotFound();
        }

        var departmentExists = await _context.Departments
            .AnyAsync(department => department.Id == dto.DepartmentId);

        if (!departmentExists)
        {
            return BadRequest("Department does not exist.");
        }

        var normalizedFirstName = dto.FirstName.Trim();
        var normalizedLastName = dto.LastName.Trim();
        var normalizedEmail = dto.Email.Trim().ToLower();

        var emailExists = await _context.Employees
            .AnyAsync(existingEmployee =>
                existingEmployee.Id != id &&
                existingEmployee.Email.ToLower() == normalizedEmail);

        if (emailExists)
        {
            return Conflict(
                "An employee with this email already exists.");
        }

        employee.FirstName = normalizedFirstName;
        employee.LastName = normalizedLastName;
        employee.Email = normalizedEmail;
        employee.DepartmentId = dto.DepartmentId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/employees/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}