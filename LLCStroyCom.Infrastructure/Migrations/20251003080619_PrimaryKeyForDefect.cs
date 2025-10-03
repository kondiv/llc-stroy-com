using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LLCStroyCom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PrimaryKeyForDefect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_defect",
                table: "defect");

            migrationBuilder.AddPrimaryKey(
                name: "PK_defect",
                table: "defect",
                columns: new[] { "id", "project_id" });
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
                column: "id");
        }
    }
}
