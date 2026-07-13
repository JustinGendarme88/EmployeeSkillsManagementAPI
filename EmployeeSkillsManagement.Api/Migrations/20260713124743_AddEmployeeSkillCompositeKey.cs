using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeSkillsManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeSkillCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeSkills",
                table: "EmployeeSkills");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeSkills_EmployeeId",
                table: "EmployeeSkills");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EmployeeSkills");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeSkills",
                table: "EmployeeSkills",
                columns: new[] { "EmployeeId", "SkillId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeSkills",
                table: "EmployeeSkills");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "EmployeeSkills",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeSkills",
                table: "EmployeeSkills",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkills_EmployeeId",
                table: "EmployeeSkills",
                column: "EmployeeId");
        }
    }
}
