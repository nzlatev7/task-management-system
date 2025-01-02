using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Removed_IsCompleted_Filed_From_DeletedTaskEntity_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_completed",
                table: "deleted_task");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_completed",
                table: "deleted_task",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
