using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LLCStroyCom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniqueIndexForProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_project_city_company_id_name",
                table: "project",
                columns: new[] { "city", "company_id", "name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_project_city_company_id_name",
                table: "project");
        }
    }
}
