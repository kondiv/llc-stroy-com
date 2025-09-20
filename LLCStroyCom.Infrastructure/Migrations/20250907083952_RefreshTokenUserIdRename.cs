using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LLCStroyCom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefreshTokenUserIdRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_token_user_UserId",
                table: "refresh_token");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "refresh_token",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_token_UserId",
                table: "refresh_token",
                newName: "IX_refresh_token_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_token_user_user_id",
                table: "refresh_token",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_token_user_user_id",
                table: "refresh_token");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "refresh_token",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_token_user_id",
                table: "refresh_token",
                newName: "IX_refresh_token_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_token_user_UserId",
                table: "refresh_token",
                column: "UserId",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
