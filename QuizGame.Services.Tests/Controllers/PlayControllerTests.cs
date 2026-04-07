using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuizGame.Controllers;
using QuizGame.Core.Contracts;
using QuizGame.Data.Models;
using QuizGame.ViewModels.Game;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuizGame.Services.Tests.Controllers
{
    [TestFixture]
    public class PlayControllerTests
    {
        private Mock<IGameService> _gameServiceMock;
        private Mock<IQuizService> _quizServiceMock;
        private Mock<ILogger<PlayController>> _loggerMock;
        private PlayController _controller;

        [SetUp]
        public void SetUp()
        {
            _gameServiceMock = new Mock<IGameService>();
            _quizServiceMock = new Mock<IQuizService>();
            _loggerMock = new Mock<ILogger<PlayController>>();

            _controller = new PlayController(
                _gameServiceMock.Object,
                _quizServiceMock.Object,
                _loggerMock.Object
            );

            // Set up TempData
            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempDataDictionary = new TempDataDictionary(
                new DefaultHttpContext(), tempDataProvider.Object);
            _controller.TempData = tempDataDictionary;

            // Set up a fake authenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TearDown]
        public void DisposeData()
        {
            _controller.Dispose();
        }

        // ─── Index ────────────────────────────────────────────────────────────────

        [Test]
        public async Task Index_ReturnsViewResult_WithQuizList()
        {
            // Arrange
            var quizzes = new List<Quiz> { new Quiz(), new Quiz() };
            _quizServiceMock.Setup(s => s.GetAllQuizzesAsync())
                            .ReturnsAsync(quizzes);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(quizzes));
        }

        [Test]
        public async Task Index_WhenServiceThrows_SetsTempDataAndRedirects()
        {
            // Arrange
            _quizServiceMock.Setup(s => s.GetAllQuizzesAsync())
                            .ThrowsAsync(new Exception("db error"));

            // Act
            var result = await _controller.Index();

            // Assert
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(_controller.Index)));
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        // ─── Start ────────────────────────────────────────────────────────────────

        [Test]
        public async Task Start_WithEmptyGuid_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Start(Guid.Empty);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Start_WithValidId_RedirectsToQuestion()
        {
            // Arrange
            var quizId = Guid.NewGuid();
            var attemptId = Guid.NewGuid();
            _gameServiceMock.Setup(s => s.StartAttemptAsync(quizId, _controller.User))
                            .ReturnsAsync(attemptId);

            // Act
            var result = await _controller.Start(quizId);

            // Assert
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(_controller.Question)));
            Assert.That(redirect.RouteValues["attemptId"], Is.EqualTo(attemptId));
        }

        [Test]
        public async Task Start_WhenInvalidOperationExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            var quizId = Guid.NewGuid();
            _gameServiceMock.Setup(s => s.StartAttemptAsync(quizId, _controller.User))
                            .ThrowsAsync(new InvalidOperationException("already started"));

            // Act
            var result = await _controller.Start(quizId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Start_WhenGenericExceptionThrown_RedirectsToIndex()
        {
            // Arrange
            var quizId = Guid.NewGuid();
            _gameServiceMock.Setup(s => s.StartAttemptAsync(quizId, _controller.User))
                            .ThrowsAsync(new Exception("unexpected"));

            // Act
            var result = await _controller.Start(quizId);

            // Assert
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        // ─── Question ─────────────────────────────────────────────────────────────

        [Test]
        public async Task Question_WithEmptyGuid_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Question(Guid.Empty);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Question_WhenVmIsNull_ReturnsNotFound()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            _gameServiceMock.Setup(s => s.GetCurrentQuestionAsync(attemptId))
                            .ReturnsAsync((PlayQuestionViewModel?)null);

            // Act
            var result = await _controller.Question(Guid.Empty);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Question_WhenVmReturned_ReturnsView()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var vm = new PlayQuestionViewModel();
            _gameServiceMock.Setup(s => s.GetCurrentQuestionAsync(attemptId))
                .ReturnsAsync(vm);

            // Act
            var result = await _controller.Question(attemptId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(vm));
        }

        [Test]
        public async Task Question_WhenVmIsNull_RedirectsToFinish()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            _gameServiceMock.Setup(s => s.GetCurrentQuestionAsync(attemptId))
                .ReturnsAsync((PlayQuestionViewModel?)null);

            // Act
            var result = await _controller.Question(attemptId);

            // Assert
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(_controller.Finish)));
            Assert.That(redirect.RouteValues["attemptId"], Is.EqualTo(attemptId));
        }

        [Test]
        public async Task Question_WhenExceptionThrown_RedirectsToIndex()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            _gameServiceMock.Setup(s => s.GetCurrentQuestionAsync(attemptId))
                            .ThrowsAsync(new Exception("db failure"));

            // Act
            var result = await _controller.Question(attemptId);

            // Assert
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        // ─── SubmitAnswer ─────────────────────────────────────────────────────────

        [Test]
        public async Task SubmitAnswer_WithEmptySelectedAnswerId_RedirectsToQuestion()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var questionId = Guid.NewGuid();

            // Act
            var result = await _controller.SubmitAnswer(attemptId, questionId, Guid.Empty);

            // Assert
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(_controller.Question)));
            Assert.That(redirect.RouteValues["attemptId"], Is.EqualTo(attemptId));
        }

        [Test]
        public async Task SubmitAnswer_WithValidIds_CallsServiceAndRedirectsToQuestion()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var answerId = Guid.NewGuid();

            _gameServiceMock.Setup(s => s.SubmitAnswerAsync(attemptId, questionId, answerId))
                            .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SubmitAnswer(attemptId, questionId, answerId);

            // Assert
            _gameServiceMock.Verify(s => s.SubmitAnswerAsync(attemptId, questionId, answerId), Times.Once);
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(_controller.Question)));
        }

        [Test]
        public async Task SubmitAnswer_WhenInvalidOperationExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var answerId = Guid.NewGuid();

            _gameServiceMock.Setup(s => s.SubmitAnswerAsync(attemptId, questionId, answerId))
                            .ThrowsAsync(new InvalidOperationException());

            // Act
            var result = await _controller.SubmitAnswer(attemptId, questionId, answerId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task SubmitAnswer_WhenGenericExceptionThrown_RedirectsToIndex()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var answerId = Guid.NewGuid();

            _gameServiceMock.Setup(s => s.SubmitAnswerAsync(attemptId, questionId, answerId))
                            .ThrowsAsync(new Exception("unexpected"));

            // Act
            var result = await _controller.SubmitAnswer(attemptId, questionId, answerId);

            // Assert
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        // ─── Finish ───────────────────────────────────────────────────────────────

        [Test]
        public async Task Finish_WithValidAttempt_ReturnsViewWithSummary()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var summary = new GameSummaryViewModel();

            _gameServiceMock.Setup(s => s.FinishAttemptAsync(attemptId))
                            .ReturnsAsync(summary);

            // Act
            var result = await _controller.Finish(attemptId, Guid.Empty, Guid.Empty);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(summary));
        }

        [Test]
        public async Task Finish_WhenSummaryIsNull_SetsTempDataAndRedirectsToIndex()
        {
            // Arrange
            var attemptId = Guid.NewGuid();

            _gameServiceMock.Setup(s => s.FinishAttemptAsync(attemptId))
                            .ReturnsAsync((GameSummaryViewModel?)null);

            // Act
            var result = await _controller.Finish(attemptId, Guid.Empty, Guid.Empty);

            // Assert
            Assert.That(_controller.TempData["ErrorMessage"], Is.Null);
        }

        [Test]
        public async Task Finish_WhenInvalidOperationExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            var attemptId = Guid.NewGuid();

            _gameServiceMock.Setup(s => s.FinishAttemptAsync(attemptId))
                            .ThrowsAsync(new InvalidOperationException());

            // Act
            var result = await _controller.Finish(attemptId, Guid.Empty, Guid.Empty);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Finish_WhenGenericExceptionThrown_RedirectsToIndex()
        {
            // Arrange
            var attemptId = Guid.NewGuid();

            _gameServiceMock.Setup(s => s.FinishAttemptAsync(attemptId))
                            .ThrowsAsync(new Exception("unexpected"));

            // Act
            var result = await _controller.Finish(attemptId, Guid.Empty, Guid.Empty);

            // Assert
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }
    }
}