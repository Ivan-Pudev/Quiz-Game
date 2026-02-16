using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizGame.Data.Migrations
{
    /// <inheritdoc />
    public partial class LeaderboardRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Leaderboards_QuizId",
                table: "Leaderboards");

            migrationBuilder.DropIndex(
                name: "IX_LeaderboardEntry_LeaderboardId",
                table: "LeaderboardEntry");

            migrationBuilder.AddColumn<int>(
                name: "LeaderboardId",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1,
                column: "LeaderboardId",
                value: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboards_QuizId",
                table: "Leaderboards",
                column: "QuizId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardEntry_LeaderboardId_UserId",
                table: "LeaderboardEntry",
                columns: new[] { "LeaderboardId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Leaderboards_QuizId",
                table: "Leaderboards");

            migrationBuilder.DropIndex(
                name: "IX_LeaderboardEntry_LeaderboardId_UserId",
                table: "LeaderboardEntry");

            migrationBuilder.DropColumn(
                name: "LeaderboardId",
                table: "Quizzes");

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboards_QuizId",
                table: "Leaderboards",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardEntry_LeaderboardId",
                table: "LeaderboardEntry",
                column: "LeaderboardId");
        }
    }
}
