using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using QuizGame.Data;
using QuizGame.Data.Models;
using QuizGame.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuizGame.Services.Tests.Repositories
{
    [TestFixture]
    public class QuizRepositoryTests
    {
        private QuizGameDbContext _dbContext = null!;
        private QuizRepository _repository = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<QuizGameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new QuizGameDbContext(options);
            _repository = new QuizRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _repository.Dispose();
        }

        [Test]
        public async Task GetAllQuestionsOrderByContentAsync_ReturnsQuestionsOrderedByContentAscending()
        {
            var q1 = new Question { Id = Guid.NewGuid(), Content = "Zebra" };
            var q2 = new Question { Id = Guid.NewGuid(), Content = "Apple" };
            var q3 = new Question { Id = Guid.NewGuid(), Content = "Monkey" };

            await _dbContext.Questions.AddRangeAsync(q1, q2, q3);
            await _dbContext.SaveChangesAsync();

            var result = (await _repository.GetAllQuestionsOrderByContentAsync()).ToList();

            Assert.That(result.Select(q => q.Content), Is.EqualTo(new[] { "Apple", "Monkey", "Zebra" }));
        }

        [Test]
        public async Task GetQuestionsFromTheirIdsAsync_ReturnsOnlyMatchingQuestions()
        {
            var q1 = new Question { Id = Guid.NewGuid(), Content = "First" };
            var q2 = new Question { Id = Guid.NewGuid(), Content = "Second" };
            var q3 = new Question { Id = Guid.NewGuid(), Content = "Third" };

            await _dbContext.Questions.AddRangeAsync(q1, q2, q3);
            await _dbContext.SaveChangesAsync();

            var ids = new List<Guid> { q1.Id, q3.Id };

            var result = (await _repository.GetQuestionsFromTheirIdsAsync(ids)).ToList();

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Select(q => q.Id), Is.EquivalentTo(ids));
        }

        [Test]
        public async Task GetAllQuizzesWithQuestionAnswersCategoriesAndLeaderboardAsync_ReturnsQuizWithAllNavigationsLoaded()
        {
            var quiz = SeedQuizWithFullGraph();
            await _dbContext.Quizzes.AddAsync(quiz);
            await _dbContext.SaveChangesAsync();

            var result = (await _repository.GetAllQuizzesWithQuestionAnswersCategoriesAndLeaderboardAsync()).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Questions, Is.Not.Null);
            Assert.That(result[0].Questions.Count, Is.EqualTo(1));
            Assert.That(result[0].Questions.First().Answers, Is.Not.Null);
            Assert.That(result[0].Questions.First().Answers.Count, Is.EqualTo(2));
            Assert.That(result[0].Questions.First().Categories, Is.Not.Null);
            Assert.That(result[0].Questions.First().Categories.Count, Is.EqualTo(1));
            Assert.That(result[0].Leaderboard, Is.Not.Null);
        }

        [Test]
        public async Task GetAllDeletedQuizzesAsync_ReturnsOnlyDeletedQuizzes()
        {
            var deletedQuiz = SeedQuizWithFullGraph();
            deletedQuiz.IsDeleted = true;

            var activeQuiz = SeedQuizWithFullGraph();
            activeQuiz.Id = Guid.NewGuid();
            activeQuiz.IsDeleted = false;

            await _dbContext.Quizzes.AddRangeAsync(deletedQuiz, activeQuiz);
            await _dbContext.SaveChangesAsync();

            var result = (await _repository.GetAllDeletedQuizzesAsync()).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.Single().IsDeleted, Is.True);
            Assert.That(result.Single().Id, Is.EqualTo(deletedQuiz.Id));
        }

        [Test]
        public async Task GetQuizWithQuestionsAnswersCategoriesAndLeaderboardByIdAsync_ReturnsMatchingQuiz()
        {
            var quiz = SeedQuizWithFullGraph();
            await _dbContext.Quizzes.AddAsync(quiz);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetQuizWithQuestionsAnswersCategoriesAndLeaderboardByIdAsync(quiz.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(quiz.Id));
            Assert.That(result.Questions, Is.Not.Null);
            Assert.That(result.Questions.Count, Is.EqualTo(1));
            Assert.That(result.Questions.First().Answers.Count, Is.EqualTo(2));
            Assert.That(result.Questions.First().Categories.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetQuizWithQuestionsByIdAsync_ReturnsQuizWithQuestions()
        {
            var quiz = new Quiz
            {
                Id = Guid.NewGuid(),
                Title = "Quiz 1",
                Questions = new List<Question>
                {
                    new Question { Id = Guid.NewGuid(), Content = "Question 1" }
                },
                Description = "1"
            };

            await _dbContext.Quizzes.AddAsync(quiz);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetQuizWithQuestionsByIdAsync(quiz.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Questions, Is.Not.Null);
            Assert.That(result.Questions.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task AddQuizAsync_AddsQuizAndReturnsTrue()
        {
            
            var quiz = new Quiz
            {
                Id = Guid.NewGuid(),
                Title = "New quiz",
                Description = "1",
            };

            var result = await _repository.AddQuizAsync(quiz);

            Assert.That(result, Is.True);
            Assert.That(await _dbContext.Quizzes.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateQuizAsync_UpdatesQuizAndReturnsTrue()
        {
            var quiz = new Quiz
            {
                Id = Guid.NewGuid(),
                Title = "Old title",
                Description = "1"
            };

            await _dbContext.Quizzes.AddAsync(quiz);
            await _dbContext.SaveChangesAsync();

            quiz.Title = "Updated title";

            var result = await _repository.UpdateQuizAsync(quiz);

            Assert.That(result, Is.True);

            var updatedQuiz = await _dbContext.Quizzes.FirstAsync(q => q.Id == quiz.Id);
            Assert.That(updatedQuiz.Title, Is.EqualTo("Updated title"));
        }

        [Test]
        public async Task RestoreQuizAsync_SetsIsDeletedToFalseAndReturnsTrue()
        {
            var quiz = new Quiz
            {
                Id = Guid.NewGuid(),
                Title = "Deleted quiz",
                IsDeleted = true,
                Description = "1"
            };

            await _dbContext.Quizzes.AddAsync(quiz);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.RestoreQuizAsync(quiz);

            Assert.That(result, Is.True);
            Assert.That(quiz.IsDeleted, Is.False);
        }

        [Test]
        public async Task SoftDeleteQuizAsync_SetsIsDeletedToTrueAndReturnsTrue()
        {
            var quiz = new Quiz
            {
                Id = Guid.NewGuid(),
                Title = "Active quiz",
                IsDeleted = false,
                Description = "1"
            };

            await _dbContext.Quizzes.AddAsync(quiz);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.SoftDeleteQuizAsync(quiz);

            Assert.That(result, Is.True);
            Assert.That(quiz.IsDeleted, Is.True);
        }

        [Test]
        public async Task HardDeleteQuizAsync_RemovesQuizAndReturnsTrue()
        {
            var quiz = new Quiz
            {
                Id = Guid.NewGuid(),
                Title = "Quiz to delete",
                Description = "1"
            };

            await _dbContext.Quizzes.AddAsync(quiz);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.HardDeleteQuizAsync(quiz);

            Assert.That(result, Is.True);
            Assert.That(await _dbContext.Quizzes.CountAsync(), Is.EqualTo(0));
        }

        private static Quiz SeedQuizWithFullGraph()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2", };
            var user = new ApplicationUser() { Id = Guid.NewGuid(), FullName = "a" };
            var leaderboard = new Leaderboard
            {
                Id = Guid.NewGuid(),
                QuizId = quiz.Id,
                Quiz = quiz,
                LastUpdated = DateOnly.MinValue,
                Title = "3",
                Description = "4"
            };

            var entry = new LeaderboardEntry
            {
                Id = Guid.NewGuid(),
                LeaderboardId = Guid.NewGuid(),
                Leaderboard = leaderboard,
                UserId = Guid.NewGuid(),
                Score = 5,
                IsDeleted = false,
                User = user
            };

            var question = new Question
            {
                Id = Guid.NewGuid(),
                Content = "What is 2 + 2?",
                Answers = new List<Answer>
                {
                    new Answer { Id = Guid.NewGuid(), Content = "3" },
                    new Answer { Id = Guid.NewGuid(), Content = "4" }
                },
                Categories = new List<Category>
                {
                    new Category { Id = Guid.NewGuid(), Name = "Math" }
                }
            };
            
            return new Quiz
            {
                Id = Guid.NewGuid(),
                Title = "Sample quiz",
                Questions = new List<Question> { question },
                Leaderboard = leaderboard,
                Description = "1"
            };
        }
    }
}
