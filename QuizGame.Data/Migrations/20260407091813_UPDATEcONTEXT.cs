using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizGame.Data.Migrations
{
    /// <inheritdoc />
    public partial class UPDATEcONTEXT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Leaderboards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "LeaderboardEntries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Leaderboards",
                keyColumn: "Id",
                keyValue: new Guid("a68f8eb4-76ef-41d8-beca-10bce9c61403"),
                columns: new[] { "IsDeleted", "LastUpdated" },
                values: new object[] { false, new DateOnly(2026, 4, 7) });

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: new Guid("4301f783-5664-41fc-af53-c2de0e1e454a"),
                column: "IsDeleted",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Leaderboards");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "LeaderboardEntries");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "Leaderboards",
                keyColumn: "Id",
                keyValue: new Guid("a68f8eb4-76ef-41d8-beca-10bce9c61403"),
                column: "LastUpdated",
                value: new DateOnly(2026, 3, 29));
        }
    }
}
