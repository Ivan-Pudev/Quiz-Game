using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizGame.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedMultipleAttemptsPerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeaderboardEntries_LeaderboardId_UserId",
                table: "LeaderboardEntries");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardEntries_LeaderboardId_UserId",
                table: "LeaderboardEntries",
                columns: new[] { "LeaderboardId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeaderboardEntries_LeaderboardId_UserId",
                table: "LeaderboardEntries");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardEntries_LeaderboardId_UserId",
                table: "LeaderboardEntries",
                columns: new[] { "LeaderboardId", "UserId" },
                unique: true);
        }
    }
}
