using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using QuizGame.Data;
using QuizGame.Data.Models;
using QuizGame.Data.Models.Enums;
using QuizGame.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizGame.Services.Tests.Repositories
{
    [TestFixture]
    public class GameRepositoryTests
    {
        private SqliteConnection _connection = null!;
        private DbContextOptions<QuizGameDbContext> _options = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<QuizGameDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new QuizGameDbContext(_options);
            context.Database.EnsureCreated();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _connection.Close();
            _connection.Dispose();
        }

        private QuizGameDbContext CreateContext()
        {
            var context = new QuizGameDbContext(_options);
            context.Database.EnsureCreated();
            return context;
        }

        [Test]
        public async Task GetQuizAttemptWithQuizAndAnswersByIdAsync_Returns_Attempt_With_Quiz_And_Answers()
        {
            var attemptId = Guid.NewGuid();
            var quizId = Guid.NewGuid();

            using (var context = CreateContext())
            {
                // 🔥 Required dependencies

                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    FullName = "test",
                    NormalizedUserName = "TEST"
                };

                var question = new Question
                {
                    Content = "Question",
                    QuestionType = QuestionType.SingleChoice,
                    Points = 10
                };

                var answer = new Answer
                {
                    Content = "Answer",
                    Question = question
                };

                var quiz = new Quiz
                {
                    Id = quizId,
                    Title = "Quiz",
                    Description = "Desc",
                    StartTime = DateTime.UtcNow
                };

                var attempt = new QuizAttempt
                {
                    Id = attemptId,
                    Quiz = quiz,
                    User = user
                };

                // 🔥 VALID AttemptAnswers
                var attemptAnswers = new List<AttemptAnswer>
        {
            new AttemptAnswer
            {
                QuizAttempt = attempt,
                Question = question,
                Answer = answer
            },
            new AttemptAnswer
            {
                QuizAttempt = attempt,
                Question = question,
                Answer = answer
            }
        };

                context.AddRange(user, question, answer, quiz, attempt);
                context.AttemptAnswers.AddRange(attemptAnswers);

                await context.SaveChangesAsync();
            }

            using var readContext = CreateContext();
            var repository = new GameRepository(readContext);

            var result = await repository.GetQuizAttemptWithQuizAndAnswersByIdAsync(attemptId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(attemptId));
            Assert.That(result.Quiz, Is.Not.Null);
            Assert.That(result.Quiz.Id, Is.EqualTo(quizId));
            Assert.That(result.Answers, Is.Not.Null);
            Assert.That(result.Answers.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetQuizAttemptWithQuizQuestionsAndAnswersByIdAsync_Returns_Attempt_With_Quiz_Questions_And_Answers()
        {
            var attemptId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = "testuser"
            };

            var questionId = Guid.NewGuid();
            var quizId = Guid.NewGuid();
            using (var context = CreateContext())
            {
                var quiz = new Quiz
                {
                    Id = quizId,
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            Id = questionId,
                            Answers = new List<Answer>
                            {
                                new Answer {  Id = Guid.NewGuid(),
                                    Content = "Test answer1",
                                    IsCorrect = true, },
                                new Answer {  Id = Guid.NewGuid(),
                                    Content = "Test answer2",
                                    IsCorrect = false, }
                            },
                            Content = "12324",
                            Points = 10
                        }
                    },
                    Description = "1",
                    Title = "2"
                };

                var attempt = new QuizAttempt
                {
                    Id = attemptId,
                    Quiz = quiz,
                    Answers = new List<AttemptAnswer>(),
                    User = user
                };

                context.QuizAttempts.Add(attempt);
                await context.SaveChangesAsync();
            }

            using var readContext = CreateContext();
            var repository = new GameRepository(readContext);

            var result = await repository.GetQuizAttemptWithQuizQuestionsAndAnswersByIdAsync(attemptId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(attemptId));
            Assert.That(result.Quiz, Is.Not.Null);
            Assert.That(result.Quiz.Id, Is.EqualTo(quizId));
            Assert.That(result.Quiz.Questions, Is.Not.Null);
            Assert.That(result.Quiz.Questions.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddQuizAttemptAsync_Should_Save_Attempt_And_Return_True()
        {
            using var context = CreateContext();
            var repository = new GameRepository(context);

            // ✅ Create valid user
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = "testuser",              // 🔥 REQUIRED for Identity
                NormalizedUserName = "TESTUSER"     // 🔥 IMPORTANT
            };

            var quiz = new Quiz
            {
                Id = Guid.NewGuid(),
                Title = "Test Quiz",
                Description = "Test Description",
                StartTime = DateTime.UtcNow
            };

            // 🔥 Save dependencies FIRST
            context.Users.Add(user);
            context.Quizzes.Add(quiz);
            await context.SaveChangesAsync();

            var attempt = new QuizAttempt
            {
                Id = Guid.NewGuid(),
                QuizId = quiz.Id,   // ✅ use FK directly (safer)
                UserId = user.Id
            };

            var result = await repository.AddQuizAttemptAsync(attempt);

            Assert.That(result, Is.True);

            var savedAttempt = await context.QuizAttempts
                .FirstOrDefaultAsync(x => x.Id == attempt.Id);

            Assert.That(savedAttempt, Is.Not.Null);
        }

        [Test]
        public async Task AddAttemptAnswerAsync_Should_Save_Answer_And_Return_True()
        {
            using var context = CreateContext();

            // 🔥 1. Create User (REQUIRED)
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = "testuser"
            };

            // 🔥 2. Create Question
            var question = new Question
            {
                Id = Guid.NewGuid(),
                Content = "Test question",
                QuestionType = QuestionType.SingleChoice,
                Points = 10
            };

            // 🔥 3. Create Answer (linked to Question)
            var answer = new Answer
            {
                Id = Guid.NewGuid(),
                Content = "Test answer",
                IsCorrect = true,
                Question = question
            };

            // 🔥 4. Create Quiz
            var quiz = new Quiz
            {
                Id = Guid.NewGuid(),
                Title = "Test Quiz",
                Description = "Test Description",
                StartTime = DateTime.UtcNow
            };

            // 🔥 5. Create Attempt (REQUIRES User + Quiz)
            var attempt = new QuizAttempt
            {
                Id = Guid.NewGuid(),
                Quiz = quiz,
                User = user
            };

            context.AddRange(user, question, answer, quiz, attempt);
            await context.SaveChangesAsync();

            var repository = new GameRepository(context);

            // 🔥 6. Create AttemptAnswer (ALL FKs satisfied)
            var attemptAnswer = new AttemptAnswer
            {
                Id = Guid.NewGuid(),
                QuizAttemptId = attempt.Id,
                QuestionId = question.Id,
                SelectedAnswerId = answer.Id,
                IsCorrect = true,
                EarnedPoints = 10
            };

            var result = await repository.AddAttemptAnswerAsync(attemptAnswer);

            Assert.That(result, Is.True);

            var saved = await context.AttemptAnswers.FindAsync(attemptAnswer.Id);
            Assert.That(saved, Is.Not.Null);
        }

        [Test]
        public async Task UpdateAttempAnswersAsync_Should_Update_Answer_And_Return_True()
        {
            using var context = CreateContext();

            // 🔥 1. Create full valid graph

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = "testuser",
            };

            var question = new Question
            {
                Content = "Question",
                QuestionType = QuestionType.SingleChoice,
                Points = 10
            };

            var answer = new Answer
            {
                Content = "Answer",
                Question = question
            };

            var quiz = new Quiz
            {
                Title = "Quiz",
                Description = "Desc",
                StartTime = DateTime.UtcNow
            };

            var attempt = new QuizAttempt
            {
                Quiz = quiz,
                User = user
            };

            var attemptAnswer = new AttemptAnswer
            {
                QuizAttempt = attempt,
                Question = question,
                Answer = answer,
                IsCorrect = false,
                EarnedPoints = 0
            };

            context.AddRange(user, question, answer, quiz, attempt, attemptAnswer);
            await context.SaveChangesAsync();

            // 🔥 2. NEW CONTEXT (important!)
            using var updateContext = CreateContext();
            var repository = new GameRepository(updateContext);

            var existing = await updateContext.AttemptAnswers.FirstAsync();

            // 🔥 3. Modify ONLY scalar field
            existing.EarnedPoints = 50;

            var result = await repository.UpdateAttempAnswersAsync(existing);

            Assert.That(result, Is.True);

            var updated = await updateContext.AttemptAnswers.FirstAsync();
            Assert.That(updated.EarnedPoints, Is.EqualTo(50));
        }
    }
}