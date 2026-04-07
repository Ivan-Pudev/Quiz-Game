using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuizGame.Areas.Admin.Controllers;
using QuizGame.Core.Contracts;
using QuizGame.ViewModels.Admin.User;

namespace QuizGame.Services.Tests.Controllers
{
    [TestFixture]
    public class UserManagementControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<ILogger<UserManagementController>> _loggerMock;
        private UserManagementController _controller;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<UserManagementController>>();
            _controller = new UserManagementController(_userServiceMock.Object, _loggerMock.Object);

            // Provide TempData so controller can write to it without NullReferenceException
            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());
        }
        [TearDown]
        public void DisposeData()
        {
            _controller.Dispose();
        }
        // ───────────────────────────── Index ─────────────────────────────

        [Test]
        public async Task Index_ReturnsViewWithUsers_WhenServiceSucceeds()
        {
            var users = new List<AdminUserViewModel> { new AdminUserViewModel() };
            _userServiceMock
                .Setup(s => s.GetAllUsersAsync(false))
                .ReturnsAsync(users);

            var result = await _controller.Index();

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(users));
        }

        [Test]
        public async Task Index_RedirectsToIndex_WhenServiceThrows()
        {
            _userServiceMock
                .Setup(s => s.GetAllUsersAsync(false))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _controller.Index();

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        // ───────────────────────────── Create GET ─────────────────────────────

        [Test]
        public async Task Create_Get_ReturnsView()
        {
            var result = _controller.Create();

            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        // ───────────────────────────── Create POST ─────────────────────────────

        [Test]
        public async Task Create_Post_RedirectsToIndex_WhenModelValid()
        {
            var model = new AdminCreateUserViewModel();
            _userServiceMock
                .Setup(s => s.CreateUserAsync(model))
                .Returns(Task.CompletedTask);

            var result = await _controller.Create(model);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        [Test]
        public async Task Create_Post_ReturnsView_WhenModelStateInvalid()
        {
            _controller.ModelState.AddModelError("Username", "Required");
            var model = new AdminCreateUserViewModel();

            var result = await _controller.Create(model);

            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task Create_Post_ReturnsBadRequest_WhenInvalidOperationExceptionThrown()
        {
            var model = new AdminCreateUserViewModel();
            _userServiceMock
                .Setup(s => s.CreateUserAsync(model))
                .ThrowsAsync(new InvalidOperationException());

            var result = await _controller.Create(model);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Create_Post_RedirectsToIndex_WhenGenericExceptionThrown()
        {
            var model = new AdminCreateUserViewModel();
            _userServiceMock
                .Setup(s => s.CreateUserAsync(model))
                .ThrowsAsync(new Exception("unexpected"));

            var result = await _controller.Create(model);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        // ───────────────────────────── Edit ─────────────────────────────

        [Test]
        public async Task Edit_ReturnsNotFound_WhenUserIdIsEmpty()
        {
            var result = await _controller.Edit(Guid.Empty);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_ReturnsView_WhenUserFound()
        {
            var userId = Guid.NewGuid();
            var viewModel = new AdminManageUserRolesViewModel();
            _userServiceMock
                .Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(viewModel);

            var result = await _controller.Edit(userId);

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(viewModel));
        }

        [Test]
        public async Task Edit_RedirectsToIndex_WhenServiceThrows()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.GetUserByIdAsync(userId))
                .ThrowsAsync(new Exception("error"));

            var result = await _controller.Edit(userId);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        // ───────────────────────────── AssignRole ─────────────────────────────

        [Test]
        public async Task AssignRole_ReturnsNotFound_WhenUserIdIsEmpty()
        {
            var result = await _controller.AssignRole(Guid.Empty, "Admin");

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task AssignRole_RedirectsToIndex_WhenSuccessful()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.AssignRoleToUserAsync(userId, "Admin"))
                .Returns(Task.FromResult(true));

            var result = await _controller.AssignRole(userId, "Admin");

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        [Test]
        public async Task AssignRole_ReturnsBadRequest_WhenInvalidOperationExceptionThrown()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.AssignRoleToUserAsync(userId, "Admin"))
                .ThrowsAsync(new InvalidOperationException());

            var result = await _controller.AssignRole(userId, "Admin");

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task AssignRole_RedirectsToIndex_WhenGenericExceptionThrown()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.AssignRoleToUserAsync(userId, "Admin"))
                .ThrowsAsync(new Exception("unexpected"));

            var result = await _controller.AssignRole(userId, "Admin");

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        // ───────────────────────────── RemoveRole ─────────────────────────────

        [Test]
        public async Task RemoveRole_ReturnsNotFound_WhenUserIdIsEmpty()
        {
            var result = await _controller.RemoveRole(Guid.Empty, "Admin");

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task RemoveRole_RedirectsToIndex_WhenSuccessful()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.RemoveRoleFromUserAsync(userId, "Admin"))
                .Returns(Task.CompletedTask);

            var result = await _controller.RemoveRole(userId, "Admin");

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        [Test]
        public async Task RemoveRole_ReturnsBadRequest_WhenInvalidOperationExceptionThrown()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.RemoveRoleFromUserAsync(userId, "Admin"))
                .ThrowsAsync(new InvalidOperationException());

            var result = await _controller.RemoveRole(userId, "Admin");

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        // ───────────────────────────── Delete (Soft) ─────────────────────────────

        [Test]
        public async Task Delete_ReturnsNotFound_WhenUserIdIsEmpty()
        {
            var result = await _controller.Delete(Guid.Empty);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Delete_RedirectsToHomeIndex_WhenSuccessful()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.SoftDeleteUserAsync(userId))
                .Returns(Task.CompletedTask);

            var result = await _controller.Delete(userId);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Delete_ReturnsBadRequest_WhenInvalidOperationExceptionThrown()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.SoftDeleteUserAsync(userId))
                .ThrowsAsync(new InvalidOperationException());

            var result = await _controller.Delete(userId);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        // ───────────────────────────── DeletedAccounts ─────────────────────────────

        [Test]
        public async Task DeletedAccounts_ReturnsViewWithDeletedUsers()
        {
            var users = new List<AdminUserViewModel> { new AdminUserViewModel() };
            _userServiceMock
                .Setup(s => s.GetAllUsersAsync(true))
                .ReturnsAsync(users);

            var result = await _controller.DeletedAccounts();

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(users));
        }

        [Test]
        public async Task DeletedAccounts_RedirectsToIndex_WhenServiceThrows()
        {
            _userServiceMock
                .Setup(s => s.GetAllUsersAsync(true))
                .ThrowsAsync(new Exception("error"));

            var result = await _controller.DeletedAccounts();

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        // ───────────────────────────── RestoreAccount ─────────────────────────────

        [Test]
        public async Task RestoreAccount_ReturnsNotFound_WhenUserIdIsEmpty()
        {
            var result = await _controller.RestoreAccount(Guid.Empty);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task RestoreAccount_RedirectsToDeletedAccounts_WhenSuccessful()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.RestoreUserAsync(userId))
                .Returns(Task.CompletedTask);

            var result = await _controller.RestoreAccount(userId);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(_controller.DeletedAccounts)));
        }

        [Test]
        public async Task RestoreAccount_ReturnsBadRequest_WhenInvalidOperationExceptionThrown()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.RestoreUserAsync(userId))
                .ThrowsAsync(new InvalidOperationException());

            var result = await _controller.RestoreAccount(userId);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task RestoreAccount_RedirectsToDeletedAccounts_WhenGenericExceptionThrown()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.RestoreUserAsync(userId))
                .ThrowsAsync(new Exception("unexpected"));

            var result = await _controller.RestoreAccount(userId);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(_controller.DeletedAccounts)));
        }

        // ───────────────────────────── DeleteAccount (Hard) ─────────────────────────────

        [Test]
        public async Task DeleteAccount_ReturnsNotFound_WhenUserIdIsEmpty()
        {
            var result = await _controller.DeleteAccount(Guid.Empty);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteAccount_RedirectsToDeletedAccounts_WhenSuccessful()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.HardDeleteUserAsync(userId))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteAccount(userId);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(_controller.DeletedAccounts)));
        }

        [Test]
        public async Task DeleteAccount_ReturnsBadRequest_WhenInvalidOperationExceptionThrown()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.HardDeleteUserAsync(userId))
                .ThrowsAsync(new InvalidOperationException());

            var result = await _controller.DeleteAccount(userId);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task DeleteAccount_RedirectsToDeletedAccounts_WhenGenericExceptionThrown()
        {
            var userId = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.HardDeleteUserAsync(userId))
                .ThrowsAsync(new Exception("unexpected"));

            var result = await _controller.DeleteAccount(userId);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect!.ActionName, Is.EqualTo(nameof(_controller.DeletedAccounts)));
        }
    }
}
