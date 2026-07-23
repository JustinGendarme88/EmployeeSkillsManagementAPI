using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;
using EmployeeSkillsManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeSkillsManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET: api/employees
    // GET: api/employees?name=john
    // GET: api/employees?email=gmail
    // GET: api/employees?departmentId=1
    // GET: api/employees?page=1&pageSize=10
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

        var result = await _employeeService.GetEmployeesAsync(
            name,
            email,
            departmentId,
            page,
            pageSize);

        return Ok(result);
    }

    // GET: api/employees/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Employee>> GetEmployee(int id)
    {
        var employee =
            await _employeeService.GetEmployeeByIdAsync(id);

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
        var result =
            await _employeeService.CreateEmployeeAsync(dto);

        if (result.Status ==
            EmployeeServiceStatus.DepartmentNotFound)
        {
            return BadRequest("Department does not exist.");
        }

        if (result.Status ==
            EmployeeServiceStatus.EmailAlreadyExists)
        {
            return Conflict(
                "An employee with this email already exists.");
        }

        return CreatedAtAction(
            nameof(GetEmployee),
            new { id = result.Data!.Id },
            result.Data);
    }

    // PUT: api/employees/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEmployee(
        int id,
        UpdateEmployeeDto dto)
    {
        var result =
            await _employeeService.UpdateEmployeeAsync(id, dto);

        if (result.Status == EmployeeServiceStatus.NotFound)
        {
            return NotFound();
        }

        if (result.Status ==
            EmployeeServiceStatus.DepartmentNotFound)
        {
            return BadRequest("Department does not exist.");
        }

        if (result.Status ==
            EmployeeServiceStatus.EmailAlreadyExists)
        {
            return Conflict(
                "An employee with this email already exists.");
        }

        return NoContent();
    }

    // DELETE: api/employees/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var result =
            await _employeeService.DeleteEmployeeAsync(id);

        if (result.Status == EmployeeServiceStatus.NotFound)
        {
            return NotFound();
        }

        return NoContent();
    }
}