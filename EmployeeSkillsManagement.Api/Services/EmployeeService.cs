using EmployeeSkillsManagement.Api.Data;
using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSkillsManagement.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _context;

    public EmployeeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Employee>> GetEmployeesAsync(
        string? name,
        string? email,
        int? departmentId,
        int page,
        int pageSize)
    {
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

        return new PagedResult<Employee>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Items = employees
        };
    }
}