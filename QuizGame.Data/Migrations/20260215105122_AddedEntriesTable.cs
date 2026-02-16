using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizGame.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedEntriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaderboardEntry_AspNetUsers_UserId",
                table: "LeaderboardEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaderboardEntry_Leaderboards_LeaderboardId",
                table: "LeaderboardEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaderboardEntry",
                table: "LeaderboardEntry");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "LeaderboardEntry");

            migrationBuilder.RenameTable(
                name: "LeaderboardEntry",
                newName: "LeaderboardEntries");

            migrationBuilder.RenameIndex(
                name: "IX_LeaderboardEntry_UserId",
                table: "LeaderboardEntries",
                newName: "IX_LeaderboardEntries_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LeaderboardEntry_LeaderboardId_UserId",
                table: "LeaderboardEntries",
                newName: "IX_LeaderboardEntries_LeaderboardId_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaderboardEntries",
                table: "LeaderboardEntries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaderboardEntries_AspNetUsers_UserId",
                table: "LeaderboardEntries",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaderboardEntries_Leaderboards_LeaderboardId",
                table: "LeaderboardEntries",
                column: "LeaderboardId",
                principalTable: "Leaderboards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaderboardEntries_AspNetUsers_UserId",
                table: "LeaderboardEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaderboardEntries_Leaderboards_LeaderboardId",
                table: "LeaderboardEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaderboardEntries",
                table: "LeaderboardEntries");

            migrationBuilder.RenameTable(
                name: "LeaderboardEntries",
                newName: "LeaderboardEntry");

            migrationBuilder.RenameIndex(
                name: "IX_LeaderboardEntries_UserId",
                table: "LeaderboardEntry",
                newName: "IX_LeaderboardEntry_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LeaderboardEntries_LeaderboardId_UserId",
                table: "LeaderboardEntry",
                newName: "IX_LeaderboardEntry_LeaderboardId_UserId");

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "LeaderboardEntry",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaderboardEntry",
                table: "LeaderboardEntry",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaderboardEntry_AspNetUsers_UserId",
                table: "LeaderboardEntry",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaderboardEntry_Leaderboards_LeaderboardId",
                table: "LeaderboardEntry",
                column: "LeaderboardId",
                principalTable: "Leaderboards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
