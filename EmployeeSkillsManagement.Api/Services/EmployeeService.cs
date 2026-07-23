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

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(employee => employee.Department)
            .FirstOrDefaultAsync(employee => employee.Id == id);
    }

    public async Task<EmployeeServiceResult<Employee>> CreateEmployeeAsync(
        CreateEmployeeDto dto)
    {
        var departmentExists = await _context.Departments
            .AnyAsync(department =>
                department.Id == dto.DepartmentId);

        if (!departmentExists)
        {
            return new EmployeeServiceResult<Employee>
            {
                Status = EmployeeServiceStatus.DepartmentNotFound
            };
        }

        var normalizedFirstName = dto.FirstName.Trim();
        var normalizedLastName = dto.LastName.Trim();
        var normalizedEmail = dto.Email.Trim().ToLower();

        var emailExists = await _context.Employees
            .AnyAsync(employee =>
                employee.Email.ToLower() == normalizedEmail);

        if (emailExists)
        {
            return new EmployeeServiceResult<Employee>
            {
                Status = EmployeeServiceStatus.EmailAlreadyExists
            };
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

        var createdEmployee = await _context.Employees
            .AsNoTracking()
            .Include(existingEmployee =>
                existingEmployee.Department)
            .FirstAsync(existingEmployee =>
                existingEmployee.Id == employee.Id);

        return new EmployeeServiceResult<Employee>
        {
            Status = EmployeeServiceStatus.Success,
            Data = createdEmployee
        };
    }

    public async Task<EmployeeServiceResult<bool>> UpdateEmployeeAsync(
        int id,
        UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return new EmployeeServiceResult<bool>
            {
                Status = EmployeeServiceStatus.NotFound
            };
        }

        var departmentExists = await _context.Departments
            .AnyAsync(department =>
                department.Id == dto.DepartmentId);

        if (!departmentExists)
        {
            return new EmployeeServiceResult<bool>
            {
                Status = EmployeeServiceStatus.DepartmentNotFound
            };
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
            return new EmployeeServiceResult<bool>
            {
                Status = EmployeeServiceStatus.EmailAlreadyExists
            };
        }

        employee.FirstName = normalizedFirstName;
        employee.LastName = normalizedLastName;
        employee.Email = normalizedEmail;
        employee.DepartmentId = dto.DepartmentId;

        await _context.SaveChangesAsync();

        return new EmployeeServiceResult<bool>
        {
            Status = EmployeeServiceStatus.Success,
            Data = true
        };
    }

    public async Task<EmployeeServiceResult<bool>> DeleteEmployeeAsync(
        int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return new EmployeeServiceResult<bool>
            {
                Status = EmployeeServiceStatus.NotFound
            };
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return new EmployeeServiceResult<bool>
        {
            Status = EmployeeServiceStatus.Success,
            Data = true
        };
    }
}