using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LLCStroyCom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedStatusEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_project_status_status_id",
                table: "project");

            migrationBuilder.DropTable(
                name: "status");

            migrationBuilder.DropIndex(
                name: "IX_project_status_id",
                table: "project");

            migrationBuilder.RenameColumn(
                name: "status_id",
                table: "project",
                newName: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "project",
                newName: "status_id");

            migrationBuilder.CreateTable(
                name: "status",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(31)", maxLength: 31, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_status", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_project_status_id",
                table: "project",
                column: "status_id");

            migrationBuilder.AddForeignKey(
                name: "FK_project_status_status_id",
                table: "project",
                column: "status_id",
                principalTable: "status",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
