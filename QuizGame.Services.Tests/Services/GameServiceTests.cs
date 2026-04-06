using Moq;
using NUnit.Framework;
using QuizGame.Core;
using QuizGame.Core.Contracts;
using QuizGame.Data.Models;
using QuizGame.Data.Repository.Contracts;
using System.Security.Claims;

namespace QuizGame.Services.Tests.Services
{
    [TestFixture]
    public class GameServiceTests
    {
        private Mock<IGameRepository> _gameRepoMock;
        private Mock<IQuizRepository> _quizRepoMock;
        private Mock<IQuizService> _quizServiceMock;
        private Mock<ILeaderboardRepository> _leaderboardRepoMock;
        private GameService _gameService;

        [SetUp]
        public void SetUp()
        {
            _gameRepoMock = new Mock<IGameRepository>();
            _quizRepoMock = new Mock<IQuizRepository>();
            _quizServiceMock = new Mock<IQuizService>();
            _leaderboardRepoMock = new Mock<ILeaderboardRepository>();

            _gameService = new GameService(
                _quizRepoMock.Object,
                _leaderboardRepoMock.Object,
                _quizServiceMock.Object,
                _gameRepoMock.Object);
        }

        [Test]
        public async Task StartAttemptAsync_ValidQuiz_ReturnsAttemptId()
        {
            // Arrange
            var quizId = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            var quiz = new Quiz
            {
                Id = quizId,
                Questions = new List<Question> { new Question { Points = 10 } }
            };

            _quizRepoMock.Setup(r => r.GetQuizWithQuestionsByIdAsync(quizId))
                .ReturnsAsync(quiz);

            _gameRepoMock.Setup(r => r.AddQuizAttemptAsync(It.IsAny<QuizAttempt>()))
                .ReturnsAsync(true);

            // Act
            var result = await _gameService.StartAttemptAsync(quizId, user);

            // Assert
            Assert.That(result, Is.Not.EqualTo(Guid.Empty));
            _gameRepoMock.Verify(r => r.AddQuizAttemptAsync(It.Is<QuizAttempt>(a => a.MaxScore == 10)), Times.Once);
        }

        [Test]
        public void StartAttemptAsync_UserNotLoggedIn_ThrowsException()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity()); // No claims

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _gameService.StartAttemptAsync(Guid.NewGuid(), user));
            Assert.That(ex.Message, Is.EqualTo("User not logged in"));
        }

        [Test]
        public void StartAttemptAsync_QuizNotFound_ThrowsException()
        {
            // Arrange
            var quizId = Guid.NewGuid();
            var user = CreateUser(Guid.NewGuid().ToString());

            _quizRepoMock.Setup(r => r.GetQuizWithQuestionsByIdAsync(quizId))
                .ReturnsAsync((Quiz)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _gameService.StartAttemptAsync(quizId, user));
            Assert.That(ex.Message, Is.EqualTo("Quiz not found"));
        }

        [Test]
        public void StartAttemptAsync_RepositorySaveFails_ThrowsInvalidOperationException()
        {
            // Arrange
            var quizId = Guid.NewGuid();
            var user = CreateUser(Guid.NewGuid().ToString());
            var quiz = new Quiz { Id = quizId, Questions = new List<Question>() };

            _quizRepoMock.Setup(r => r.GetQuizWithQuestionsByIdAsync(quizId)).ReturnsAsync(quiz);

            _gameRepoMock.Setup(r => r.AddQuizAttemptAsync(It.IsAny<QuizAttempt>())).ReturnsAsync(false);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _gameService.StartAttemptAsync(quizId, user));
        }

        [Test]
        public async Task GetCurrentQuestionAsync_ValidAttempt_ReturnsViewModel()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var attempt = new QuizAttempt
            {
                Id = attemptId,
                CurrentQuestionIndex = 0,
                Quiz = new Quiz
                {
                    Questions = new List<Question>
                    {
                        new Question { Id = Guid.NewGuid(), Content = "What is C#?", Answers = new List<Answer>() }
                    }
                }
            };

            _gameRepoMock.Setup(r => r.GetQuizAttemptWithQuizQuestionsAndAnswersByIdAsync(attemptId))
                .ReturnsAsync(attempt);

            // Act
            var result = await _gameService.GetCurrentQuestionAsync(attemptId);

            // Assert
            Assert.That(result,Is.Not.Null);
            Assert.That(result.QuestionContent, Is.EqualTo("What is C#?"));
        }

        [Test]
        public async Task GetCurrentQuestionAsync_AttemptIsFinished_ReturnsNull()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var finishedAttempt = new QuizAttempt { IsFinished = true };

            _gameRepoMock.Setup(r => r.GetQuizAttemptWithQuizQuestionsAndAnswersByIdAsync(attemptId))
                .ReturnsAsync(finishedAttempt);

            // Act
            var result = await _gameService.GetCurrentQuestionAsync(attemptId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCurrentQuestionAsync_IndexOutOfRange_ReturnsNull()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var attempt = new QuizAttempt
            {
                CurrentQuestionIndex = 5, // High index
                Quiz = new Quiz { Questions = new List<Question> { new Question() } } // Only 1 question
            };

            _gameRepoMock.Setup(r => r.GetQuizAttemptWithQuizQuestionsAndAnswersByIdAsync(attemptId))
                .ReturnsAsync(attempt);

            // Act
            var result = await _gameService.GetCurrentQuestionAsync(attemptId);

            // Assert
            Assert.That(result,Is.Null);
        }

        [Test]
        public async Task SubmitAnswerAsync_CorrectAnswer_IncrementsScore()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var answerId = Guid.NewGuid();

            var question = new Question
            {
                Id = questionId,
                Points = 5,
                Answers = new List<Answer> { new Answer { Id = answerId, IsCorrect = true } }
            };

            var attempt = new QuizAttempt
            {
                Id = attemptId,
                Score = 0,
                Quiz = new Quiz { Questions = new List<Question> { question } }
            };

            _gameRepoMock.Setup(r => r.GetQuizAttemptWithQuizQuestionsAndAnswersByIdAsync(attemptId))
                .ReturnsAsync(attempt);
            _gameRepoMock.Setup(r => r.AddAttemptAnswerAsync(It.IsAny<AttemptAnswer>()))
                .ReturnsAsync(true);
            _gameRepoMock.Setup(r => r.UpdateAttempAnswersAsync(It.IsAny<AttemptAnswer>()))
                .ReturnsAsync(true);

            // Act
            await _gameService.SubmitAnswerAsync(attemptId, questionId, answerId);

            // Assert
            Assert.That(attempt.Score, Is.EqualTo(5));
            Assert.That(attempt.CurrentQuestionIndex, Is.EqualTo(1));
        }

        [Test]
        public void SubmitAnswerAsync_WrongQuestionIdForQuiz_ThrowsException()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var attempt = new QuizAttempt
            {
                Quiz = new Quiz { Questions = new List<Question> { new Question { Id = Guid.NewGuid() } } }
            };

            _gameRepoMock.Setup(r => r.GetQuizAttemptWithQuizQuestionsAndAnswersByIdAsync(attemptId))
                .ReturnsAsync(attempt);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() =>
                _gameService.SubmitAnswerAsync(attemptId, Guid.NewGuid(), Guid.NewGuid()));
            Assert.That(ex.Message, Is.EqualTo("Question not found in this quiz"));
        }

        [Test]
        public async Task FinishAttemptAsync_ValidId_ReturnsSummary()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var quizId = Guid.NewGuid();
            var attempt = new QuizAttempt
            {
                Id = attemptId,
                QuizId = quizId,
                Score = 10,
                MaxScore = 20,
                Quiz = new Quiz { Title = "Math Quiz" },
                Answers = new List<AttemptAnswer>
                {
                    new AttemptAnswer { IsCorrect = true },
                    new AttemptAnswer { IsCorrect = false }
                }
            };

            _gameRepoMock.Setup(r => r.GetQuizAttemptWithQuizAndAnswersByIdAsync(attemptId))
                .ReturnsAsync(attempt);
            _leaderboardRepoMock.Setup(r => r.GetLeaderboardWithEntriesAndUserByQuizIdAsync(quizId))
                .ReturnsAsync(new Leaderboard { Id = Guid.NewGuid() });

            // Act
            var result = await _gameService.FinishAttemptAsync(attemptId);

            // Assert
            Assert.That(result.QuizTitle, Is.EqualTo("Math Quiz"));
            Assert.That(result.CorrectAnswers, Is.EqualTo(1));
            _quizServiceMock.Verify(s => s.SubmitScoreAsync(quizId, It.IsAny<Guid>(), 10), Times.Once);
        }

        [Test]
        public async Task SubmitAnswerAsync_IncorrectAnswer_DoesNotIncreaseScore()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var answerId = Guid.NewGuid();

            var question = new Question
            {
                Id = questionId,
                Points = 10,
                Answers = new List<Answer>
                    { new Answer { Id = answerId, IsCorrect = false } }
            };

            var attempt = new QuizAttempt { Score = 0, Quiz = new Quiz { Questions = new List<Question> { question } } };

            _gameRepoMock.Setup(r => r.GetQuizAttemptWithQuizQuestionsAndAnswersByIdAsync(attemptId)).ReturnsAsync(attempt);
            _gameRepoMock.Setup(r => r.AddAttemptAnswerAsync(It.IsAny<AttemptAnswer>())).ReturnsAsync(true);
            _gameRepoMock.Setup(r => r.UpdateAttempAnswersAsync(It.IsAny<AttemptAnswer>())).ReturnsAsync(true);

            // Act
            await _gameService.SubmitAnswerAsync(attemptId, questionId, answerId);

            // Assert
            Assert.That(attempt.Score, Is.EqualTo(0));
            Assert.That(attempt.CurrentQuestionIndex, Is.EqualTo(1));
        }


        private ClaimsPrincipal CreateUser(string userId)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }));
        }
    }
}
