using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace QuizGame.Services.Tests.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using QuizGame.Controllers;
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Admin.Leaderboard;
    using QuizGame.ViewModels.Leaderboards;

    [TestFixture]
    public class LeaderboardControllerTests
    {
        private Mock<ILeaderboardService> _leaderboardServiceMock;
        private Mock<ILogger<LeaderboardController>> _loggerMock;
        private LeaderboardController _controller;

        [SetUp]
        public void SetUp()
        {
            _leaderboardServiceMock = new Mock<ILeaderboardService>();
            _loggerMock = new Mock<ILogger<LeaderboardController>>();
            _controller = new LeaderboardController(_leaderboardServiceMock.Object, _loggerMock.Object);

            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());
        }

        [TearDown]
        public void DisposeData()
        {
            _controller.Dispose();
        }

        // ──────────────────────────────────────────────
        // Index
        // ──────────────────────────────────────────────

        [Test]
        public async Task Index_ReturnsViewResult_WithLeaderboards()
        {
            var leaderboards = new List<Leaderboard> { new Leaderboard(), new Leaderboard() };
            _leaderboardServiceMock
                .Setup(s => s.GetLeaderboardsAsync())
                .ReturnsAsync(leaderboards);

            IActionResult result = await _controller.Index();

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(leaderboards));
        }

        [Test]
        public async Task Index_OnException_RedirectsToIndex_AndSetsErrorTempData()
        {
            _leaderboardServiceMock
                .Setup(s => s.GetLeaderboardsAsync())
                .ThrowsAsync(new Exception("db error"));

            IActionResult result = await _controller.Index();

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.Index)));
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        // ──────────────────────────────────────────────
        // Rankings
        // ──────────────────────────────────────────────

        [Test]
        public async Task Rankings_WithEmptyGuid_ReturnsNotFound()
        {
            IActionResult result = await _controller.Rankings(Guid.Empty);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        [Test]
        public async Task Rankings_WithValidId_ReturnsViewResult_WithEntries()
        {
            var id = Guid.NewGuid();
            var entries = new List<LeaderboardRowVm> { new LeaderboardRowVm() };
            _leaderboardServiceMock
                .Setup(s => s.GetLeaderboardEntriesByQuizIdAsync(id))
                .ReturnsAsync(entries);

            IActionResult result = await _controller.Rankings(id);

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(entries));
        }

        [Test]
        public async Task Rankings_OnInvalidOperationException_ReturnsBadRequest()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.GetLeaderboardEntriesByQuizIdAsync(id))
                .ThrowsAsync(new InvalidOperationException());

            IActionResult result = await _controller.Rankings(id);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        [Test]
        public async Task Rankings_OnGeneralException_RedirectsToIndex()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.GetLeaderboardEntriesByQuizIdAsync(id))
                .ThrowsAsync(new Exception("unexpected"));

            IActionResult result = await _controller.Rankings(id);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.Index)));
        }

        // ──────────────────────────────────────────────
        // Details
        // ──────────────────────────────────────────────

        [Test]
        public async Task Details_ReturnsViewResult_WithAdminViewModels()
        {
            var viewModels = new AdminLeaderboardPageViewModel();
            _leaderboardServiceMock
                .Setup(s => s.GetLeaderboardsToManageAsync())
                .ReturnsAsync(viewModels);

            IActionResult result = await _controller.Details();

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(viewModels));
        }

        [Test]
        public async Task Details_OnException_RedirectsToIndex_AndSetsErrorTempData()
        {
            _leaderboardServiceMock
                .Setup(s => s.GetLeaderboardsToManageAsync())
                .ThrowsAsync(new Exception("fail"));

            IActionResult result = await _controller.Details();

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.Index)));
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        // ──────────────────────────────────────────────
        // ManageEntries
        // ──────────────────────────────────────────────

        [Test]
        public async Task ManageEntries_WithEmptyGuid_ReturnsNotFound()
        {
            IActionResult result = await _controller.ManageEntries(Guid.Empty);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        [Test]
        public async Task ManageEntries_WithValidId_ReturnsViewResult()
        {
            var id = Guid.NewGuid();
            var viewModel = new AdminManageEntriesViewModel();
            _leaderboardServiceMock
                .Setup(s => s.GetLeaderboardsEntriesToManageDetailsAsync(id))
                .ReturnsAsync(viewModel);

            IActionResult result = await _controller.ManageEntries(id);

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(viewModel));
        }

        [Test]
        public async Task ManageEntries_OnException_RedirectsToIndex()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.GetLeaderboardsEntriesToManageDetailsAsync(id))
                .ThrowsAsync(new Exception("fail"));

            IActionResult result = await _controller.ManageEntries(id);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.Index)));
        }

        // ──────────────────────────────────────────────
        // UpdateEntry
        // ──────────────────────────────────────────────

        [Test]
        public async Task UpdateEntry_WithEmptyGuid_ReturnsNotFound()
        {
            IActionResult result = await _controller.UpdateEntry(Guid.Empty, 100);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        [Test]
        public async Task UpdateEntry_WithValidId_RedirectsToIndex()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.UpdateEntryAsync(id, 100))
                .Returns(Task.CompletedTask);

            IActionResult result = await _controller.UpdateEntry(id, 100);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.Index)));
        }

        [Test]
        public async Task UpdateEntry_OnInvalidOperationException_ReturnsBadRequest()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.UpdateEntryAsync(id, It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException());

            IActionResult result = await _controller.UpdateEntry(id, 50);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        [Test]
        public async Task UpdateEntry_OnGeneralException_RedirectsToIndex()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.UpdateEntryAsync(id, It.IsAny<int>()))
                .ThrowsAsync(new Exception("unexpected"));

            IActionResult result = await _controller.UpdateEntry(id, 50);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.Index)));
        }

        // ──────────────────────────────────────────────
        // Delete (soft)
        // ──────────────────────────────────────────────

        [Test]
        public async Task Delete_WithEmptyGuid_ReturnsNotFound()
        {
            IActionResult result = await _controller.Delete(Guid.Empty);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        [Test]
        public async Task Delete_WithValidId_RedirectsToDetails_AndSetsSuccessTempData()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.SoftDeleteEntryAsync(id))
                .Returns(Task.CompletedTask);

            IActionResult result = await _controller.Delete(id);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.Details)));
            Assert.That(_controller.TempData["SuccessMessage"], Is.Not.Null);
        }

        [Test]
        public async Task Delete_OnInvalidOperationException_ReturnsBadRequest()
        {
            Guid? id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.SoftDeleteEntryAsync(id))
                .ThrowsAsync(new InvalidOperationException());

            IActionResult result = await _controller.Delete(id);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        [Test]
        public async Task Delete_OnGeneralException_RedirectsToIndex()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.SoftDeleteEntryAsync(id))
                .ThrowsAsync(new Exception("fail"));

            IActionResult result = await _controller.Delete(id);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.Index)));
        }

        // ──────────────────────────────────────────────
        // GlobalLeaderboard
        // ──────────────────────────────────────────────

        [Test]
        public async Task GlobalLeaderboard_ReturnsViewResult_WithViewModel()
        {
            var viewModel = new AdminGlobalLeaderboardViewModel();
            _leaderboardServiceMock
                .Setup(s => s.GetGlobalLeaderboardAsync())
                .ReturnsAsync(viewModel);

            IActionResult result = await _controller.GlobalLeaderboard();

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(viewModel));
        }

        [Test]
        public async Task GlobalLeaderboard_OnException_RedirectsToIndex_AndSetsErrorTempData()
        {
            _leaderboardServiceMock
                .Setup(s => s.GetGlobalLeaderboardAsync())
                .ThrowsAsync(new Exception("fail"));

            IActionResult result = await _controller.GlobalLeaderboard();

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.Index)));
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        // ──────────────────────────────────────────────
        // DeletedEntries
        // ──────────────────────────────────────────────

        [Test]
        public async Task DeletedEntries_ReturnsViewResult_WithEntries()
        {
            var entries = new List<AdminLeaderboardEntryViewModel> { new AdminLeaderboardEntryViewModel() };
            _leaderboardServiceMock
                .Setup(s => s.GetLeaderboardsEntriesToManageAsync())
                .ReturnsAsync(entries);

            IActionResult result = await _controller.DeletedEntries();

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(entries));
        }

        [Test]
        public async Task DeletedEntries_OnException_RedirectsToIndex_AndSetsErrorTempData()
        {
            _leaderboardServiceMock
                .Setup(s => s.GetLeaderboardsEntriesToManageAsync())
                .ThrowsAsync(new Exception("fail"));

            IActionResult result = await _controller.DeletedEntries();

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.Index)));
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        // ──────────────────────────────────────────────
        // RestoreEntry
        // ──────────────────────────────────────────────

        [Test]
        public async Task RestoreEntry_WithEmptyGuid_ReturnsNotFound()
        {
            IActionResult result = await _controller.RestoreEntry(Guid.Empty);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        [Test]
        public async Task RestoreEntry_WithValidId_RedirectsToDeletedEntries_AndSetsSuccessTempData()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.RestoreEntryAsync(id))
                .Returns(Task.CompletedTask);

            IActionResult result = await _controller.RestoreEntry(id);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.DeletedEntries)));
            Assert.That(_controller.TempData["SuccessMessage"], Is.Not.Null);
        }

        [Test]
        public async Task RestoreEntry_OnInvalidOperationException_ReturnsBadRequest()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.RestoreEntryAsync(id))
                .ThrowsAsync(new InvalidOperationException());

            IActionResult result = await _controller.RestoreEntry(id);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        [Test]
        public async Task RestoreEntry_OnGeneralException_RedirectsToDeletedEntries()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.RestoreEntryAsync(id))
                .ThrowsAsync(new Exception("fail"));

            IActionResult result = await _controller.RestoreEntry(id);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.DeletedEntries)));
        }

        // ──────────────────────────────────────────────
        // DeleteEntry (hard)
        // ──────────────────────────────────────────────

        [Test]
        public async Task DeleteEntry_WithEmptyGuid_ReturnsNotFound()
        {
            IActionResult result = await _controller.DeleteEntry(Guid.Empty);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        [Test]
        public async Task DeleteEntry_WithValidId_RedirectsToDeletedEntries_AndSetsSuccessTempData()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.HardDeleteEntryAsync(id))
                .Returns(Task.CompletedTask);

            IActionResult result = await _controller.DeleteEntry(id);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.DeletedEntries)));
            Assert.That(_controller.TempData["SuccessMessage"], Is.Not.Null);
        }

        [Test]
        public async Task DeleteEntry_OnInvalidOperationException_ReturnsBadRequest()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.HardDeleteEntryAsync(id))
                .ThrowsAsync(new InvalidOperationException());

            IActionResult result = await _controller.DeleteEntry(id);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
            Assert.That(_controller.TempData["ErrorMessage"], Is.Not.Null);
        }

        [Test]
        public async Task DeleteEntry_OnGeneralException_RedirectsToIndex()
        {
            var id = Guid.NewGuid();
            _leaderboardServiceMock
                .Setup(s => s.HardDeleteEntryAsync(id))
                .ThrowsAsync(new Exception("unexpected"));

            IActionResult result = await _controller.DeleteEntry(id);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(LeaderboardController.DeletedEntries)));
        }
    }
}
