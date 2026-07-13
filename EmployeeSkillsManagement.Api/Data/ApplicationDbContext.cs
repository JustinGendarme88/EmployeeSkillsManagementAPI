using EmployeeSkillsManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSkillsManagement.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<EmployeeSkill> EmployeeSkills => Set<EmployeeSkill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmployeeSkill>()
            .HasKey(employeeSkill => new
            {
                employeeSkill.EmployeeId,
                employeeSkill.SkillId
            });

        modelBuilder.Entity<EmployeeSkill>()
            .HasOne(employeeSkill => employeeSkill.Employee)
            .WithMany(employee => employee.EmployeeSkills)
            .HasForeignKey(employeeSkill => employeeSkill.EmployeeId);

        modelBuilder.Entity<EmployeeSkill>()
            .HasOne(employeeSkill => employeeSkill.Skill)
            .WithMany(skill => skill.EmployeeSkills)
            .HasForeignKey(employeeSkill => employeeSkill.SkillId);
    }
}