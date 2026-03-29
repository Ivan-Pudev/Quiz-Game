using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizGame.Data.Migrations
{
    /// <inheritdoc />
    public partial class QuizLeaderboardRelationFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaderboardId",
                table: "Quizzes");

            migrationBuilder.UpdateData(
                table: "Leaderboards",
                keyColumn: "Id",
                keyValue: new Guid("a68f8eb4-76ef-41d8-beca-10bce9c61403"),
                column: "LastUpdated",
                value: new DateOnly(2026, 3, 29));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LeaderboardId",
                table: "Quizzes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Leaderboards",
                keyColumn: "Id",
                keyValue: new Guid("a68f8eb4-76ef-41d8-beca-10bce9c61403"),
                column: "LastUpdated",
                value: new DateOnly(2026, 3, 25));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: new Guid("4301f783-5664-41fc-af53-c2de0e1e454a"),
                column: "LeaderboardId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
