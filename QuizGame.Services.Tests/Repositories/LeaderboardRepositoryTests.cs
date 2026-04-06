using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using QuizGame.Data.Models;
using QuizGame.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuizGame.Data;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace QuizGame.Services.Tests.Repositories
{
    [NonParallelizable]
    [TestFixture]
    public class LeaderboardRepositoryTests
    {
        private SqliteConnection _connection = null!;
        private QuizGameDbContext _dbContext = null!;
        private LeaderboardRepository _repository = null!;

        [SetUp]
        public async Task SetUp()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            await _connection.OpenAsync();

            var options = new DbContextOptionsBuilder<QuizGameDbContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;

            _dbContext = new QuizGameDbContext(options);
            await _dbContext.Database.EnsureCreatedAsync();

            _repository = new LeaderboardRepository(_dbContext);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_dbContext != null)
                await _dbContext.DisposeAsync();

            if (_connection != null)
                await _connection.DisposeAsync();

            _repository?.Dispose();
        }
        [Test]
        public async Task GetLeaderboardWithEntriesAndUserBydAsync_ReturnsLeaderboard_WithEntriesAndUsers()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
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

            leaderboard.Entries = new List<LeaderboardEntry> { entry };

            _dbContext.Quizzes.Add(quiz);
            _dbContext.Users.Add(user);
            _dbContext.Leaderboards.Add(leaderboard);
            _dbContext.LeaderboardEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetLeaderboardWithEntriesAndUserBydAsync(leaderboard.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(leaderboard.Id));
            Assert.That(result.Entries, Is.Not.Null);
            Assert.That(result.Entries.Count, Is.EqualTo(1));
            Assert.That(result.Entries.First().User, Is.Not.Null);
            Assert.That(result.Entries.First().UserId, Is.EqualTo(user.Id));
        }

        [Test]
        public async Task GetLeaderboardWithEntriesAndUserByQuizIdAsync_ReturnsLeaderboard_ByQuizId()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
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

            leaderboard.Entries = new List<LeaderboardEntry> { entry };

            _dbContext.Quizzes.Add(quiz);
            _dbContext.Users.Add(user);
            _dbContext.Leaderboards.Add(leaderboard);
            _dbContext.LeaderboardEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetLeaderboardWithEntriesAndUserByQuizIdAsync(quiz.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.QuizId, Is.EqualTo(quiz.Id));
            Assert.That(result.Entries, Has.Count.EqualTo(1));
            Assert.That(result.Entries.First().User, Is.Not.Null);
        }

        [Test]
        public async Task GetLeaderboardWithEntriesAndUserByIdAsync_ReturnsEntries_OrderedByScoreDescending()
        {
            var user1 = new ApplicationUser() { Id = Guid.NewGuid(), FullName = "1" };
            var user2 = new ApplicationUser() { Id = Guid.NewGuid(), FullName = "2" };
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
            var leaderboard = new Leaderboard
            {
                Id = Guid.NewGuid(),
                QuizId = quiz.Id,
                Quiz = quiz,
                LastUpdated = DateOnly.MinValue,
                Title = "3",
                Description = "4"
            };

            var low = new LeaderboardEntry
            {
                Id = Guid.NewGuid(),
                LeaderboardId = Guid.NewGuid(),
                Leaderboard = leaderboard,
                UserId = Guid.NewGuid(),
                Score = 10,
                IsDeleted = false,
                User = user1
            };

            var high = new LeaderboardEntry
            {
                Id = Guid.NewGuid(),
                LeaderboardId = Guid.NewGuid(),
                Leaderboard = leaderboard,
                UserId = Guid.NewGuid(),
                Score = 99,
                IsDeleted = false,
                User = user2
            };

            _dbContext.Users.AddRange(user1, user2);
            _dbContext.LeaderboardEntries.AddRange(low, high);
            await _dbContext.SaveChangesAsync();

            var result = (await _repository.GetLeaderboardWithEntriesAndUserByIdAsync(leaderboard.Id)).ToList();

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Score, Is.EqualTo(99));
            Assert.That(result[0].User, Is.Not.Null);
            Assert.That(result[1].Score, Is.EqualTo(10));
        }

        [Test]
        public async Task GetLeaderboardsWithQuizzesAsync_ReturnsLeaderboards_OrderedByLastUpdatedDescending()
        {
            var quiz1 = new Quiz { Id = Guid.NewGuid(), Title = "1", Description = "2" };
            var quiz2 = new Quiz { Id = Guid.NewGuid(), Title = "3", Description = "4" };

            var oldLeaderboard = new Leaderboard
            {
                Id = Guid.NewGuid(),
                QuizId = quiz1.Id,
                Quiz = quiz1,
                LastUpdated = new DateOnly(2024, 1, 1),
                Description = "1",
                Title = "2"
            };

            var newLeaderboard = new Leaderboard
            {
                Id = Guid.NewGuid(),
                QuizId = quiz2.Id,
                Quiz = quiz2,
                LastUpdated = new DateOnly(2025, 1, 1),
                Description = "1",
                Title = "2"
            };


            _dbContext.Quizzes.AddRange(quiz1, quiz2);
            _dbContext.Leaderboards.AddRange(oldLeaderboard, newLeaderboard);
            await _dbContext.SaveChangesAsync();

            var result = (await _repository.GetLeaderboardsWithQuizzesAsync()).ToList();

            Assert.That(result, Has.Count.GreaterThan(2));
            Assert.That(result[0].Quiz, Is.Not.Null);
        }

        [Test]
        public async Task GetLeaderboardsWithEntriesByQuizIdAsync_ReturnsLeaderboard_WithEntries()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
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

            leaderboard.Entries = new List<LeaderboardEntry> { entry };

            _dbContext.Quizzes.Add(quiz);
            _dbContext.Leaderboards.Add(leaderboard);
            _dbContext.LeaderboardEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetLeaderboardsWithEntriesByQuizIdAsync(quiz.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Entries, Is.Not.Null);
            Assert.That(result.Entries.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetLeaderboardEntriesOrderedByScoreByLeaderboardIdAsync_ReturnsEntries_OrderedByScoreThenId()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
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

            var sameScoreFirst = new LeaderboardEntry
            {
                Id = Guid.NewGuid(),
                LeaderboardId = Guid.NewGuid(),
                Leaderboard = leaderboard,
                UserId = Guid.NewGuid(),
                Score = 100,
                IsDeleted = false,
                User = user
            };

            var sameScoreSecond = new LeaderboardEntry
            {
                Id = Guid.NewGuid(),
                LeaderboardId = Guid.NewGuid(),
                Leaderboard = leaderboard,
                UserId = Guid.NewGuid(),
                Score = 50,
                IsDeleted = false,
                User = user
            };

            var topScore = new LeaderboardEntry
            {
                Id = Guid.NewGuid(),
                LeaderboardId = Guid.NewGuid(),
                Leaderboard = leaderboard,
                UserId = Guid.NewGuid(),
                Score = 50,
                IsDeleted = false,
                User = user
            };

            _dbContext.Users.Add(user);
            _dbContext.LeaderboardEntries.AddRange(sameScoreSecond, topScore, sameScoreFirst);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetLeaderboardEntriesOrderedByScoreByLeaderboardIdAsync(leaderboard.Id);
            var list = result.ToList();

            Assert.That(list, Has.Count.EqualTo(3));
            Assert.That(list[0].Score, Is.EqualTo(100));
            Assert.That(list[1].Score, Is.EqualTo(50));
            Assert.That(list[2].Score, Is.EqualTo(50));
        }

        [Test]
        public async Task GetLeaderboardEntryForUserByIdAsync_ReturnsMatchingEntry()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
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

            _dbContext.LeaderboardEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetLeaderboardEntryForUserByIdAsync(entry.Id, user.Id);

            Assert.That(result.Id, Is.EqualTo(entry.Id));
            Assert.That(result.UserId, Is.EqualTo(user.Id));
        }

        [Test]
        public async Task GetLeaderboardEntryByIdAsync_ReturnsEntry()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
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

            _dbContext.LeaderboardEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetLeaderboardEntryByIdAsync(entry.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(entry.Id));
        }

        [Test]
        public async Task GetLeaderboardsWithEntriesAsync_ReturnsEntries_WithLeaderboardAndUser()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
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

            _dbContext.Leaderboards.Add(leaderboard);
            _dbContext.Users.Add(user);
            _dbContext.LeaderboardEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            var result = (await _repository.GetLeaderboardsWithEntriesAsync()).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Leaderboard, Is.Not.Null);
            Assert.That(result[0].User, Is.Not.Null);
        }

        [Test]
        public async Task GetLeaderboardsWithEntriesWithQuizAsync_ReturnsEntries_WithLeaderboardUserAndQuiz()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
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

            _dbContext.Quizzes.Add(quiz);
            _dbContext.Leaderboards.Add(leaderboard);
            _dbContext.Users.Add(user);
            _dbContext.LeaderboardEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            var result = (await _repository.GetLeaderboardsWithEntriesWithQuizAsync()).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Leaderboard, Is.Not.Null);
            Assert.That(result[0].User, Is.Not.Null);
            Assert.That(result[0].Leaderboard.Quiz, Is.Not.Null);
        }

        [Test]
        public async Task AddLeaderboardAsync_AddsLeaderboard_AndReturnsTrue()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
            var leaderboard = new Leaderboard
            {
                Id = Guid.NewGuid(),
                QuizId = quiz.Id,
                Quiz = quiz,
                LastUpdated = DateOnly.MinValue,
                Title = "3",
                Description = "4"
            };


            _dbContext.Quizzes.Add(quiz); 
            _dbContext.Leaderboards.Add(leaderboard);

            int result = await _dbContext.SaveChangesAsync();

            Assert.That(result, Is.GreaterThan(1));
        }

        [Test]
        public async Task AddLeaderboardEntryAsync_AddsEntry_AndReturnsTrue()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
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

            var result = await _repository.AddLeaderboardEntryAsync(entry);

            Assert.That(result, Is.True);
            Assert.That(await _dbContext.LeaderboardEntries.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateLeaderboardEntriesAsync_UpdatesEntry_AndReturnsTrue()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
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

            _dbContext.LeaderboardEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            entry.Score = 42;
            var result = await _repository.UpdateLeaderboardEntriesAsync(entry);

            Assert.That(result, Is.True);

            var fromDb = await _dbContext.LeaderboardEntries.FirstAsync(e => e.Id == entry.Id);
            Assert.That(fromDb.Score, Is.EqualTo(42));
        }

        [Test]
        public async Task RestoreEntryAsync_SetsIsDeletedToFalse_AndReturnsTrue()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
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
            _dbContext.LeaderboardEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.RestoreEntryAsync(entry.Id);

            var fromDb = await _dbContext.LeaderboardEntries.FirstAsync(e => e.Id == entry.Id);
            Assert.That(fromDb.IsDeleted, Is.False);
        }

        [Test]
        public async Task RestoreEntryAsync_ReturnsFalse_WhenEntryDoesNotExist()
        {
            var result = await _repository.RestoreEntryAsync(Guid.NewGuid());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task SoftDeleteEntryAsync_SetsIsDeletedToTrue_AndReturnsTrue()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
            var user = new ApplicationUser() { Id = Guid.NewGuid(), FullName = "a" };
            var leaderboard = new Leaderboard
            {
                Id = Guid.NewGuid(),
                QuizId = quiz.Id,
                Quiz = quiz,
                LastUpdated = DateOnly.MinValue,
                Title = "1",
                Description = "2"
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

            _dbContext.LeaderboardEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.SoftDeleteEntryAsync(entry.Id);

            Assert.That(result, Is.True);
            var fromDb = await _dbContext.LeaderboardEntries.FirstAsync(e => e.Id == entry.Id);
            Assert.That(fromDb.IsDeleted, Is.True);
        }

        [Test]
        public async Task SoftDeleteEntryAsync_ReturnsFalse_WhenEntryDoesNotExist()
        {
            var result = await _repository.SoftDeleteEntryAsync(Guid.NewGuid());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HardDeleteEntryAsync_RemovesEntry_AndReturnsTrue()
        {
            var quiz = new Quiz { Id = Guid.NewGuid(), Description = "1", Title = "2" };
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

            _dbContext.LeaderboardEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.HardDeleteEntryAsync(entry.Id);

            Assert.That(result, Is.True);
            Assert.That(await _dbContext.LeaderboardEntries.AnyAsync(e => e.Id == entry.Id), Is.False);
        }

        [Test]
        public async Task HardDeleteEntryAsync_ReturnsFalse_WhenEntryDoesNotExist()
        {
            var result = await _repository.HardDeleteEntryAsync(Guid.NewGuid());

            Assert.That(result, Is.False);
        }
    }
}
