using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;
using EmployeeSkillsManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeSkillsManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(
        IDepartmentService departmentService)
    {
        _departmentService = departmentService;
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

        var result =
            await _departmentService.GetDepartmentsAsync(
                name,
                page,
                pageSize);

        return Ok(result);
    }

    // GET: api/departments/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Department>> GetDepartment(
        int id)
    {
        var department =
            await _departmentService.GetDepartmentByIdAsync(id);

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
        var result =
            await _departmentService.CreateDepartmentAsync(dto);

        if (result.Status ==
            DepartmentServiceStatus.NameAlreadyExists)
        {
            return Conflict(
                "A department with this name already exists.");
        }

        return CreatedAtAction(
            nameof(GetDepartment),
            new { id = result.Data!.Id },
            result.Data);
    }

    // PUT: api/departments/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDepartment(
        int id,
        UpdateDepartmentDto dto)
    {
        var result =
            await _departmentService.UpdateDepartmentAsync(
                id,
                dto);

        if (result.Status ==
            DepartmentServiceStatus.NotFound)
        {
            return NotFound();
        }

        if (result.Status ==
            DepartmentServiceStatus.NameAlreadyExists)
        {
            return Conflict(
                "A department with this name already exists.");
        }

        return NoContent();
    }

    // DELETE: api/departments/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var result =
            await _departmentService.DeleteDepartmentAsync(id);

        if (result.Status ==
            DepartmentServiceStatus.NotFound)
        {
            return NotFound();
        }

        return NoContent();
    }
}