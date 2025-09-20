using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LLCStroyCom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniqueCompanyNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_company_name",
                table: "company",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_company_name",
                table: "company");
        }
    }
}
