using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LLCStroyCom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserCompanyRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "user",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_user_CompanyId",
                table: "user",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_company_CompanyId",
                table: "user",
                column: "CompanyId",
                principalTable: "company",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_company_CompanyId",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_CompanyId",
                table: "user");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "user");
        }
    }
}
