using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using QuizGame.Data;
using QuizGame.Data.Models;
using QuizGame.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QuizGame.Services.Tests.Repositories
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private QuizGameDbContext _dbContext;

        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<RoleManager<IdentityRole<Guid>>> _roleManagerMock;

        private UserRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<QuizGameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new QuizGameDbContext(options);


            _userManagerMock = MockUserManager();
            _roleManagerMock = MockRoleManager();

            _repository = new UserRepository(
                _dbContext,
                _userManagerMock.Object,
                _roleManagerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            if (_dbContext is IDisposable disposable1)
            {
                disposable1.Dispose();
            }

            if (_repository is IDisposable disposable2)
            {
                disposable2.Dispose();
            }
        }

        // ================= MOCK HELPERS =================

        private Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<RoleManager<IdentityRole<Guid>>> MockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole<Guid>>>();

            return new Mock<RoleManager<IdentityRole<Guid>>>(
                store.Object, null, null, null, null);
        }

        // ================= FIND USER =================

        [Test]
        public async Task FindUserByIdAsync_ReturnsUser()
        {
            var user = new ApplicationUser { Id = Guid.NewGuid() };

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            var result = await _repository.FindUserByIdAsync(user.Id);

            Assert.That(result, Is.EqualTo(user));
        }

        // ================= GET ALL USERS =================

        [Test]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            _dbContext.Users.AddRange(
                new ApplicationUser { Email = "b@test.com",FullName ="b@test" },
                new ApplicationUser { Email = "a@test.com",FullName = "a@test"}
            );
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetAllUsersAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Email, Is.EqualTo("a@test.com")); // ordered
        }

        [Test]
        public async Task GetAllUsersAsync_WithFilter_ReturnsFiltered()
        {
            _dbContext.Users.AddRange(
                new ApplicationUser { Email = "a@test.com", FullName = "a@test" },
                new ApplicationUser { Email = "b@test.com", FullName = "b@test" }
            );
            await _dbContext.SaveChangesAsync();

            Expression<Func<ApplicationUser, bool>> filter = u => u.Email.StartsWith("a");

            var result = await _repository.GetAllUsersAsync(filter);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        // ================= GET ROLES =================

        [Test]
        public async Task GetAllRolesAsync_ReturnsRoles()
        {
            _dbContext.Roles.AddRange(
                new IdentityRole<Guid> { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<Guid> { Name = "User", NormalizedName = "USER" }
            );

            await _dbContext.SaveChangesAsync();

            _roleManagerMock.Setup(r => r.Roles)
                .Returns(_dbContext.Roles);

            var result = await _repository.GetAllRolesAsync(new ApplicationUser());

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetUserRolesAsync_ReturnsUserRoles()
        {
            var user = new ApplicationUser();
            var roles = new List<string> { "Admin" };

            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(roles);

            var result = await _repository.GetUserRolesAsync(user);

            Assert.That(result, Is.EqualTo(roles));
        }


        [Test]
        public async Task AddUserAsync_ReturnsTrue_AndAddsUserToDb()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                UserName = "test@test.com",
                FullName = "test"
            };

            // Act
            var result = await _repository.AddUserAsync(user);

            // Assert
            Assert.That(result, Is.True);

            var usersInDb = await _dbContext.Users.ToListAsync();
            Assert.That(usersInDb.Count, Is.EqualTo(1));
            Assert.That(usersInDb.First().Email, Is.EqualTo("test@test.com"));
        }


        [Test]
        public async Task UpdateUserRoleAsync_UserNotFound_ReturnsFalse()
        {
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            var result = await _repository.UpdateUserRoleAsync(Guid.NewGuid(), "Admin");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateUserRoleAsync_RoleNotExists_ReturnsFalse()
        {
            var user = new ApplicationUser();

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _roleManagerMock.Setup(x => x.RoleExistsAsync("Admin"))
                .ReturnsAsync(false);

            var result = await _repository.UpdateUserRoleAsync(Guid.NewGuid(), "Admin");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateUserRoleAsync_AddRole_Success()
        {
            var user = new ApplicationUser();

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _roleManagerMock.Setup(x => x.RoleExistsAsync("Admin"))
                .ReturnsAsync(true);

            _userManagerMock.Setup(x => x.IsInRoleAsync(user, "Admin"))
                .ReturnsAsync(false);

            _userManagerMock.Setup(x => x.AddToRoleAsync(user, "Admin"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _repository.UpdateUserRoleAsync(Guid.NewGuid(), "Admin");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task UpdateUserRoleAsync_RemoveRole_Success()
        {
            var user = new ApplicationUser();

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _roleManagerMock.Setup(x => x.RoleExistsAsync("Admin"))
                .ReturnsAsync(true);

            _userManagerMock.Setup(x => x.IsInRoleAsync(user, "Admin"))
                .ReturnsAsync(true);

            _userManagerMock.Setup(x => x.RemoveFromRoleAsync(user, "Admin"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _repository.UpdateUserRoleAsync(Guid.NewGuid(), "Admin", true);

            Assert.That(result, Is.True);
        }

        // ================= SOFT DELETE =================

        [Test]
        public async Task SoftDeleteUserAsync_UserNotFound_ReturnsFalse()
        {
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            var result = await _repository.SoftDeleteUserAsync(Guid.NewGuid());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task SoftDeleteUserAsync_SetsDeletedFlag()
        {
            var user = new ApplicationUser { isDeleted = false };

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            var result = await _repository.SoftDeleteUserAsync(Guid.NewGuid());

            Assert.That(result, Is.False);
            Assert.That(user.isDeleted,Is.True);
        }

        // ================= RESTORE =================

        [Test]
        public async Task RestoreUserAsync_SetsDeletedFalse()
        {
            var user = new ApplicationUser { isDeleted = true, FullName = "user" };

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            var result = await _repository.RestoreUserAsync(Guid.NewGuid());

            Assert.That(result, Is.False);
            Assert.That(user.isDeleted, Is.False);
        }


        [Test]
        public async Task HardDeleteUserAsync_UserNotFound_ReturnsFalse()
        {
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            var result = await _repository.HardDeleteUserAsync(Guid.NewGuid());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HardDeleteUserAsync_Success_ReturnsTrue()
        {
            var user = new ApplicationUser();

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _repository.HardDeleteUserAsync(Guid.NewGuid());

            Assert.That(result, Is.True);
        }
    }
}