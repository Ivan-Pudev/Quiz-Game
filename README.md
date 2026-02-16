## 🧠 QuizGame

QuizGame is an ASP.NET Core MVC web application where users can play quizzes, track their scores, and compete on leaderboards.
Admins can create and manage quizzes, questions, and answers, while players can attempt quizzes multiple times and view rankings.

## 🚀 Features

👤 Authentication & Roles

ASP.NET Core Identity

User registration & login

## 📝 Quizzes

Create, edit, delete quizzes

Assign / remove questions dynamically

Each quiz has:

Title

Description

Start time

Leaderboard

Multiple questions with answers

## 🎮 Play Game

Start a quiz attempt

Answer questions one by one

Automatic score calculation

Multiple attempts per user allowed

Game summary after finishing

## 🏆 Leaderboards

One leaderboard per quiz

Shows all attempts (not limited to one per user)

Ranked dynamically by score

Accessible from:

Quiz details

Game summary

Leaderboards index

## 📊 Game Summary

Final score

Max possible score

Correct answers count

Total questions

Direct link to leaderboard

## 🏗️ Architecture

The application follows SOLID principles and a clean separation of concerns.

Layers

├── QuizGame.Web - Web logic

├── QuizGame.Data
├── QuizGame.Data.Models - Database and Domain Models

├── QuizGame.Core        - Business logic (Services)

├── QuizGame.ViewModels  - UI models

├── QuizGame.GCommon

Key Services

QuizzesService – quiz CRUD & leaderboard integration

GameService – gameplay, attempts, scoring

LeaderboardsService – leaderboard queries & ranking

## 🧩 Domain Models 

## Quiz

Description:

Represents a quiz with its metadata, scheduling details, and related entities.

Properties:

-Id: Unique identifier of the quiz.

-Title: Required quiz title with a maximum length constraint.

-Description: Required quiz description with a maximum length constraint.

-StartTime: Required date and time when the quiz starts.

Relationships:

-Leaderboard: Optional association used for tracking quiz scores.

-Questions: Collection of questions belonging to the quiz (one-to-many).

Notes

A quiz can exist without a leaderboard.

A quiz can contain multiple questions.

## Question

Description:

Represents a question used in quizzes, including its content, type, scoring, and relationships.

Properties:

-Id: Unique identifier of the question.

-Content: Required question text with a maximum length constraint.

-QuestionType: Required value defining the type of the question (e.g. multiple choice, true/false).

-Complexity: Numeric value representing the difficulty level of the question.

-Points: Required number of points awarded for a correct answer.

Relationships

-Quizzes: Collection of quizzes that include this question (many-to-many).

-Categories: Collection of categories associated with the question (many-to-many).

-Answers: Collection of possible answers for the question (one-to-many).

Notes

A question can be reused across multiple quizzes.

A question can belong to multiple categories.

A question should typically have at least one answer.

## Answer

Description:

Represents a possible answer to a question, including its content and correctness.

Properties:

-Id: Unique identifier of the answer.

-Content: Required answer text with a maximum length constraint.

-IsCorrect: Indicates whether the answer is correct.

-QuestionId: Required foreign key referencing the related question.

Relationships:

-Question: Required association to the question this answer belongs to (many-to-one).

Notes

Each answer must be linked to a question.

A question can have multiple answers.

Typically, at least one answer per question should be marked as correct.

## QuizAttempt

Description:

Represents a user’s attempt at taking a quiz, tracking progress, answers, and scoring.

Properties:

-Id: Unique identifier of the quiz attempt.

-QuizId: Foreign key referencing the associated quiz.

-UserId: Identifier of the user taking the quiz.

-CurrentQuestionIndex: Index of the current question the user is on.

-Score: Current score achieved by the user.

-MaxScore: Maximum possible score for the quiz.

-IsFinished: Indicates whether the quiz attempt has been completed.

Relationships

-Quiz: Required association to the quiz being attempted (many-to-one).

-Answers: Collection of answers submitted during the attempt (one-to-many).

Notes

A user can have multiple attempts for the same quiz (depending on business rules).

IsFinished should be set when all questions are answered.

Score should not exceed MaxScore.

## Leaderboard 

Description:

Represents a leaderboard associated with a quiz, used to track and display participant rankings and scores.

Properties:

-Id: Unique identifier of the leaderboard.

-Title: Required leaderboard title with a maximum length constraint.

-Description: Required leaderboard description with a maximum length constraint.

-LastUpdated: Required date indicating when the leaderboard was last updated.

-QuizId: Required foreign key referencing the related quiz.

Relationships

-Quiz: Required association to the quiz this leaderboard belongs to (one-to-one or many-to-one, depending on configuration).

-Entries: Collection of leaderboard entries representing individual user results (one-to-many).

Notes

Each leaderboard must be linked to a quiz.

LastUpdated should be refreshed whenever leaderboard entries change.

Leaderboard entries are used to calculate and display rankings.

## LeaderboardEntry 

Description:

Represents a single user’s result within a leaderboard, including score and ranking.

Properties:

-Id: Unique identifier of the leaderboard entry.

-UserId: Required identifier of the user associated with this entry.

-Score: Score achieved by the user in the quiz.

-Rank: Ranking position of the user on the leaderboard.

-LeaderboardId: Required foreign key referencing the related leaderboard.

Relationships:

-User: Required association to the application user.

-Leaderboard: Required association to the leaderboard this entry belongs to (many-to-one).

Notes

Each entry corresponds to one user on a leaderboard.

Rankings are typically recalculated when scores change.

A leaderboard can contain multiple entries.

## 🖥️ Controllers

Controller	Responsibility

QuizzesController	Manage quizzes (CRUD, details)

PlayController	Play quiz, submit answers, finish game

LeaderboardsController	View leaderboards

Account / Identity	Authentication

## 🧪 ViewModels

CreateQuizViewModel

EditQuizViewModel

DetailsQuizViewModel

GameSummaryViewModel

LeaderboardRowVm

ViewModels are never used for database access.

## 🛠️ Technologies Used

ASP.NET Core MVC

Entity Framework Core

SQL Server

ASP.NET Core Identity

Bootstrap 5

Razor Views

LINQ

## ⚙️ Setup Instructions
1️⃣ Clone the repository
git clone https://github.com/<Ivan-Pudev>/QuizGame
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

## 📸 Screenshots

### Play Quiz
![Play Quiz](screenshots/play-quiz.png)

### Game Summary
![Game Summary](screenshots/game-summary.png)

### Leaderboard
![Leaderboard](screenshots/leaderboard.png)

## Project Info

This project is for educational purposes.

✨ Future Improvements

Timed questions

Question categories filtering

Pagination for leaderboards

Answer review after game

Admin dashboard

## 👨‍💻 Author

Ivan Pudev
ASP.NET Core MVC Quiz Game Project
