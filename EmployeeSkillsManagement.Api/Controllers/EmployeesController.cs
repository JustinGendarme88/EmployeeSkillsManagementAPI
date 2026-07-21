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
    // GET: api/employees?name=john
    // GET: api/employees?email=gmail
    // GET: api/employees?departmentId=1
    // GET: api/employees?page=1&pageSize=10
    // GET: api/employees?departmentId=1&page=1&pageSize=5
    [HttpGet]
    public async Task<ActionResult> GetEmployees(
        [FromQuery] string? name,
        [FromQuery] string? email,
        [FromQuery] int? departmentId,
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

        var query = _context.Employees
            .AsNoTracking()
            .Include(employee => employee.Department)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalizedName = name.Trim().ToLower();

            query = query.Where(employee =>
                employee.FirstName.ToLower().Contains(normalizedName) ||
                employee.LastName.ToLower().Contains(normalizedName));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            var normalizedEmail = email.Trim().ToLower();

            query = query.Where(employee =>
                employee.Email.ToLower().Contains(normalizedEmail));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(employee =>
                employee.DepartmentId == departmentId.Value);
        }

        var totalItems = await query.CountAsync();

        var employees = await query
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(
            totalItems / (double)pageSize);

        var result = new PagedResult<Employee>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Items = employees
        };

        return Ok(result);
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
            .AnyAsync(department =>
                department.Id == dto.DepartmentId);

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
            .AnyAsync(department =>
                department.Id == dto.DepartmentId);

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