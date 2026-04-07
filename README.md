# 🧠 QuizGame

**QuizGame** is an ASP.NET Core MVC web application for creating, managing, and playing quizzes.  
The platform supports role-based access, multiple quiz attempts, automatic scoring, and dynamic leaderboards so users can compete based on performance.

Admins can manage quizzes, questions, and answers, while players can sign in, attempt quizzes multiple times, and view ranked results for each quiz.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [How the Application Works](#how-the-application-works)
- [Architecture](#architecture)
- [Domain Models](#domain-models)
- [Controllers](#controllers)
- [ViewModels](#viewmodels)
- [Technologies Used](#technologies-used)
- [Setup Instructions](#setup-instructions)
- [Default Roles](#default-roles)
- [Design Decisions](#design-decisions)
- [Screenshots](#screenshots)
- [Future Improvements](#future-improvements)
- [Project Info](#project-info)
- [Author](#author)

---

## Overview

QuizGame is designed to provide a complete quiz experience with both **administrative management** and **player gameplay**.

The system allows:
- administrators to create quizzes and prepare content,
- players to participate in quiz sessions,
- scores to be calculated automatically,
- leaderboards to be updated dynamically after each attempt,
- and multiple attempts per user to support replayability and competition.

The project follows clean architectural separation and emphasizes maintainability, extensibility, and clear responsibilities between layers.

---

## Features

### 👤 Authentication & Roles

- ASP.NET Core Identity integration
- User registration and login
- Role-based access control
- Separate permissions for admins and players
- Secure account handling

### 📝 Quiz Management

- Create new quizzes
- Edit existing quizzes
- Delete quizzes
- Add or remove questions dynamically
- Assign metadata such as title, description, and start time
- Support for quiz-related leaderboard tracking

### ❓ Question Management

- Create questions with content and scoring
- Support for different question types
- Reuse questions across multiple quizzes
- Assign categories to questions
- Attach multiple answers to a question
- Mark correct answers

### 🎮 Gameplay

- Start a quiz attempt
- Answer questions one by one
- Track the current question index
- Automatically calculate score
- Support multiple attempts per user
- Display a summary when the quiz is completed

### 🏆 Leaderboards

- One leaderboard per quiz
- Displays all attempts, not only the best one
- Rankings are generated dynamically based on score
- Accessible from quiz details, game summary, and leaderboard pages
- Keeps ranking data updated as new attempts are completed

### 📊 Game Summary

After completing a quiz attempt, the user can see:
- final score
- maximum possible score
- number of correct answers
- total number of questions
- a direct link to the quiz leaderboard

### 🧩 Clean Application Structure

- Separation of UI, business logic, and data access
- ViewModels used only for presentation
- Services used for application logic
- Database access handled through Entity Framework Core
- Controllers kept thin and focused

---

## How the Application Works

### For Players
1. The user registers or logs in.
2. The user opens a quiz and starts an attempt.
3. Questions are shown one at a time.
4. The user submits answers step by step.
5. The system calculates the score automatically.
6. When the attempt is finished, the user sees a summary.
7. The result is added to the quiz leaderboard.

### For Admins
1. The admin logs in with elevated permissions.
2. The admin creates quizzes and configures quiz details.
3. Questions are added or removed from quizzes.
4. Answers are created and correct answers are marked.
5. Leaderboards update automatically as users play.
6. The admin can review quiz structure and results.

---

## Architecture

The application follows **SOLID principles** and a clean separation of concerns.

### Layers

```text
QuizGame/
├── QuizGame.Web          # Web layer: controllers, views, UI logic
├── QuizGame.Data         # Data access layer
├── QuizGame.Data.Models  # Database and domain models
├── QuizGame.Core         # Business logic and services
├── QuizGame.ViewModels   # Models used by the UI
├── QuizGame.Common       # Shared helpers, constants, utilities
```

### Responsibilities

#### QuizGame.Web
Contains:
- controllers
- Razor views
- route handling
- model binding
- presentation-related logic

#### QuizGame.Data
Contains:
- DbContext
- migrations
- entity configurations
- persistence setup

#### QuizGame.Data.Models
Contains:
- entity classes
- database-facing domain objects
- relationships between quizzes, questions, answers, attempts, and leaderboards

#### QuizGame.Core
Contains:
- business rules
- scoring logic
- quiz attempt handling
- leaderboard calculations
- service implementations

#### QuizGame.ViewModels
Contains:
- UI-specific models
- form input models
- details display models
- summary and leaderboard models

#### QuizGame.Common
Contains:
- shared constants
- utility classes
- reusable helpers

---

## Key Services

### QuizzesService
Responsible for:
- creating quizzes
- updating quiz details
- deleting quizzes
- loading quiz information
- managing question assignments
- integrating quiz data with leaderboard logic

### GameService
Responsible for:
- starting quiz attempts
- progressing through questions
- storing submitted answers
- calculating scores
- finishing attempts
- creating summary data

### LeaderboardsService
Responsible for:
- retrieving leaderboard entries
- ranking participants
- ordering attempts by score
- updating leaderboard-related data
- returning leaderboard views for the UI

---

## Domain Models

## Quiz

### Description
Represents a quiz with its metadata, scheduling details, and related entities.

### Properties
- `Id` – unique identifier of the quiz
- `Title` – required quiz title with maximum length validation
- `Description` – required quiz description with maximum length validation
- `StartTime` – required date and time when the quiz becomes available

### Relationships
- `Leaderboard` – optional association for tracking scores
- `Questions` – collection of questions linked to the quiz

### Notes
- A quiz can exist without a leaderboard initially.
- A quiz may contain multiple questions.
- Questions can be reused depending on the many-to-many configuration.

---

## Question

### Description
Represents a question used in quizzes, including its content, type, scoring, and relationships.

### Properties
- `Id` – unique identifier
- `Content` – required question text
- `QuestionType` – type of question such as multiple choice or true/false
- `Complexity` – difficulty level
- `Points` – score value awarded for a correct answer

### Relationships
- `Quizzes` – quizzes that include this question
- `Categories` – categories associated with the question
- `Answers` – possible answers for the question

### Notes
- Questions may be reused in multiple quizzes.
- Questions can belong to multiple categories.
- A question should normally have at least one answer.

---

## Answer

### Description
Represents a possible answer to a question, including whether it is correct.

### Properties
- `Id` – unique identifier
- `Content` – answer text
- `IsCorrect` – indicates if the answer is correct
- `QuestionId` – foreign key to the related question

### Relationships
- `Question` – the question this answer belongs to

### Notes
- Each answer must belong to one question.
- Questions may have multiple answers.
- At least one answer should typically be marked correct.

---

## QuizAttempt

### Description
Represents a user’s attempt at taking a quiz, tracking progress, submitted answers, and scoring.

### Properties
- `Id` – unique identifier
- `QuizId` – associated quiz
- `UserId` – user taking the quiz
- `CurrentQuestionIndex` – current progress position
- `Score` – current score
- `MaxScore` – maximum possible score
- `IsFinished` – indicates completion status

### Relationships
- `Quiz` – quiz being attempted
- `Answers` – submitted answers during the attempt

### Notes
- A user may have multiple attempts for the same quiz.
- `IsFinished` is set when the last question has been answered.
- `Score` should never exceed `MaxScore`.

---

## Leaderboard

### Description
Represents a leaderboard associated with a quiz and used to display rankings.

### Properties
- `Id` – unique identifier
- `Title` – required leaderboard title
- `Description` – required leaderboard description
- `LastUpdated` – timestamp of last modification
- `QuizId` – foreign key to the related quiz

### Relationships
- `Quiz` – quiz that owns the leaderboard
- `Entries` – leaderboard entries containing user results

### Notes
- Every leaderboard must be linked to a quiz.
- `LastUpdated` should change whenever entries are updated.
- Rankings are calculated from leaderboard entries.

---

## LeaderboardEntry

### Description
Represents one user’s result in a leaderboard.

### Properties
- `Id` – unique identifier
- `UserId` – user associated with the entry
- `Score` – score achieved by the user
- `Rank` – ranking position
- `LeaderboardId` – leaderboard foreign key

### Relationships
- `User` – the application user
- `Leaderboard` – leaderboard containing the entry

### Notes
- Each entry corresponds to one user result within a leaderboard.
- Rankings can be recalculated dynamically when new results are added.
- A leaderboard may contain many entries.

---

## Controllers

| Controller | Responsibility |
|---|---|
| `QuizzesController` | Manage quizzes, quiz details, CRUD operations |
| `PlayController` | Handle gameplay, answer submission, quiz completion |
| `LeaderboardsController` | Show leaderboard rankings and quiz results |
| `Account / Identity` | Handle authentication and user sessions |

### Controller Design Notes
- Controllers stay lightweight.
- Business logic is handled by services.
- Controllers focus on request handling and response generation.
- This improves testability and separation of concerns.

---

## ViewModels

ViewModels are used to pass only the data needed by the UI.

### Common ViewModels
- `CreateQuizViewModel`
- `EditQuizViewModel`
- `DetailsQuizViewModel`
- `GameSummaryViewModel`
- `LeaderboardRowVm`

### ViewModel Rules
- ViewModels are not used for database access
- ViewModels are not entity classes
- ViewModels help keep views simple and strongly typed
- ViewModels prevent unnecessary exposure of internal data models

---

## Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Bootstrap 5
- Razor Views
- LINQ
- C#

---

## Setup Instructions

### 1. Clone the repository

```bash
git clone https://github.com/Ivan-Pudev/Quiz-Game.git
cd Quiz-Game
```

### 2. Configure the database

Update `appsettings.json` with your connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=QuizGameDb;Trusted_Connection=True;"
}
```

Adjust the connection string if your SQL Server configuration is different.

### 3. Apply migrations

```bash
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

### 5. Open the application

Navigate to the local host address shown in the terminal.

---

## Default Roles

The application can use seeded roles such as:

- **Admin**
- **Player**

### Role Behavior
- **Admin** users can manage quizzes, questions, answers, and leaderboard-related content.
- **Player** users can browse and attempt quizzes.
- Authorization rules control access to protected pages and actions.

---

## Design Decisions

- Multiple attempts per user are allowed to encourage replayability.
- Leaderboard rank is calculated dynamically instead of being stored permanently.
- Controllers contain no business logic.
- Views do not directly access the database.
- EF Core tracking is used only where necessary.
- The application separates domain logic from presentation logic.
- ViewModels are used to keep the UI layer clean and safe.

---

## Screenshots

### Play Quiz
![Play Quiz](screenshots/play-quiz.png)

### Game Summary
![Game Summary](screenshots/game-summary.png)

### Leaderboard
![Leaderboard](screenshots/leaderboard.png)

---

## Future Improvements

- Timed questions
- Question category filtering
- Leaderboard pagination
- Answer review after completing a quiz
- Admin dashboard enhancements
- Quiz difficulty levels
- Better analytics for quiz attempts
- Exportable leaderboard results
- Better mobile UI support
- Notifications for quiz completion

---

## Project Info

This project was created for educational and portfolio purposes.

It demonstrates:
- ASP.NET Core MVC application structure
- Entity Framework Core data modeling
- Authentication and authorization
- Service-based architecture
- Clean UI model separation
- Real-world leaderboard and scoring logic

---

## Author

**Ivan Pudev**  
ASP.NET Core MVC Quiz Game Project
