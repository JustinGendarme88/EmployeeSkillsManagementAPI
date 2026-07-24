using EmployeeSkillsManagement.Api.Data;
using EmployeeSkillsManagement.Api.DTOs;
using EmployeeSkillsManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSkillsManagement.Api.Services;

public class DepartmentService : IDepartmentService
{
    private readonly ApplicationDbContext _context;

    public DepartmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Department>> GetDepartmentsAsync(
        string? name,
        int page,
        int pageSize)
    {
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

        return new PagedResult<Department>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Items = departments
        };
    }

    public async Task<Department?> GetDepartmentByIdAsync(int id)
    {
        return await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(department =>
                department.Id == id);
    }

    public async Task<DepartmentServiceResult<Department>>
        CreateDepartmentAsync(CreateDepartmentDto dto)
    {
        var normalizedName = dto.Name.Trim();

        var departmentExists = await _context.Departments
            .AnyAsync(department =>
                department.Name.ToLower() ==
                normalizedName.ToLower());

        if (departmentExists)
        {
            return new DepartmentServiceResult<Department>
            {
                Status = DepartmentServiceStatus.NameAlreadyExists
            };
        }

        var department = new Department
        {
            Name = normalizedName
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return new DepartmentServiceResult<Department>
        {
            Status = DepartmentServiceStatus.Success,
            Data = department
        };
    }

    public async Task<DepartmentServiceResult<bool>>
        UpdateDepartmentAsync(
            int id,
            UpdateDepartmentDto dto)
    {
        var department =
            await _context.Departments.FindAsync(id);

        if (department is null)
        {
            return new DepartmentServiceResult<bool>
            {
                Status = DepartmentServiceStatus.NotFound
            };
        }

        var normalizedName = dto.Name.Trim();

        var departmentExists = await _context.Departments
            .AnyAsync(existingDepartment =>
                existingDepartment.Id != id &&
                existingDepartment.Name.ToLower() ==
                normalizedName.ToLower());

        if (departmentExists)
        {
            return new DepartmentServiceResult<bool>
            {
                Status = DepartmentServiceStatus.NameAlreadyExists
            };
        }

        department.Name = normalizedName;

        await _context.SaveChangesAsync();

        return new DepartmentServiceResult<bool>
        {
            Status = DepartmentServiceStatus.Success,
            Data = true
        };
    }

    public async Task<DepartmentServiceResult<bool>>
        DeleteDepartmentAsync(int id)
    {
        var department =
            await _context.Departments.FindAsync(id);

        if (department is null)
        {
            return new DepartmentServiceResult<bool>
            {
                Status = DepartmentServiceStatus.NotFound
            };
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return new DepartmentServiceResult<bool>
        {
            Status = DepartmentServiceStatus.Success,
            Data = true
        };
    }
}