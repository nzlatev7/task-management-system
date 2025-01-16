using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Added_FluentApi_Configuration_For_DeletedTaskEntity_Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_deleted_task_category_category_id",
                table: "deleted_task");

            migrationBuilder.AddForeignKey(
                name: "FK_deleted_task_category_category_id",
                table: "deleted_task",
                column: "category_id",
                principalTable: "category",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_deleted_task_category_category_id",
                table: "deleted_task");

            migrationBuilder.AddForeignKey(
                name: "FK_deleted_task_category_category_id",
                table: "deleted_task",
                column: "category_id",
                principalTable: "category",
                principalColumn: "id");
        }
    }
}
