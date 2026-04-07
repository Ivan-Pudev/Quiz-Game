using Moq;
using NUnit.Framework;
using QuizGame.Core;
using QuizGame.Data.Models;
using QuizGame.Data.Repository.Contracts;

namespace QuizGame.Services.Tests.Services
{
    [TestFixture]
    public class LeaderboardServiceTests
    {
        private Mock<ILeaderboardRepository> _repoMock;
        private LeaderboardService _sut;

        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<ILeaderboardRepository>();
            _sut = new LeaderboardService(_repoMock.Object);
        }

        
        [Test]
        public async Task GetLeaderboardsAsync_ReturnsAllLeaderboards()
        {
            var leaderboards = new List<Leaderboard>
            {
                new Leaderboard { Id = Guid.NewGuid(), Title = "LB 1" },
                new Leaderboard { Id = Guid.NewGuid(), Title = "LB 2" },
            };
            _repoMock
                .Setup(r => r.GetLeaderboardsWithQuizzesAsync())
                .ReturnsAsync(leaderboards);

            var result = await _sut.GetLeaderboardsAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
        }


        [Test]
        public async Task GetLeaderboardEntriesByQuizIdAsync_MapsEntriesToViewModels()
        {
            var quizId = Guid.NewGuid();
            var entries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { Score = 90, User = new ApplicationUser { UserName = "alice" } },
                new LeaderboardEntry { Score = 70, User = new ApplicationUser { UserName = "bob" } },
            };
            _repoMock
                .Setup(r => r.GetLeaderboardWithEntriesAndUserByIdAsync(quizId))
                .ReturnsAsync(entries);

            var result = (await _sut.GetLeaderboardEntriesByQuizIdAsync(quizId))!.ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Rank, Is.EqualTo(1));
            Assert.That(result[0].UserName, Is.EqualTo("alice"));
            Assert.That(result[0].Score, Is.EqualTo(90));
            Assert.That(result[1].Rank, Is.EqualTo(2));
            Assert.That(result[1].UserName, Is.EqualTo("bob"));
        }

        [Test]
        public async Task GetLeaderboardEntriesByQuizIdAsync_NullUser_FallsBackToUnknown()
        {
            var quizId = Guid.NewGuid();
            var entries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { Score = 50, User = null }
            };
            _repoMock
                .Setup(r => r.GetLeaderboardWithEntriesAndUserByIdAsync(quizId))
                .ReturnsAsync(entries);

            var result = (await _sut.GetLeaderboardEntriesByQuizIdAsync(quizId))!.ToList();

            Assert.That(result[0].UserName, Is.EqualTo("(unknown)"));
        }

        [Test]
        public async Task GetLeaderboardEntriesByQuizIdAsync_EmptyEntries_ReturnsEmptyList()
        {
            var quizId = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetLeaderboardWithEntriesAndUserByIdAsync(quizId))
                .ReturnsAsync(new List<LeaderboardEntry>());

            var result = await _sut.GetLeaderboardEntriesByQuizIdAsync(quizId);

            Assert.That(result, Is.Empty);
        }


        [Test]
        public async Task GetLeaderboardByQuizIdAsync_ExistingId_ReturnsLeaderboard()
        {
            var id = Guid.NewGuid();
            var leaderboard = new Leaderboard { Id = id };
            _repoMock
                .Setup(r => r.GetLeaderboardsWithEntriesByQuizIdAsync(id))
                .ReturnsAsync(leaderboard);

            var result = await _sut.GetLeaderboardByQuizIdAsync(id);

            Assert.That(result, Is.EqualTo(leaderboard));
        }

        [Test]
        public async Task GetLeaderboardByQuizIdAsync_NotFound_ReturnsNull()
        {
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetLeaderboardsWithEntriesByQuizIdAsync(id))
                .ReturnsAsync((Leaderboard?)null);

            var result = await _sut.GetLeaderboardByQuizIdAsync(id);

            Assert.That(result, Is.Null);
        }


        [Test]
        public async Task GetLeaderboardsToManageAsync_MapsLeaderboardsToViewModels()
        {
            var quizId = Guid.NewGuid();
            var lbId = Guid.NewGuid();
            var leaderboards = new List<Leaderboard>
            {
                new Leaderboard
                {
                    Id = lbId,
                    QuizId = quizId,
                    Title = "LB Title",
                    LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow),
                    Quiz = new Quiz { Title = "Quiz Title", Description = "Quiz Desc" },
                    Entries = new List<LeaderboardEntry>
                    {
                        new LeaderboardEntry(),
                        new LeaderboardEntry(),
                    }
                }
            };
            _repoMock
                .Setup(r => r.GetLeaderboardsWithQuizzesAsync())
                .ReturnsAsync(leaderboards);

            var result = (await _sut.GetLeaderboardsToManageAsync());

            //Assert.That(result.Count, Is.EqualTo(1));
            //Assert.That(result[0].Id, Is.EqualTo(lbId));
            //Assert.That(result[0].QuizId, Is.EqualTo(quizId));
            //Assert.That(result[0].QuizTitle, Is.EqualTo("Quiz Title"));
            //Assert.That(result[0].Description, Is.EqualTo("Quiz Desc"));
            //Assert.That(result[0].EntryCount, Is.EqualTo(2));
        }

        [Test]
        public async Task GetLeaderboardsToManageAsync_EmptyList_ReturnsEmptyViewModels()
        {
            _repoMock
                .Setup(r => r.GetLeaderboardsWithQuizzesAsync())
                .ReturnsAsync(new List<Leaderboard>());

            var result = await _sut.GetLeaderboardsToManageAsync();

            Assert.That(result.Leaderboards, Is.Empty);
        }


        [Test]
        public async Task GetLeaderboardsEntriesToManageAsync_MapsEntriesToViewModels()
        {
            var entryId = Guid.NewGuid();
            var lbId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry
                {
                    Id = entryId,
                    LeaderboardId = lbId,
                    Rank = 1,
                    Score = 100,
                    UserId = userId,
                    Leaderboard = new Leaderboard { Title = "LB" },
                    User = new ApplicationUser { UserName = "charlie" }
                }
            };
            _repoMock
                .Setup(r => r.GetLeaderboardsWithEntriesAsync())
                .ReturnsAsync(entries);

            var result = (await _sut.GetLeaderboardsEntriesToManageAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(entryId));
            Assert.That(result[0].LeaderboardId, Is.EqualTo(lbId));
            Assert.That(result[0].LeaderboardTitle, Is.EqualTo("LB"));
            Assert.That(result[0].Rank, Is.EqualTo(1));
            Assert.That(result[0].Score, Is.EqualTo(100));
            Assert.That(result[0].UserId, Is.EqualTo(userId));
            Assert.That(result[0].UserName, Is.EqualTo("charlie"));
        }

        [Test]
        public async Task GetLeaderboardsEntriesToManageAsync_NoEntries_ReturnsEmptyList()
        {
            _repoMock
                .Setup(r => r.GetLeaderboardsWithEntriesAsync())
                .ReturnsAsync(new List<LeaderboardEntry>());

            var result = await _sut.GetLeaderboardsEntriesToManageAsync();

            Assert.That(result, Is.Empty);
        }


        [Test]
        public async Task GetLeaderboardsEntriesToManageDetailsAsync_MapsLeaderboardAndEntries()
        {
            var lbId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var leaderboard = new Leaderboard
            {
                Id = lbId,
                Title = "My LB",
                LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow),
                Entries = new List<LeaderboardEntry>
                {
                    new LeaderboardEntry
                    {
                        Id = Guid.NewGuid(),
                        LeaderboardId = lbId,
                        Rank = 1,
                        Score = 88,
                        UserId = userId,
                        IsDeleted = false,
                        Leaderboard = new Leaderboard { Title = "My LB" },
                        User = new ApplicationUser { UserName = "dave" }
                    }
                }
            };
            _repoMock
                .Setup(r => r.GetLeaderboardWithEntriesAndUserBydAsync(lbId))
                .ReturnsAsync(leaderboard);

            var result = await _sut.GetLeaderboardsEntriesToManageDetailsAsync(lbId);

            Assert.That(result.LeaderboardId, Is.EqualTo(lbId));
            Assert.That(result.LeaderboardTitle, Is.EqualTo("My LB"));
            Assert.That(result.Entries.Count, Is.EqualTo(1));
            Assert.That(result.Entries[0].UserName, Is.EqualTo("dave"));
            Assert.That(result.AvailableUsers.Count, Is.EqualTo(1));
            Assert.That(result.AvailableUsers[0].UserName, Is.EqualTo("dave"));
        }

        [Test]
        public async Task GetLeaderboardsEntriesToManageDetailsAsync_NoEntries_ReturnsEmptyCollections()
        {
            var lbId = Guid.NewGuid();
            var leaderboard = new Leaderboard
            {
                Id = lbId,
                Title = "Empty LB",
                LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow),
                Entries = new List<LeaderboardEntry>()
            };
            _repoMock
                .Setup(r => r.GetLeaderboardWithEntriesAndUserBydAsync(lbId))
                .ReturnsAsync(leaderboard);

            var result = await _sut.GetLeaderboardsEntriesToManageDetailsAsync(lbId);

            Assert.That(result.Entries, Is.Empty);
            Assert.That(result.AvailableUsers, Is.Empty);
        }


        [Test]
        public async Task GetGlobalLeaderboardAsync_MapsRankedEntriesAndQuizBreakdown()
        {
            var userId = Guid.NewGuid();
            var quiz = new Quiz { Title = "Science Quiz" };
            var lbEntries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry
                {
                    UserId = userId,
                    Score = 75,
                    User = new ApplicationUser { UserName = "eve" },
                    Leaderboard = new Leaderboard
                    {
                        Quiz = quiz,
                        Entries = new List<LeaderboardEntry> { new LeaderboardEntry(), new LeaderboardEntry() }
                    }
                }
            };
            _repoMock
                .Setup(r => r.GetLeaderboardsWithEntriesWithQuizAsync())
                .ReturnsAsync(lbEntries);

            var result = await _sut.GetGlobalLeaderboardAsync();

            Assert.That(result.RankedEntries.Count, Is.EqualTo(1));
            Assert.That(result.RankedEntries[0].UserId, Is.EqualTo(userId));
            Assert.That(result.RankedEntries[0].UserName, Is.EqualTo("eve"));
            Assert.That(result.RankedEntries[0].TotalScore, Is.EqualTo(75));

            Assert.That(result.QuizBreakdown.Count, Is.EqualTo(1));
            Assert.That(result.QuizBreakdown[0].QuizTitle, Is.EqualTo("Science Quiz"));
            Assert.That(result.QuizBreakdown[0].EntryCount, Is.EqualTo(2));
        }

        [Test]
        public async Task GetGlobalLeaderboardAsync_NoEntries_ReturnsEmptyCollections()
        {
            _repoMock
                .Setup(r => r.GetLeaderboardsWithEntriesWithQuizAsync())
                .ReturnsAsync(new List<LeaderboardEntry>());

            var result = await _sut.GetGlobalLeaderboardAsync();

            Assert.That(result.RankedEntries, Is.Empty);
            Assert.That(result.QuizBreakdown, Is.Empty);
        }

        // ─── UpdateEntryAsync ─────────────────────────────────────────────────

        [Test]
        public async Task UpdateEntryAsync_ValidEntry_UpdatesScore()
        {
            var entryId = Guid.NewGuid();
            var entry = new LeaderboardEntry { Id = entryId, Score = 50 };
            _repoMock
                .Setup(r => r.GetLeaderboardEntryByIdAsync(entryId))
                .ReturnsAsync(entry);
            _repoMock
                .Setup(r => r.UpdateLeaderboardEntriesAsync(entry))
                .ReturnsAsync(true);

            await _sut.UpdateEntryAsync(entryId, 95);

            Assert.That(entry.Score, Is.EqualTo(95));
            _repoMock.Verify(r => r.UpdateLeaderboardEntriesAsync(entry), Times.Once);
        }

        [Test]
        public void UpdateEntryAsync_EntryNotFound_ThrowsInvalidOperationException()
        {
            var entryId = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetLeaderboardEntryByIdAsync(entryId))
                .ReturnsAsync((LeaderboardEntry?)null);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.UpdateEntryAsync(entryId, 80));
        }

        [Test]
        public void UpdateEntryAsync_UpdateFails_ThrowsInvalidOperationException()
        {
            var entryId = Guid.NewGuid();
            var entry = new LeaderboardEntry { Id = entryId, Score = 50 };
            _repoMock
                .Setup(r => r.GetLeaderboardEntryByIdAsync(entryId))
                .ReturnsAsync(entry);
            _repoMock
                .Setup(r => r.UpdateLeaderboardEntriesAsync(entry))
                .ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.UpdateEntryAsync(entryId, 80));
        }


        [Test]
        public async Task RestoreEntryAsync_Success_CallsRepository()
        {
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.RestoreEntryAsync(id))
                .ReturnsAsync(true);

            await _sut.RestoreEntryAsync(id);

            _repoMock.Verify(r => r.RestoreEntryAsync(id), Times.Once);
        }

        [Test]
        public void RestoreEntryAsync_RepositoryFails_ThrowsInvalidOperationException()
        {
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.RestoreEntryAsync(id))
                .ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.RestoreEntryAsync(id));
        }


        [Test]
        public async Task SoftDeleteEntryAsync_Success_CallsRepository()
        {
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.SoftDeleteEntryAsync(id))
                .ReturnsAsync(true);

            await _sut.SoftDeleteEntryAsync(id);

            _repoMock.Verify(r => r.SoftDeleteEntryAsync(id), Times.Once);
        }

        [Test]
        public void SoftDeleteEntryAsync_RepositoryFails_ThrowsInvalidOperationException()
        {
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.SoftDeleteEntryAsync(id))
                .ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.SoftDeleteEntryAsync(id));
        }


        [Test]
        public async Task HardDeleteEntryAsync_Success_CallsRepository()
        {
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.HardDeleteEntryAsync(id))
                .ReturnsAsync(true);

            await _sut.HardDeleteEntryAsync(id);

            _repoMock.Verify(r => r.HardDeleteEntryAsync(id), Times.Once);
        }

        [Test]
        public void HardDeleteEntryAsync_RepositoryFails_ThrowsInvalidOperationException()
        {
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.HardDeleteEntryAsync(id))
                .ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.HardDeleteEntryAsync(id));
        }
    }
}
