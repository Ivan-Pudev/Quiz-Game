🧠 QuizGame

QuizGame is an ASP.NET Core MVC web application where users can play quizzes, track their scores, and compete on leaderboards.
Admins can create and manage quizzes, questions, and answers, while players can attempt quizzes multiple times and view rankings.

🚀 Features
👤 Authentication & Roles

ASP.NET Core Identity

User registration & login

📝 Quizzes

Create, edit, delete quizzes

Assign / remove questions dynamically

Each quiz has:

Title

Description

Start time

Multiple questions with answers

🎮 Play Game

Start a quiz attempt

Answer questions one by one

Automatic score calculation

Multiple attempts per user allowed

Game summary after finishing

🏆 Leaderboards

One leaderboard per quiz

Shows all attempts (not limited to one per user)

Ranked dynamically by score

Accessible from:

Quiz details

Game summary

Leaderboards index

📊 Game Summary

Final score

Max possible score

Correct answers count

Total questions

Direct link to leaderboard

🏗️ Architecture

The application follows SOLID principles and a clean separation of concerns.

Layers
QuizGame
│
├── QuizGame.Data        // EF Core entities & DbContext
├── QuizGame.Core        // Business logic (Services)
├── QuizGame.ViewModels  // UI models
├── QuizGame.Controllers
└── QuizGame.Views

Key Services

QuizzesService – quiz CRUD & leaderboard integration

GameService – gameplay, attempts, scoring

LeaderboardsService – leaderboard queries & ranking

🧩 Domain Models (simplified)

Quiz

Question

Answer

QuizAttempt

Leaderboard

LeaderboardEntry

🖥️ Controllers
Controller	Responsibility
QuizzesController	Manage quizzes (CRUD, details)
PlayController	Play quiz, submit answers, finish game
LeaderboardsController	View leaderboards
Account / Identity	Authentication
🧪 ViewModels

CreateQuizViewModel

EditQuizViewModel

DetailsQuizViewModel

GameSummaryViewModel

LeaderboardRowVm

ViewModels are never used for database access.

🛠️ Technologies Used

ASP.NET Core MVC

Entity Framework Core

SQL Server

ASP.NET Core Identity

Bootstrap 5

Razor Views

LINQ

⚙️ Setup Instructions
1️⃣ Clone the repository
git clone https://github.com/<Ivan-Pudev>/QuizGame.git
cd QuizGame

2️⃣ Configure database

Update appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=QuizGameDb;Trusted_Connection=True;"
}

3️⃣ Apply migrations
dotnet ef database update

4️⃣ Run the app
dotnet run

🔐 Default Roles

Create roles on startup or seed:

Player

Players can manage quizzes, players can only play.

🧠 Design Decisions

Multiple attempts per user are allowed

Leaderboard rank is calculated dynamically (not stored)

No business logic in controllers

No database access in views

EF Core tracking used only where needed

📸 Screenshots (optional)

Add screenshots of:

Quiz list

Play quiz

Game summary

Leaderboard

📄 License

This project is for educational purposes.

✨ Future Improvements

Timed questions

Question categories filtering

Pagination for leaderboards

Answer review after game

Admin dashboard

👨‍💻 Author

Ivan Pudev
ASP.NET Core MVC Quiz Game Project
