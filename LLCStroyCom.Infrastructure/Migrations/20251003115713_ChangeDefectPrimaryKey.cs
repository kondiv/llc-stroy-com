using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LLCStroyCom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDefectPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_defect",
                table: "defect");

            migrationBuilder.DropIndex(
                name: "IX_defect_project_id",
                table: "defect");

            migrationBuilder.AddPrimaryKey(
                name: "PK_defect",
                table: "defect",
                columns: new[] { "project_id", "id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_defect",
                table: "defect");

            migrationBuilder.AddPrimaryKey(
                name: "PK_defect",
                table: "defect",
                columns: new[] { "id", "project_id" });

            migrationBuilder.CreateIndex(
                name: "IX_defect_project_id",
                table: "defect",
                column: "project_id");
        }
    }
}
