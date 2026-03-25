using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuizGame.Data.Migrations
{
    /// <inheritdoc />
    public partial class IntroduceApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    Complexity = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeaderboardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesQuestions",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                name: "Leaderboards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    LastUpdated = table.Column<DateOnly>(type: "date", nullable: false),
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaderboards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leaderboards_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionQuiz",
                columns: table => new
                {
                    QuestionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuizzesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "QuizAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentQuestionIndex = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    MaxScore = table.Column<int>(type: "int", nullable: false),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaderboardEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    LeaderboardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaderboardEntries_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaderboardEntries_Leaderboards_LeaderboardId",
                        column: x => x.LeaderboardId,
                        principalTable: "Leaderboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttemptAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuizAttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SelectedAnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    EarnedPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttemptAnswers_Answers_SelectedAnswerId",
                        column: x => x.SelectedAnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttemptAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttemptAnswers_QuizAttempts_QuizAttemptId",
                        column: x => x.QuizAttemptId,
                        principalTable: "QuizAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { new Guid("211a7b3d-1535-4fae-9015-9ce026df66f9"), "https://img.freepik.com/free-photo/blackboard-inscribed-with-scientific-formulas-calculations_1150-19413.jpg?semt=ais_hybrid&w=740&q=80", "Math" },
                    { new Guid("31961ab3-d6c8-43f4-8744-d9b21a815ed0"), "https://dualcreditathome.com/wp-content/uploads/2014/02/history.jpg", "History" },
                    { new Guid("644fb15e-3f0a-4d29-aae9-7deb3f08ee5c"), "https://img.freepik.com/free-vector/geography-subject-with-worldmap-books_1308-30998.jpg?semt=ais_hybrid&w=740&q=80", "Geography" },
                    { new Guid("915f826d-fe20-4be5-a8f2-37a65c9a92c4"), null, "Hobbies" },
                    { new Guid("b0cc81f8-da63-4ce3-ad27-93298ccf26c1"), null, "Science" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "Complexity", "Content", "Points", "QuestionType" },
                values: new object[,]
                {
                    { new Guid("04df888f-1067-4e40-821b-9892fc603f5b"), 1, "Humans have four lungs.", 10, 3 },
                    { new Guid("49850a02-893a-45e9-8e5b-2a01a040d60e"), 3, "Sound travels faster in water than in air.", 20, 3 },
                    { new Guid("5267700d-9487-4ab0-9e24-3962e71df82e"), 2, "What is the square root of 144?", 15, 4 },
                    { new Guid("6271f595-33f5-480e-978b-02f9febc50de"), 1, "Which country is home to the Kangaroo?", 10, 2 },
                    { new Guid("698f8ac5-a4ae-4031-a7fa-f4bf245f374e"), 3, "In which year did the Titanic sink?", 30, 2 },
                    { new Guid("7043874b-ed1a-4ab0-8519-5dc8408abf68"), 1, "Which planet is known as the Red Planet?", 10, 2 },
                    { new Guid("7c6b2449-c14b-4da6-85e5-6e511a16e0ec"), 2, "The Great Wall of China was built in a single century.", 15, 3 },
                    { new Guid("9b3b9e33-4e2a-4ea2-9ae0-7c0ab10cb09b"), 3, "What is the chemical symbol for Gold?", 25, 4 },
                    { new Guid("adc049bd-1ef8-423b-9c37-a0e0b2708595"), 2, "Who painted the 'Starry Night'?", 20, 2 },
                    { new Guid("e861c8e7-8dbd-447b-9943-78c812a14768"), 4, "Which element has the atomic number 1?", 40, 4 }
                });

            migrationBuilder.InsertData(
                table: "Quizzes",
                columns: new[] { "Id", "Description", "LeaderboardId", "StartTime", "Title" },
                values: new object[] { new Guid("4301f783-5664-41fc-af53-c2de0e1e454a"), "A mix of everything!", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Friday Night Trivia" });

            migrationBuilder.InsertData(
                table: "Answers",
                columns: new[] { "Id", "Content", "IsCorrect", "QuestionId" },
                values: new object[,]
                {
                    { new Guid("241b93eb-9da0-4c28-b71c-be72e60d81e5"), "12", true, new Guid("5267700d-9487-4ab0-9e24-3962e71df82e") },
                    { new Guid("2f1c439a-eb0e-40b3-8106-25f79c81b9d1"), "1905", false, new Guid("698f8ac5-a4ae-4031-a7fa-f4bf245f374e") },
                    { new Guid("3922d6ba-c178-44d6-b080-109bccd7af25"), "False", true, new Guid("04df888f-1067-4e40-821b-9892fc603f5b") },
                    { new Guid("3dfbcde1-75c8-495a-9c33-f8551c040928"), "False", false, new Guid("49850a02-893a-45e9-8e5b-2a01a040d60e") },
                    { new Guid("4643ff98-ea40-4f28-aa75-1a4628263c72"), "Gd", false, new Guid("9b3b9e33-4e2a-4ea2-9ae0-7c0ab10cb09b") },
                    { new Guid("4d0eb419-e569-484a-aaa6-ee3cc330b47d"), "14", false, new Guid("5267700d-9487-4ab0-9e24-3962e71df82e") },
                    { new Guid("4df293fa-7c21-43b4-af7e-224c627adcf3"), "Ag", false, new Guid("9b3b9e33-4e2a-4ea2-9ae0-7c0ab10cb09b") },
                    { new Guid("54b83914-8e31-4ce0-abd2-5eca6289248f"), "South Africa", false, new Guid("6271f595-33f5-480e-978b-02f9febc50de") },
                    { new Guid("5804614e-88a6-48a5-a9a6-faf1c76ddc77"), "Oxygen", false, new Guid("e861c8e7-8dbd-447b-9943-78c812a14768") },
                    { new Guid("616afaf1-57b7-45c5-aeb1-6268efbc2337"), "True", false, new Guid("04df888f-1067-4e40-821b-9892fc603f5b") },
                    { new Guid("61a0689b-7621-4862-8c67-7dde69f2d2c3"), "1912", true, new Guid("698f8ac5-a4ae-4031-a7fa-f4bf245f374e") },
                    { new Guid("653274d3-62e7-4fe3-9eb3-c5fc0f54e5fa"), "Hydrogen", true, new Guid("e861c8e7-8dbd-447b-9943-78c812a14768") },
                    { new Guid("65ea829c-9836-411d-8c36-8592770bd3a8"), "Jupiter", false, new Guid("7043874b-ed1a-4ab0-8519-5dc8408abf68") },
                    { new Guid("6c05587a-61a7-4d0c-ba67-9a14c32f1c7a"), "16", false, new Guid("5267700d-9487-4ab0-9e24-3962e71df82e") },
                    { new Guid("6df6355c-2db9-4d95-8928-2ff9d084576a"), "Australia", true, new Guid("6271f595-33f5-480e-978b-02f9febc50de") },
                    { new Guid("78ea56a6-5157-491e-87b9-93c92e4cc54a"), "Claude Monet", false, new Guid("adc049bd-1ef8-423b-9c37-a0e0b2708595") },
                    { new Guid("79a88322-5898-4b26-8cd7-35d41fac5dcd"), "True", true, new Guid("49850a02-893a-45e9-8e5b-2a01a040d60e") },
                    { new Guid("80ada585-44cf-44c8-8265-288dd2789c0d"), "Pablo Picasso", false, new Guid("adc049bd-1ef8-423b-9c37-a0e0b2708595") },
                    { new Guid("862cccfe-37fa-4219-bbe2-020497a550e9"), "Vincent van Gogh", true, new Guid("adc049bd-1ef8-423b-9c37-a0e0b2708595") },
                    { new Guid("96e0ef87-3297-49ac-975e-77dc99fd09fe"), "Mars", true, new Guid("7043874b-ed1a-4ab0-8519-5dc8408abf68") },
                    { new Guid("a367a8af-d6b8-4034-b64e-25b32bb9263e"), "Venus", false, new Guid("7043874b-ed1a-4ab0-8519-5dc8408abf68") },
                    { new Guid("b0c63cdc-c8b0-480f-aee0-2fbf3c8ec052"), "True", false, new Guid("7c6b2449-c14b-4da6-85e5-6e511a16e0ec") },
                    { new Guid("c6e4797b-245a-43a5-b1cb-e7786affc96c"), "Au", true, new Guid("9b3b9e33-4e2a-4ea2-9ae0-7c0ab10cb09b") },
                    { new Guid("d1fc7d89-7ba0-4132-8890-877d30b1b12b"), "Brazil", false, new Guid("6271f595-33f5-480e-978b-02f9febc50de") },
                    { new Guid("ec7da6e0-c8ad-40dd-88b3-7f1a6176f9de"), "False", true, new Guid("7c6b2449-c14b-4da6-85e5-6e511a16e0ec") },
                    { new Guid("fcee2ffa-8b19-4650-9652-5e09aeb770ee"), "Helium", false, new Guid("e861c8e7-8dbd-447b-9943-78c812a14768") },
                    { new Guid("fd1cd964-91e3-496f-8c34-6311644fc383"), "1920", false, new Guid("698f8ac5-a4ae-4031-a7fa-f4bf245f374e") }
                });

            migrationBuilder.InsertData(
                table: "CategoriesQuestions",
                columns: new[] { "CategoriesId", "QuestionsId" },
                values: new object[,]
                {
                    { new Guid("211a7b3d-1535-4fae-9015-9ce026df66f9"), new Guid("5267700d-9487-4ab0-9e24-3962e71df82e") },
                    { new Guid("31961ab3-d6c8-43f4-8744-d9b21a815ed0"), new Guid("6271f595-33f5-480e-978b-02f9febc50de") },
                    { new Guid("31961ab3-d6c8-43f4-8744-d9b21a815ed0"), new Guid("698f8ac5-a4ae-4031-a7fa-f4bf245f374e") },
                    { new Guid("31961ab3-d6c8-43f4-8744-d9b21a815ed0"), new Guid("7c6b2449-c14b-4da6-85e5-6e511a16e0ec") },
                    { new Guid("644fb15e-3f0a-4d29-aae9-7deb3f08ee5c"), new Guid("6271f595-33f5-480e-978b-02f9febc50de") },
                    { new Guid("644fb15e-3f0a-4d29-aae9-7deb3f08ee5c"), new Guid("7c6b2449-c14b-4da6-85e5-6e511a16e0ec") },
                    { new Guid("b0cc81f8-da63-4ce3-ad27-93298ccf26c1"), new Guid("49850a02-893a-45e9-8e5b-2a01a040d60e") },
                    { new Guid("b0cc81f8-da63-4ce3-ad27-93298ccf26c1"), new Guid("7043874b-ed1a-4ab0-8519-5dc8408abf68") },
                    { new Guid("b0cc81f8-da63-4ce3-ad27-93298ccf26c1"), new Guid("9b3b9e33-4e2a-4ea2-9ae0-7c0ab10cb09b") },
                    { new Guid("b0cc81f8-da63-4ce3-ad27-93298ccf26c1"), new Guid("e861c8e7-8dbd-447b-9943-78c812a14768") }
                });

            migrationBuilder.InsertData(
                table: "Leaderboards",
                columns: new[] { "Id", "Description", "LastUpdated", "QuizId", "Title" },
                values: new object[] { new Guid("a68f8eb4-76ef-41d8-beca-10bce9c61403"), "Top scores for friday players", new DateOnly(2026, 3, 25), new Guid("4301f783-5664-41fc-af53-c2de0e1e454a"), "Friday Night Rankings" });

            migrationBuilder.InsertData(
                table: "QuestionQuiz",
                columns: new[] { "QuestionsId", "QuizzesId" },
                values: new object[,]
                {
                    { new Guid("7043874b-ed1a-4ab0-8519-5dc8408abf68"), new Guid("4301f783-5664-41fc-af53-c2de0e1e454a") },
                    { new Guid("7c6b2449-c14b-4da6-85e5-6e511a16e0ec"), new Guid("4301f783-5664-41fc-af53-c2de0e1e454a") },
                    { new Guid("9b3b9e33-4e2a-4ea2-9ae0-7c0ab10cb09b"), new Guid("4301f783-5664-41fc-af53-c2de0e1e454a") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptAnswers_QuestionId",
                table: "AttemptAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptAnswers_QuizAttemptId",
                table: "AttemptAnswers",
                column: "QuizAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptAnswers_SelectedAnswerId",
                table: "AttemptAnswers",
                column: "SelectedAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesQuestions_QuestionsId",
                table: "CategoriesQuestions",
                column: "QuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardEntries_LeaderboardId_UserId",
                table: "LeaderboardEntries",
                columns: new[] { "LeaderboardId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardEntries_UserId",
                table: "LeaderboardEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboards_QuizId",
                table: "Leaderboards",
                column: "QuizId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionQuiz_QuizzesId",
                table: "QuestionQuiz",
                column: "QuizzesId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_QuizId",
                table: "QuizAttempts",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_UserId",
                table: "QuizAttempts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AttemptAnswers");

            migrationBuilder.DropTable(
                name: "CategoriesQuestions");

            migrationBuilder.DropTable(
                name: "LeaderboardEntries");

            migrationBuilder.DropTable(
                name: "QuestionQuiz");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "QuizAttempts");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Leaderboards");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Quizzes");
        }
    }
}
