using Moq;
using NUnit.Framework;
using QuizGame.Core;
using QuizGame.Data.Models;
using QuizGame.Data.Repository.Contracts;
using QuizGame.ViewModels.Quizzes;

namespace QuizGame.Services.Tests.Services
{
    [TestFixture]
    public class QuizServiceTests
    {
        private Mock<IQuizRepository> _quizRepoMock;
        private Mock<ILeaderboardRepository> _leaderboardRepoMock;
        private QuizService _sut;

        [SetUp]
        public void SetUp()
        {
            _quizRepoMock = new Mock<IQuizRepository>();
            _leaderboardRepoMock = new Mock<ILeaderboardRepository>();
            _sut = new QuizService(_quizRepoMock.Object, _leaderboardRepoMock.Object);
        }


        [Test]
        public async Task GetQuizByIdAsync_ExistingId_ReturnsQuiz()
        {
            var id = Guid.NewGuid();
            var expected = new Quiz { Id = id, Title = "Test Quiz" };
            _quizRepoMock
                .Setup(r => r.GetQuizWithQuestionsAnswersCategoriesAndLeaderboardByIdAsync(id))
                .ReturnsAsync(expected);

            var result = await _sut.GetQuizByIdAsync(id);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetQuizByIdAsync_NonExistingId_ReturnsNull()
        {
            var id = Guid.NewGuid();
            _quizRepoMock
                .Setup(r => r.GetQuizWithQuestionsAnswersCategoriesAndLeaderboardByIdAsync(id))
                .ReturnsAsync((Quiz?)null);

            var result = await _sut.GetQuizByIdAsync(id);

            Assert.That(result, Is.Null);
        }


        [Test]
        public async Task GetAllQuizzesAsync_ReturnsAllQuizzes()
        {
            var quizzes = new List<Quiz>
            {
                new Quiz { Id = Guid.NewGuid(), Title = "Quiz 1" },
                new Quiz { Id = Guid.NewGuid(), Title = "Quiz 2" },
            };
            _quizRepoMock
                .Setup(r => r.GetAllQuizzesWithQuestionAnswersCategoriesAndLeaderboardAsync())
                .ReturnsAsync(quizzes);

            var result = await _sut.GetAllQuizzesAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
        }


        [Test]
        public async Task GetAllDeletedQuizzesAsync_MapsToViewModelsCorrectly()
        {
            var quizId = Guid.NewGuid();
            var deletedQuiz = new Quiz
            {
                Id = quizId,
                Title = "Deleted",
                Description = "Desc",
                IsDeleted = true,
                StartTime = new DateTime(2024, 1, 1),
                Questions = new List<Question>()
            };
            _quizRepoMock
                .Setup(r => r.GetAllDeletedQuizzesAsync())
                .ReturnsAsync(new List<Quiz> { deletedQuiz });

            var result = (await _sut.GetAllDeletedQuizzesAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(quizId));
            Assert.That(result[0].Title, Is.EqualTo("Deleted"));
            Assert.That(result[0].IsDeleted, Is.True);
        }

        [Test]
        public async Task GetAllDeletedQuizzesAsync_NoDeletedQuizzes_ReturnsEmptyList()
        {
            _quizRepoMock
                .Setup(r => r.GetAllDeletedQuizzesAsync())
                .ReturnsAsync(new List<Quiz>());

            var result = await _sut.GetAllDeletedQuizzesAsync();

            Assert.That(result, Is.Empty);
        }


        [Test]
        public async Task GetAllQuestionsAsync_ReturnsQuestionsFromRepository()
        {
            var questions = new List<Question>
            {
                new Question { Id = Guid.NewGuid(), Content = "Q1" },
                new Question { Id = Guid.NewGuid(), Content = "Q2" },
            };
            _quizRepoMock
                .Setup(r => r.GetAllQuestionsOrderByContentAsync())
                .ReturnsAsync(questions);

            var result = await _sut.GetAllQuestionsAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
        }


        [Test]
        public async Task CreateQuizFormAsync_ReturnsViewModelWithAllQuestions()
        {
            var questions = new List<Question>
            {
                new Question { Id = Guid.NewGuid(), Content = "Alpha" },
                new Question { Id = Guid.NewGuid(), Content = "Beta" },
            };
            _quizRepoMock
                .Setup(r => r.GetAllQuestionsOrderByContentAsync())
                .ReturnsAsync(questions);

            var result = await _sut.CreateQuizFormAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Questions.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task CreateQuizAsync_WithSelectedQuestions_AddsQuizAndCreatesLeaderboard()
        {
            var questionId = Guid.NewGuid();
            var viewModel = new CreateQuizViewModel
            {
                Title = "New Quiz",
                Description = "Desc",
                StartTime = DateTime.UtcNow,
                SelectedQuestionIds = new List<Guid> { questionId }
            };

            _quizRepoMock
                .Setup(r => r.GetQuestionsFromTheirIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<Question> { new Question { Id = questionId } });
            _quizRepoMock
                .Setup(r => r.AddQuizAsync(It.IsAny<Quiz>()))
                .ReturnsAsync(true);
            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardsWithEntriesByQuizIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Leaderboard?)null);
            _quizRepoMock
                .Setup(r => r.GetQuizWithQuestionsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Quiz { Title = "New Quiz", Description = "Desc" });
            _leaderboardRepoMock
                .Setup(r => r.AddLeaderboardAsync(It.IsAny<Leaderboard>()))
                .ReturnsAsync(true);

            await _sut.CreateQuizAsync(viewModel);

            _quizRepoMock.Verify(r => r.AddQuizAsync(It.IsAny<Quiz>()), Times.Once);
            _leaderboardRepoMock.Verify(r => r.AddLeaderboardAsync(It.IsAny<Leaderboard>()), Times.Once);
        }

        [Test]
        public async Task CreateQuizAsync_AddQuizFails_ThrowsInvalidOperationException()
        {
            var viewModel = new CreateQuizViewModel
            {
                Title = "Fail Quiz",
                Description = "Desc",
                StartTime = DateTime.UtcNow,
                SelectedQuestionIds = new List<Guid> { Guid.NewGuid() }
            };

            _quizRepoMock
                .Setup(r => r.GetQuestionsFromTheirIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new List<Question> { new Question { Id = Guid.NewGuid() } });
            _quizRepoMock
                .Setup(r => r.AddQuizAsync(It.IsAny<Quiz>()))
                .ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.CreateQuizAsync(viewModel));
        }


        [Test]
        public void ShowQuizDetails_MapsAllFieldsToViewModel()
        {
            var id = Guid.NewGuid();
            var quiz = new Quiz
            {
                Id = id,
                Title = "Details Quiz",
                Description = "A description",
                StartTime = new DateTime(2025, 6, 1),
                IsDeleted = false,
                Questions = new List<Question> { new Question { Content = "Q?" } }
            };

            var result = _sut.ShowQuizDetails(quiz);

            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Title, Is.EqualTo("Details Quiz"));
            Assert.That(result.Description, Is.EqualTo("A description"));
            Assert.That(result.IsDeleted, Is.False);
            Assert.That(result.Questions.Count(), Is.EqualTo(1));
        }


        [Test]
        public async Task EditQuizGetDataFromForm_PopulatesIsSelectedCorrectly()
        {
            var selectedId = Guid.NewGuid();
            var otherQuestionId = Guid.NewGuid();

            var quiz = new Quiz
            {
                Id = Guid.NewGuid(),
                Title = "Edit Me",
                Description = "D",
                StartTime = DateTime.UtcNow,
                Questions = new List<Question> { new Question { Id = selectedId, Content = "Selected Q" } }
            };

            _quizRepoMock
                .Setup(r => r.GetAllQuestionsOrderByContentAsync())
                .ReturnsAsync(new List<Question>
                {
                    new Question { Id = selectedId, Content = "Selected Q", Points = 5 },
                    new Question { Id = otherQuestionId, Content = "Other Q", Points = 3 },
                });

            var result = await _sut.EditQuizGetDataFromForm(quiz);

            var selectedQ = result.SelectedQuestions.First(q => q.QuestionId == selectedId);
            var unselectedQ = result.SelectedQuestions.First(q => q.QuestionId == otherQuestionId);

            Assert.That(selectedQ.IsSelected, Is.True);
            Assert.That(unselectedQ.IsSelected, Is.False);
        }


        [Test]
        public async Task EditQuizAsync_ValidData_UpdatesQuizSuccessfully()
        {
            var quizId = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var quiz = new Quiz
            {
                Id = quizId,
                Title = "Old Title",
                Questions = new List<Question>()
            };
            var viewModel = new EditQuizViewModel
            {
                Id = quizId,
                Title = "New Title",
                Description = "New Desc",
                StartTime = DateTime.UtcNow
            };
            var selectedIds = new List<Guid> { questionId };

            _quizRepoMock
                .Setup(r => r.GetQuizWithQuestionsByIdAsync(quizId))
                .ReturnsAsync(quiz);
            _quizRepoMock
                .Setup(r => r.GetQuestionsFromTheirIdsAsync(selectedIds))
                .ReturnsAsync(new List<Question> { new Question { Id = questionId } });
            _quizRepoMock
                .Setup(r => r.UpdateQuizAsync(It.IsAny<Quiz>()))
                .ReturnsAsync(true);

            await _sut.EditQuizAsync(viewModel, selectedIds);

            _quizRepoMock.Verify(r => r.UpdateQuizAsync(It.IsAny<Quiz>()), Times.Once);
        }

        [Test]
        public void EditQuizAsync_QuizNotFound_ThrowsInvalidOperationException()
        {
            var viewModel = new EditQuizViewModel { Id = Guid.NewGuid() };
            _quizRepoMock
                .Setup(r => r.GetQuizWithQuestionsByIdAsync(viewModel.Id))
                .ReturnsAsync((Quiz?)null);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.EditQuizAsync(viewModel, new List<Guid> { Guid.NewGuid() }));
        }


        [Test]
        public async Task SoftDeleteQuizAsync_ValidId_CallsSoftDelete()
        {
            var id = Guid.NewGuid();
            var quiz = new Quiz { Id = id, Questions = new List<Question>() };
            _quizRepoMock.Setup(r => r.GetQuizWithQuestionsByIdAsync(id)).ReturnsAsync(quiz);
            _quizRepoMock.Setup(r => r.SoftDeleteQuizAsync(quiz)).ReturnsAsync(true);

            await _sut.SoftDeleteQuizAsync(id);

            _quizRepoMock.Verify(r => r.SoftDeleteQuizAsync(quiz), Times.Once);
        }

        [Test]
        public void SoftDeleteQuizAsync_QuizNotFound_ThrowsInvalidOperationException()
        {
            var id = Guid.NewGuid();
            _quizRepoMock.Setup(r => r.GetQuizWithQuestionsByIdAsync(id)).ReturnsAsync((Quiz?)null);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.SoftDeleteQuizAsync(id));
        }

        [Test]
        public void SoftDeleteQuizAsync_RepositoryFails_ThrowsInvalidOperationException()
        {
            var id = Guid.NewGuid();
            var quiz = new Quiz { Id = id, Questions = new List<Question>() };
            _quizRepoMock.Setup(r => r.GetQuizWithQuestionsByIdAsync(id)).ReturnsAsync(quiz);
            _quizRepoMock.Setup(r => r.SoftDeleteQuizAsync(quiz)).ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.SoftDeleteQuizAsync(id));
        }


        [Test]
        public async Task RestoreQuizAsync_ValidId_CallsRestore()
        {
            var id = Guid.NewGuid();
            var quiz = new Quiz { Id = id, Questions = new List<Question>() };
            _quizRepoMock.Setup(r => r.GetQuizWithQuestionsByIdAsync(id)).ReturnsAsync(quiz);
            _quizRepoMock.Setup(r => r.RestoreQuizAsync(quiz)).ReturnsAsync(true);

            await _sut.RestoreQuizAsync(id);

            _quizRepoMock.Verify(r => r.RestoreQuizAsync(quiz), Times.Once);
        }

        [Test]
        public void RestoreQuizAsync_QuizNotFound_ThrowsInvalidOperationException()
        {
            var id = Guid.NewGuid();
            _quizRepoMock.Setup(r => r.GetQuizWithQuestionsByIdAsync(id)).ReturnsAsync((Quiz?)null);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.RestoreQuizAsync(id));
        }

        [Test]
        public void RestoreQuizAsync_RepositoryFails_ThrowsInvalidOperationException()
        {
            var id = Guid.NewGuid();
            var quiz = new Quiz { Id = id, Questions = new List<Question>() };
            _quizRepoMock.Setup(r => r.GetQuizWithQuestionsByIdAsync(id)).ReturnsAsync(quiz);
            _quizRepoMock.Setup(r => r.RestoreQuizAsync(quiz)).ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.RestoreQuizAsync(id));
        }


        [Test]
        public async Task HardDeleteQuizAsync_ValidId_ClearsQuestionsAndDeletes()
        {
            var id = Guid.NewGuid();
            var quiz = new Quiz
            {
                Id = id,
                Questions = new List<Question> { new Question { Content = "Q" } }
            };
            _quizRepoMock.Setup(r => r.GetQuizWithQuestionsByIdAsync(id)).ReturnsAsync(quiz);
            _quizRepoMock.Setup(r => r.HardDeleteQuizAsync(quiz)).ReturnsAsync(true);

            await _sut.HardDeleteQuizAsync(id);

            Assert.That(quiz.Questions, Is.Empty);
            _quizRepoMock.Verify(r => r.HardDeleteQuizAsync(quiz), Times.Once);
        }

        [Test]
        public void HardDeleteQuizAsync_QuizNotFound_ThrowsInvalidOperationException()
        {
            var id = Guid.NewGuid();
            _quizRepoMock.Setup(r => r.GetQuizWithQuestionsByIdAsync(id)).ReturnsAsync((Quiz?)null);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.HardDeleteQuizAsync(id));
        }

        [Test]
        public void HardDeleteQuizAsync_RepositoryFails_ThrowsInvalidOperationException()
        {
            var id = Guid.NewGuid();
            var quiz = new Quiz { Id = id, Questions = new List<Question>() };
            _quizRepoMock.Setup(r => r.GetQuizWithQuestionsByIdAsync(id)).ReturnsAsync(quiz);
            _quizRepoMock.Setup(r => r.HardDeleteQuizAsync(quiz)).ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.HardDeleteQuizAsync(id));
        }

        
        [Test]
        public async Task CreateLeaderboardAsync_LeaderboardExists_ReturnsExistingLeaderboard()
        {
            var quizId = Guid.NewGuid();
            var existing = new Leaderboard { QuizId = quizId, Title = "Existing" };
            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardsWithEntriesByQuizIdAsync(quizId))
                .ReturnsAsync(existing);

            var result = await _sut.CreateLeaderboardAsync(quizId);

            Assert.That(result, Is.EqualTo(existing));
            _leaderboardRepoMock.Verify(r => r.AddLeaderboardAsync(It.IsAny<Leaderboard>()), Times.Never);
        }

        [Test]
        public async Task CreateLeaderboardAsync_NoLeaderboard_CreatesNewOne()
        {
            var quizId = Guid.NewGuid();
            var quiz = new Quiz { Id = quizId, Title = "Quiz", Description = "D" };
            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardsWithEntriesByQuizIdAsync(quizId))
                .ReturnsAsync((Leaderboard?)null);
            _quizRepoMock
                .Setup(r => r.GetQuizWithQuestionsByIdAsync(quizId))
                .ReturnsAsync(quiz);
            _leaderboardRepoMock
                .Setup(r => r.AddLeaderboardAsync(It.IsAny<Leaderboard>()))
                .ReturnsAsync(true);

            var result = await _sut.CreateLeaderboardAsync(quizId);

            Assert.That(result.QuizId, Is.EqualTo(quizId));
            Assert.That(result.Title, Is.EqualTo("Quiz"));
        }

        [Test]
        public void CreateLeaderboardAsync_AddFails_ThrowsInvalidOperationException()
        {
            var quizId = Guid.NewGuid();
            var quiz = new Quiz { Id = quizId, Title = "Quiz", Description = "D" };
            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardsWithEntriesByQuizIdAsync(quizId))
                .ReturnsAsync((Leaderboard?)null);
            _quizRepoMock
                .Setup(r => r.GetQuizWithQuestionsByIdAsync(quizId))
                .ReturnsAsync(quiz);
            _leaderboardRepoMock
                .Setup(r => r.AddLeaderboardAsync(It.IsAny<Leaderboard>()))
                .ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.CreateLeaderboardAsync(quizId));
        }

        [Test]
        public async Task SubmitScoreAsync_NewEntry_CreatesLeaderboardEntryAndRecalculatesRanks()
        {
            var quizId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var leaderboardId = Guid.NewGuid();
            var leaderboard = new Leaderboard { Id = leaderboardId, QuizId = quizId, Title = "LB" };

            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardsWithEntriesByQuizIdAsync(quizId))
                .ReturnsAsync(leaderboard);
            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardEntryForUserByIdAsync(leaderboardId, userId))
                .ReturnsAsync((LeaderboardEntry?)null);
            _leaderboardRepoMock
                .Setup(r => r.AddLeaderboardEntryAsync(It.IsAny<LeaderboardEntry>()))
                .ReturnsAsync(true);
            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardEntriesOrderedByScoreByLeaderboardIdAsync(leaderboardId))
                .ReturnsAsync(new List<LeaderboardEntry>
                {
                    new LeaderboardEntry { LeaderboardId = leaderboardId, UserId = userId, Score = 80 }
                });
            _leaderboardRepoMock
                .Setup(r => r.UpdateLeaderboardEntriesAsync(It.IsAny<LeaderboardEntry>()))
                .ReturnsAsync(true);

            await _sut.SubmitScoreAsync(quizId, userId, 80);

            _leaderboardRepoMock.Verify(r => r.AddLeaderboardEntryAsync(It.IsAny<LeaderboardEntry>()), Times.Once);
        }

        [Test]
        public async Task SubmitScoreAsync_ExistingEntryWithHigherScore_UpdatesScore()
        {
            var quizId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var leaderboardId = Guid.NewGuid();
            var leaderboard = new Leaderboard { Id = leaderboardId, QuizId = quizId, Title = "LB" };
            var existingEntry = new LeaderboardEntry
            {
                LeaderboardId = leaderboardId,
                UserId = userId,
                Score = 50
            };

            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardsWithEntriesByQuizIdAsync(quizId))
                .ReturnsAsync(leaderboard);
            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardEntryForUserByIdAsync(leaderboardId, userId))
                .ReturnsAsync(existingEntry);
            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardEntriesOrderedByScoreByLeaderboardIdAsync(leaderboardId))
                .ReturnsAsync(new List<LeaderboardEntry> { existingEntry });
            _leaderboardRepoMock
                .Setup(r => r.UpdateLeaderboardEntriesAsync(It.IsAny<LeaderboardEntry>()))
                .ReturnsAsync(true);

            await _sut.SubmitScoreAsync(quizId, userId, 90);

            Assert.That(existingEntry.Score, Is.EqualTo(90));
        }

        [Test]
        public async Task SubmitScoreAsync_ExistingEntryWithLowerOrEqualScore_DoesNotUpdateScore()
        {
            var quizId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var leaderboardId = Guid.NewGuid();
            var leaderboard = new Leaderboard { Id = leaderboardId, QuizId = quizId, Title = "LB" };
            var existingEntry = new LeaderboardEntry
            {
                LeaderboardId = leaderboardId,
                UserId = userId,
                Score = 95
            };

            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardsWithEntriesByQuizIdAsync(quizId))
                .ReturnsAsync(leaderboard);
            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardEntryForUserByIdAsync(leaderboardId, userId))
                .ReturnsAsync(existingEntry);
            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardEntriesOrderedByScoreByLeaderboardIdAsync(leaderboardId))
                .ReturnsAsync(new List<LeaderboardEntry> { existingEntry });
            _leaderboardRepoMock
                .Setup(r => r.UpdateLeaderboardEntriesAsync(It.IsAny<LeaderboardEntry>()))
                .ReturnsAsync(true);

            await _sut.SubmitScoreAsync(quizId, userId, 60);

            Assert.That(existingEntry.Score, Is.EqualTo(95));
        }

        [Test]
        public void SubmitScoreAsync_AddEntryFails_ThrowsInvalidOperationException()
        {
            var quizId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var leaderboardId = Guid.NewGuid();
            var leaderboard = new Leaderboard { Id = leaderboardId, QuizId = quizId, Title = "LB" };

            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardsWithEntriesByQuizIdAsync(quizId))
                .ReturnsAsync(leaderboard);
            _leaderboardRepoMock
                .Setup(r => r.GetLeaderboardEntryForUserByIdAsync(leaderboardId, userId))
                .ReturnsAsync((LeaderboardEntry?)null);
            _leaderboardRepoMock
                .Setup(r => r.AddLeaderboardEntryAsync(It.IsAny<LeaderboardEntry>()))
                .ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _sut.SubmitScoreAsync(quizId, userId, 80));
        }

        [Test]
        public async Task AddSelectedQuestions_AssignsReturnedQuestionsToQuiz()
        {
            var quiz = new Quiz { Questions = new List<Question>() };
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var questions = ids.Select(i => new Question { Id = i }).ToList();

            _quizRepoMock
                .Setup(r => r.GetQuestionsFromTheirIdsAsync(ids))
                .ReturnsAsync(questions);

            await _sut.AddSelectedQuestions(quiz, ids);

            Assert.That(quiz.Questions.Count, Is.EqualTo(2));
        }
    }
}
