using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizGame.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedRankPropertyToEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "LeaderboardEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "LeaderboardEntries");
        }
    }
}
