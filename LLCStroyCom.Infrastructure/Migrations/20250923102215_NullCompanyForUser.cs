using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LLCStroyCom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NullCompanyForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_company_CompanyId",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "user",
                newName: "company_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_CompanyId",
                table: "user",
                newName: "IX_user_company_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "company_id",
                table: "user",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_user_company_company_id",
                table: "user",
                column: "company_id",
                principalTable: "company",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_company_company_id",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "user",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_user_company_id",
                table: "user",
                newName: "IX_user_CompanyId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "user",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_user_company_CompanyId",
                table: "user",
                column: "CompanyId",
                principalTable: "company",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
