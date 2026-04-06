namespace QuizGame.Services.Tests.Services
{
    using Core;
    using Data.Models;
    using Data.Repository.Contracts;
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using ViewModels.Admin.User;

    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IPasswordHasher<ApplicationUser>> _passwordHasherMock;
        private UserService _userService;

        private Guid _userId;
        private ApplicationUser _user;
        private List<string> _roles = new List<string>();

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher<ApplicationUser>>();

            _userService = new UserService(
                _userRepoMock.Object,
                _passwordHasherMock.Object);

            _userId = Guid.NewGuid();
            _user = new ApplicationUser { Id = _userId, Email = "test@test.com" };
            _roles = new List<string> { "Admin", "User" };
        }

        [Test]
        public async Task GetUserByIdAsync_ShouldReturnCorrectViewModel()
        {
            var identityRoles = new List<IdentityRole<Guid>>
            {
                new IdentityRole<Guid> { Name = "Admin" }
            };

            _userRepoMock.Setup(r => r.FindUserByIdAsync(_userId))
                .ReturnsAsync(_user);

            _userRepoMock.Setup(r => r.GetUserRolesAsync(_user))
                .ReturnsAsync(_roles);

            _userRepoMock.Setup(r => r.GetAllRolesAsync(_user))
                .ReturnsAsync(identityRoles);

            var result = await _userService.GetUserByIdAsync(_userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(_userId));
            Assert.That(result.Email, Is.EqualTo("test@test.com"));

            Assert.That(result.Roles, Is.EquivalentTo(_roles));
            Assert.That(result.AvailableRoles, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task GetAllUsersAsync_ShouldMapUsersCorrectly()
        {
            var users = new List<ApplicationUser>
            {
                _user,
                new ApplicationUser { Id = Guid.NewGuid(), Email = "b@test.com" }
            };

            _userRepoMock.Setup(r => r.GetAllUsersAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(users);

            _userRepoMock.Setup(r => r.GetUserRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });

            var result = (await _userService.GetAllUsersAsync()).ToList();

            Assert.That(result, Has.Count.EqualTo(2));

            Assert.That(result.Select(u => u.Email),
                Is.EquivalentTo(new[] { "test@test.com", "b@test.com" }));

            Assert.That(result.All(u => u.Roles.Contains("User")), Is.True);

        }
        [Test]
        public async Task GetUserByIdAsync_ShouldReturnViewModel_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, Email = "test@test.com" };

            _userRepoMock.Setup(r => r.FindUserByIdAsync(userId))
                .ReturnsAsync(user);

            _userRepoMock.Setup(r => r.GetUserRolesAsync(It.Is<ApplicationUser>(u => u.Id == userId)))
                .ReturnsAsync(new List<string> { "Admin" });

            _userRepoMock.Setup(r => r.GetAllRolesAsync(It.Is<ApplicationUser>(u => u.Id == userId)))
                .ReturnsAsync(new List<IdentityRole<Guid>>());

            var result = await _userService.GetUserByIdAsync(userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(userId));
            Assert.That(result.Email, Is.EqualTo("test@test.com"));
            Assert.That(result.Roles, Contains.Item("Admin"));
        }

        [Test]
        public void GetUserByIdAsync_ShouldThrow_WhenUserNotFound()
        {
            _userRepoMock.Setup(r => r.FindUserByIdAsync(It.IsAny<Guid?>()))
                .ReturnsAsync((ApplicationUser?)null);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.GetUserByIdAsync(Guid.NewGuid()));

            Assert.That(ex, Is.Not.Null);
        }

        [Test]
        public async Task CreateUserAsync_ShouldAssignRoles_WhenRolesProvided()
        {
            var model = new AdminCreateUserViewModel
            {
                Email = "test@test.com",
                Password = "123456",
                ConfirmPassword = "123456",
                SelectedRoles = new List<string> { "Admin", "User" }
            };

            _passwordHasherMock
                .Setup(p => p.HashPassword(It.IsAny<ApplicationUser>(), model.Password))
                .Returns("hashed");

            _userRepoMock
                .Setup(r => r.AddUserAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(true);

            _userRepoMock
                .Setup(r => r.UpdateUserRoleAsync(It.IsAny<Guid?>(), It.IsAny<string>(), false))
                .ReturnsAsync(true);

            await _userService.CreateUserAsync(model);

            _userRepoMock.Verify(r => r.AddUserAsync(It.IsAny<ApplicationUser>()), Times.Once);

            _userRepoMock.Verify(r =>
                r.UpdateUserRoleAsync(It.IsAny<Guid?>(), "Admin", false), Times.Once);

            _userRepoMock.Verify(r =>
                r.UpdateUserRoleAsync(It.IsAny<Guid?>(), "User", false), Times.Once);
        }

        [Test]
        public void CreateUserAsync_ShouldThrow_WhenAddUserFails()
        {
            var model = new AdminCreateUserViewModel
            {
                Email = "test@test.com",
                Password = "123456",
                ConfirmPassword = "123456",
                SelectedRoles = new List<string>()
            };

            _passwordHasherMock
                .Setup(p => p.HashPassword(It.IsAny<ApplicationUser>(), model.Password))
                .Returns("hashed");

            _userRepoMock
                .Setup(r => r.AddUserAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(false);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.CreateUserAsync(model));

            Assert.That(ex, Is.Not.Null);
        }

        [Test]
        public async Task AssignRoleToUserAsync_ShouldReturnTrue_WhenSuccess()
        {
            _userRepoMock
                .Setup(r => r.UpdateUserRoleAsync(_userId, "Admin", false))
                .ReturnsAsync(true);

            var result = await _userService.AssignRoleToUserAsync(_userId, "Admin");

            Assert.That(result, Is.True);

            _userRepoMock.Verify(r =>
                r.UpdateUserRoleAsync(_userId, "Admin", false), Times.Once);
        }

        [TestCase("")]
        [TestCase(" ")]
        public void AssignRoleToUserAsync_ShouldThrow_WhenRoleInvalid(string role)
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.AssignRoleToUserAsync(Guid.NewGuid(), role));
        }

        [Test]
        public void AssignRoleToUserAsync_ShouldThrow_WhenRepoFails()
        {
            _userRepoMock
                .Setup(r => r.UpdateUserRoleAsync(It.IsAny<Guid?>(), "Admin", false))
                .ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.AssignRoleToUserAsync(Guid.NewGuid(), "Admin"));
        }

        [Test]
        public void CreateUserAsync_ShouldThrow_WhenRoleAssignmentFails()
        {
            var model = new AdminCreateUserViewModel
            {
                Email = "test@test.com",
                Password = "123456",
                ConfirmPassword = "123456",
                SelectedRoles = new List<string> { "Admin" }
            };

            _passwordHasherMock
                .Setup(p => p.HashPassword(It.IsAny<ApplicationUser>(), model.Password))
                .Returns("hashed");

            _userRepoMock
                .Setup(r => r.AddUserAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(true);

            _userRepoMock
                .Setup(r => r.UpdateUserRoleAsync(It.IsAny<Guid?>(), "Admin", false))
                .ReturnsAsync(false);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.CreateUserAsync(model));

            Assert.That(ex, Is.Not.Null);
        }

        [Test]
        public async Task CreateUserAsync_ShouldNotAssignRoles_WhenNoneSelected()
        {
            var model = new AdminCreateUserViewModel
            {
                Email = "test@test.com",
                Password = "123456",
                ConfirmPassword = "123456",
                SelectedRoles = new List<string>()
            };

            _passwordHasherMock
                .Setup(p => p.HashPassword(It.IsAny<ApplicationUser>(), model.Password))
                .Returns("hashed");

            _userRepoMock
                .Setup(r => r.AddUserAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(true);

            await _userService.CreateUserAsync(model);

            _userRepoMock.Verify(r =>
                    r.UpdateUserRoleAsync(It.IsAny<Guid?>(), It.IsAny<string>(), false),
                Times.Never);
        }

        [Test]
        public async Task RemoveRoleFromUserAsync_ShouldCallRepository()
        {
            _userRepoMock.Setup(r =>
                    r.UpdateUserRoleAsync(_userId, "Admin", true))
                .ReturnsAsync(true);

            await _userService.RemoveRoleFromUserAsync(_userId, "Admin");

            _userRepoMock.Verify(r =>
                r.UpdateUserRoleAsync(_userId, "Admin", true), Times.Once);
        }

        [TestCase("")]
        [TestCase(" ")]
        public void RemoveRoleFromUserAsync_ShouldThrow_WhenRoleInvalid(string role)
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.RemoveRoleFromUserAsync(Guid.NewGuid(), role));
        }

        [Test]
        public void RemoveRoleFromUserAsync_ShouldThrow_WhenRepoFails()
        {
            _userRepoMock
                .Setup(r => r.UpdateUserRoleAsync(It.IsAny<Guid?>(), "Admin", true))
                .ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.RemoveRoleFromUserAsync(Guid.NewGuid(), "Admin"));
        }

        [Test]
        public async Task RemoveRoleFromUserAsync_ShouldCallRepository_WithRemovingFlag()
        {
            var userId = Guid.NewGuid();

            _userRepoMock
                .Setup(r => r.UpdateUserRoleAsync(userId, "Admin", true))
                .ReturnsAsync(true);

            await _userService.RemoveRoleFromUserAsync(userId, "Admin");

            _userRepoMock.Verify(r =>
                r.UpdateUserRoleAsync(userId, "Admin", true), Times.Once);
        }

        [Test]
        public void SoftDeleteUserAsync_ShouldThrow_WhenRepositoryFails()
        {
            _userRepoMock.Setup(r => r.SoftDeleteUserAsync(It.IsAny<Guid?>()))
                .ReturnsAsync(false);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.SoftDeleteUserAsync(Guid.NewGuid()));

            Assert.That(ex, Is.Not.Null);
        }

        [Test]
        public async Task RestoreUserAsync_ShouldCallRepository_WhenSuccessful()
        {

            _userRepoMock
                .Setup(r => r.RestoreUserAsync(_userId))
                .ReturnsAsync(true);

            await _userService.RestoreUserAsync(_userId);

            _userRepoMock.Verify(r => r.RestoreUserAsync(_userId), Times.Once);
        }

        [Test]
        public void RestoreUserAsync_ShouldThrow_WhenRepositoryFails()
        {
            _userRepoMock
                .Setup(r => r.RestoreUserAsync(_userId))
                .ReturnsAsync(false);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.RestoreUserAsync(_userId));

            Assert.That(ex, Is.Not.Null);

            _userRepoMock.Verify(r => r.RestoreUserAsync(_userId), Times.Once);
        }

        [Test]
        public async Task HardDeleteUserAsync_ShouldCallRepository_WhenSuccessful()
        {
            var userId = Guid.NewGuid();

            _userRepoMock
                .Setup(r => r.HardDeleteUserAsync(userId))
                .ReturnsAsync(true);

            await _userService.HardDeleteUserAsync(userId);

            _userRepoMock.Verify(r => r.HardDeleteUserAsync(userId), Times.Once);
        }

        [Test]
        public void HardDeleteUserAsync_ShouldThrow_WhenRepositoryFails()
        {
            var userId = Guid.NewGuid();

            _userRepoMock
                .Setup(r => r.HardDeleteUserAsync(userId))
                .ReturnsAsync(false);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.HardDeleteUserAsync(userId));

            Assert.That(ex, Is.Not.Null);

            _userRepoMock.Verify(r => r.HardDeleteUserAsync(userId), Times.Once);
        }
    }
}
