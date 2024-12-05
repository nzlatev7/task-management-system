using Microsoft.EntityFrameworkCore.Migrations;
using TaskManagementSystem.Enums;

#nullable disable

namespace TaskManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Added_Priority_Field_To_TaskEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "priority",
                table: "task",
                type: "smallint",
                nullable: false,
                defaultValue: (short)Priority.Medium);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "priority",
                table: "task");
        }
    }
}
