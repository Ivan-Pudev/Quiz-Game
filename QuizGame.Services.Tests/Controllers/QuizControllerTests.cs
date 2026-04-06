using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuizGame.Controllers;
using QuizGame.Core.Contracts;
using QuizGame.Data.Models;
using QuizGame.ViewModels.Leaderboards;
using QuizGame.ViewModels.Quizzes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizGame.Services.Tests.Controllers
{
    [TestFixture]
    public class QuizControllerTests
    {
        private Mock<IQuizService> _quizServiceMock;
        private Mock<ILeaderboardService> _leaderboardServiceMock;
        private Mock<ILogger<QuizController>> _loggerMock;

        private QuizController _controller;

        [SetUp]
        public void Setup()
        {
            _quizServiceMock = new Mock<IQuizService>();
            _leaderboardServiceMock = new Mock<ILeaderboardService>();
            _loggerMock = new Mock<ILogger<QuizController>>();

            _controller = new QuizController(
                _quizServiceMock.Object,
                _leaderboardServiceMock.Object,
                _loggerMock.Object);

            // ✅ FIX: Setup HttpContext
            var httpContext = new DefaultHttpContext();

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // ✅ FIX: Setup TempData
            _controller.TempData = new TempDataDictionary(
                httpContext,
                Mock.Of<ITempDataProvider>()
            );
        }

        [TearDown]
        public void TearDown()
        {
            if (_controller is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }


        [Test]
        public async Task Index_ReturnsView_WithQuizzes()
        {
            var quizzes = new List<Quiz> { new Quiz(), new Quiz() };

            _quizServiceMock.Setup(x => x.GetAllQuizzesAsync())
                .ReturnsAsync(quizzes);

            var result = await _controller.Index();

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = (ViewResult)result;

            Assert.That(viewResult.Model, Is.EqualTo(quizzes));
            _quizServiceMock.Verify(x => x.GetAllQuizzesAsync(), Times.Once);
        }

        [Test]
        public async Task Index_OnException_RedirectsToIndex()
        {
            _quizServiceMock.Setup(x => x.GetAllQuizzesAsync())
                .ThrowsAsync(new Exception());

            var result = await _controller.Index();

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }


        [Test]
        public async Task Create_Get_ReturnsView_WithModel()
        {
            var vm = new CreateQuizViewModel();

            _quizServiceMock.Setup(x => x.CreateQuizFormAsync())
                .ReturnsAsync(vm);

            var result = await _controller.Create();

            Assert.That(result, Is.TypeOf<ViewResult>());
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.EqualTo(vm));
        }


        [Test]
        public async Task Create_Post_InvalidModel_ReturnsSameViewModel()
        {
            _controller.ModelState.AddModelError("error", "error");

            var model = new CreateQuizViewModel();

            var result = await _controller.Create(model);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task Create_Post_Valid_RedirectsToIndex_AndCallsService()
        {
            var model = new CreateQuizViewModel();

            var result = await _controller.Create(model);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            _quizServiceMock.Verify(x => x.CreateQuizAsync(model), Times.Once);
        }

        [Test]
        public async Task Create_Post_InvalidOperation_ReturnsBadRequest()
        {
            // Arrange
            _quizServiceMock.Setup(x => x.CreateQuizAsync(It.IsAny<CreateQuizViewModel>()))
                .ThrowsAsync(new InvalidOperationException());

            _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>()
            );

            // Act
            var result = await _controller.Create(new CreateQuizViewModel());

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task Create_Post_GenericException_ReturnsView_WithQuestions()
        {
            var model = new CreateQuizViewModel();

            _quizServiceMock.Setup(x => x.CreateQuizAsync(model))
                .ThrowsAsync(new Exception());

            _quizServiceMock.Setup(x => x.GetAllQuestionsAsync())
                .ReturnsAsync(new List<Question> { new Question() });

            var result = await _controller.Create(model);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.EqualTo(model));
            Assert.That(model.Questions, Is.Not.Empty);
        }


        [Test]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.Details(Guid.Empty);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Details_QuizNotFound_ReturnsNotFound()
        {
            _quizServiceMock.Setup(x => x.GetQuizByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Quiz)null);

            var result = await _controller.Details(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Details_Valid_ReturnsView_WithModel()
        {
            var quiz = new Quiz();
            var vm = new DetailsQuizViewModel();

            _quizServiceMock.Setup(x => x.GetQuizByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(quiz);

            _quizServiceMock.Setup(x => x.ShowQuizDetails(quiz))
                .Returns(vm);

            var result = await _controller.Details(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<ViewResult>());
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.EqualTo(vm));
        }


        [Test]
        public async Task Edit_Get_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.Edit(Guid.Empty);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_Get_QuizNotFound_ReturnsNotFound()
        {
            _quizServiceMock.Setup(x => x.GetQuizByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Quiz)null);

            var result = await _controller.Edit(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_Get_Valid_ReturnsView()
        {
            var quiz = new Quiz();
            var vm = new EditQuizViewModel();

            _quizServiceMock.Setup(x => x.GetQuizByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(quiz);

            _quizServiceMock.Setup(x => x.EditQuizGetDataFromForm(quiz))
                .ReturnsAsync(vm);

            var result = await _controller.Edit(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<ViewResult>());
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.EqualTo(vm));
        }

        [Test]
        public async Task Edit_Post_Valid_UpdatesQuiz_AndRedirects()
        {
            var id = Guid.NewGuid();

            var quiz = new Quiz();

            var vm = new EditQuizViewModel
            {
                Id = id,
                SelectedQuestions = new List<QuestionSelectionViewModel>
                {
                    new QuestionSelectionViewModel { QuestionId = Guid.NewGuid(), IsSelected = true },
                    new QuestionSelectionViewModel { QuestionId = Guid.NewGuid(), IsSelected = false }
                }
            };

            _quizServiceMock.Setup(x => x.GetQuizByIdAsync(id))
                .ReturnsAsync(quiz);

            _quizServiceMock.Setup(x => x.EditQuizGetDataFromForm(quiz))
                .ReturnsAsync(vm);

            // Act
            var result = await _controller.Edit(id, vm);

            // Assert
            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo("Index"));

            _quizServiceMock.Verify(x =>
                    x.EditQuizAsync(vm, It.Is<List<Guid>>(l => l.Count == 1)),
                Times.Once);
        }


        [Test]
        public async Task Delete_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.Delete(Guid.Empty);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Delete_Valid_RedirectsToIndex_AndCallsService()
        {
            var id = Guid.NewGuid();

            var result = await _controller.Delete(id);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            _quizServiceMock.Verify(x => x.SoftDeleteQuizAsync(id), Times.Once);
        }

        [Test]
        public async Task Delete_InvalidOperation_ReturnsBadRequest()
        {
            _quizServiceMock.Setup(x => x.SoftDeleteQuizAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new InvalidOperationException());

            var result = await _controller.Delete(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task DeletedQuizzes_ReturnsView_WithQuizzes()
        {
            // Arrange
            var data = new List<DetailsQuizViewModel>
            {
                new DetailsQuizViewModel(),
                new DetailsQuizViewModel()
            };

            _quizServiceMock.Setup(x => x.GetAllDeletedQuizzesAsync())
                .ReturnsAsync(data);

            // Act
            var result = await _controller.DeletedQuizzes();

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>());
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.EqualTo(data));
        }

        [Test]
        public async Task DeletedQuizzes_OnException_RedirectsToIndex()
        {
            _quizServiceMock.Setup(x => x.GetAllDeletedQuizzesAsync())
                .ThrowsAsync(new Exception());

            var result = await _controller.DeletedQuizzes();

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }


        [Test]
        public async Task Leaderboard_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.Leaderboard(Guid.Empty);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Leaderboard_Valid_ReturnsView_WithData()
        {
            var data = new List<LeaderboardRowVm>();

            _leaderboardServiceMock.Setup(x => x.GetLeaderboardEntriesByQuizIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(data);

            var result = await _controller.Leaderboard(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<ViewResult>());
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.EqualTo(data));
        }


        [Test]
        public async Task RestoreQuiz_Valid_CallsService_AndRedirects()
        {
            var id = Guid.NewGuid();

            var result = await _controller.RestoreQuiz(id);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo("DeletedQuizzes"));

            _quizServiceMock.Verify(x => x.RestoreQuizAsync(id), Times.Once);
        }

        [Test]
        public async Task RestoreQuiz_InvalidOperation_ReturnsBadRequest()
        {
            _quizServiceMock.Setup(x => x.RestoreQuizAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new InvalidOperationException());

            var result = await _controller.RestoreQuiz(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }


        [Test]
        public async Task DeleteQuiz_Valid_CallsService_AndRedirects()
        {
            var id = Guid.NewGuid();

            var result = await _controller.DeleteQuiz(id);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo("Index"));

            _quizServiceMock.Verify(x => x.HardDeleteQuizAsync(id), Times.Once);
        }

        [Test]
        public async Task DeleteQuiz_InvalidOperation_ReturnsBadRequest()
        {
            _quizServiceMock.Setup(x => x.HardDeleteQuizAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new InvalidOperationException());

            var result = await _controller.DeleteQuiz(Guid.NewGuid());

            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }
    }
}