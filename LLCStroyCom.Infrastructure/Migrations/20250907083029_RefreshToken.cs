using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LLCStroyCom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_role_RoleId",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "user",
                newName: "role_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_RoleId",
                table: "user",
                newName: "IX_user_role_id");

            migrationBuilder.CreateTable(
                name: "refresh_token",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_token_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "Ck_ApplicatinoUser_RoleId_Positive",
                table: "user",
                sql: "role_id > 0");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_UserId",
                table: "refresh_token",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_role_role_id",
                table: "user",
                column: "role_id",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_role_role_id",
                table: "user");

            migrationBuilder.DropTable(
                name: "refresh_token");

            migrationBuilder.DropIndex(
                name: "IX_user_email",
                table: "user");

            migrationBuilder.DropCheckConstraint(
                name: "Ck_ApplicatinoUser_RoleId_Positive",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "user",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_user_role_id",
                table: "user",
                newName: "IX_user_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_role_RoleId",
                table: "user",
                column: "RoleId",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
