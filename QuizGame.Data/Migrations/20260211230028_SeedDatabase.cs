using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuizGame.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_AspNetUsers_UserId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Questions_QuestionId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaderboardEntry_Leaderboards_LeaderboardId",
                table: "LeaderboardEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaderboardEntry_Quizzes_QuizId",
                table: "LeaderboardEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Quizzes_QuizId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_QuizId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_LeaderboardEntry_QuizId",
                table: "LeaderboardEntry");

            migrationBuilder.DropIndex(
                name: "IX_Categories_QuestionId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Answers_UserId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "LeaderboardEntry");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "LeaderboardEntry");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Answers");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Leaderboards",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "LastUpdated",
                table: "Leaderboards",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "QuizId",
                table: "Leaderboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Leaderboards",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "LeaderboardId",
                table: "LeaderboardEntry",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Categories",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CategoriesQuestions",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    QuestionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesQuestions", x => new { x.CategoriesId, x.QuestionsId });
                    table.ForeignKey(
                        name: "FK_CategoriesQuestions_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoriesQuestions_Questions_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionQuiz",
                columns: table => new
                {
                    QuestionsId = table.Column<int>(type: "int", nullable: false),
                    QuizzesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionQuiz", x => new { x.QuestionsId, x.QuizzesId });
                    table.ForeignKey(
                        name: "FK_QuestionQuiz_Questions_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionQuiz_Quizzes_QuizzesId",
                        column: x => x.QuizzesId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "https://img.freepik.com/free-photo/blackboard-inscribed-with-scientific-formulas-calculations_1150-19413.jpg?semt=ais_hybrid&w=740&q=80", "Math" },
                    { 2, "https://img.freepik.com/free-vector/geography-subject-with-worldmap-books_1308-30998.jpg?semt=ais_hybrid&w=740&q=80", "Geography" },
                    { 3, null, "Science" },
                    { 4, "https://dualcreditathome.com/wp-content/uploads/2014/02/history.jpg", "History" },
                    { 5, null, "Hobbies" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "Complexity", "Content", "Points", "QuestionType", "TimeLimit" },
                values: new object[,]
                {
                    { 1, 1, "Which planet is known as the Red Planet?", 10, 2, 15 },
                    { 2, 2, "The Great Wall of China was built in a single century.", 15, 3, 10 },
                    { 3, 3, "What is the chemical symbol for Gold?", 25, 4, 20 },
                    { 4, 2, "Who painted the 'Starry Night'?", 20, 2, 20 },
                    { 5, 3, "Sound travels faster in water than in air.", 20, 3, 15 },
                    { 6, 1, "Which country is home to the Kangaroo?", 10, 2, 10 },
                    { 7, 2, "What is the square root of 144?", 15, 4, 15 },
                    { 8, 3, "In which year did the Titanic sink?", 30, 2, 25 },
                    { 9, 1, "Humans have four lungs.", 10, 3, 10 },
                    { 10, 4, "Which element has the atomic number 1?", 40, 4, 20 }
                });

            migrationBuilder.InsertData(
                table: "Quizzes",
                columns: new[] { "Id", "Description", "StartTime", "Title" },
                values: new object[] { 1, "A mix of everything!", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Friday Night Trivia" });

            migrationBuilder.InsertData(
                table: "Answers",
                columns: new[] { "Id", "Content", "IsCorrect", "QuestionId" },
                values: new object[,]
                {
                    { 1, "Mars", true, 1 },
                    { 2, "Venus", false, 1 },
                    { 3, "Jupiter", false, 1 },
                    { 4, "True", false, 2 },
                    { 5, "False", true, 2 },
                    { 6, "Au", true, 3 },
                    { 7, "Ag", false, 3 },
                    { 8, "Gd", false, 3 },
                    { 9, "Vincent van Gogh", true, 4 },
                    { 10, "Pablo Picasso", false, 4 },
                    { 11, "Claude Monet", false, 4 },
                    { 12, "True", true, 5 },
                    { 13, "False", false, 5 },
                    { 14, "Australia", true, 6 },
                    { 15, "South Africa", false, 6 },
                    { 16, "Brazil", false, 6 },
                    { 17, "12", true, 7 },
                    { 18, "14", false, 7 },
                    { 19, "16", false, 7 },
                    { 20, "1912", true, 8 },
                    { 21, "1905", false, 8 },
                    { 22, "1920", false, 8 },
                    { 23, "True", false, 9 },
                    { 24, "False", true, 9 },
                    { 25, "Hydrogen", true, 10 },
                    { 26, "Helium", false, 10 },
                    { 27, "Oxygen", false, 10 }
                });

            migrationBuilder.InsertData(
                table: "CategoriesQuestions",
                columns: new[] { "CategoriesId", "QuestionsId" },
                values: new object[,]
                {
                    { 1, 7 },
                    { 2, 1 },
                    { 2, 2 },
                    { 2, 6 },
                    { 3, 1 },
                    { 3, 3 },
                    { 3, 5 },
                    { 3, 10 },
                    { 4, 2 },
                    { 4, 6 },
                    { 4, 8 }
                });

            migrationBuilder.InsertData(
                table: "Leaderboards",
                columns: new[] { "Id", "Description", "LastUpdated", "QuizId", "Title" },
                values: new object[] { 1, "Top scores for friday players", new DateOnly(2026, 2, 11), 1, "Friday Night Rankings" });

            migrationBuilder.InsertData(
                table: "QuestionQuiz",
                columns: new[] { "QuestionsId", "QuizzesId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboards_QuizId",
                table: "Leaderboards",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesQuestions_QuestionsId",
                table: "CategoriesQuestions",
                column: "QuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionQuiz_QuizzesId",
                table: "QuestionQuiz",
                column: "QuizzesId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaderboardEntry_Leaderboards_LeaderboardId",
                table: "LeaderboardEntry",
                column: "LeaderboardId",
                principalTable: "Leaderboards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Leaderboards_Quizzes_QuizId",
                table: "Leaderboards",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaderboardEntry_Leaderboards_LeaderboardId",
                table: "LeaderboardEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaderboards_Quizzes_QuizId",
                table: "Leaderboards");

            migrationBuilder.DropTable(
                name: "CategoriesQuestions");

            migrationBuilder.DropTable(
                name: "QuestionQuiz");

            migrationBuilder.DropIndex(
                name: "IX_Leaderboards_QuizId",
                table: "Leaderboards");

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Answers",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Leaderboards",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Leaderboards");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Leaderboards");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "Leaderboards");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Leaderboards");

            migrationBuilder.AddColumn<int>(
                name: "QuizId",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LeaderboardId",
                table: "LeaderboardEntry",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LastUpdated",
                table: "LeaderboardEntry",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuizId",
                table: "LeaderboardEntry",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Categories",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Answers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizId",
                table: "Questions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardEntry_QuizId",
                table: "LeaderboardEntry",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_QuestionId",
                table: "Categories",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_UserId",
                table: "Answers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_AspNetUsers_UserId",
                table: "Answers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Questions_QuestionId",
                table: "Categories",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaderboardEntry_Leaderboards_LeaderboardId",
                table: "LeaderboardEntry",
                column: "LeaderboardId",
                principalTable: "Leaderboards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaderboardEntry_Quizzes_QuizId",
                table: "LeaderboardEntry",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Quizzes_QuizId",
                table: "Questions",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id");
        }
    }
}
