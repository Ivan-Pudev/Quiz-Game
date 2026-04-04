using Microsoft.AspNetCore.Mvc;
using QuizGame.Core.Contracts;
using QuizGame.ViewModels.Admin.User;

namespace QuizGame.Areas.Admin.Controllers
{
    public class UserManagementController : BaseAdminController
    {
        private readonly IUserService _userService;

        public UserManagementController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = this.GetAdminUserId()!;

            IEnumerable<AdminUserViewModel> adminUserViewModels = await _userService
                .GetAllUsersAsync(userId,false);
            
            return View(adminUserViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to create user";
                return View(new AdminCreateUserViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminCreateUserViewModel quizViewModel)
        {
            try
            {
                await _userService.CreateUserAsync(quizViewModel);
                TempData["Success"] = "Created user successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to create user.";

                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute(Name = "id")] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            try
            {
                AdminManageUserRolesViewModel? viewModel = await _userService.GetUserByIdAsync(userId);

                return View(viewModel);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to open edit page.";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(Guid userId, string role)
        {
            try
            {
                bool assignRoleResult = await _userService
                    .AssignRoleToUserAsync(userId, role);
                if (!assignRoleResult)
                {
                    TempData["Success"] = "Role assigned successfully";

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                TempData["Error"] = "Role can't be assigned."; 

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(Guid userId, string role)
        {
            try
            {
                bool removeRoleResult = await _userService
                    .RemoveRoleFromUserAsync(userId, role);
                if (!removeRoleResult)
                {
                    TempData["Error"] = "Role cannot be removed.";

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                TempData["Error"] = "Role cannot be removed.";

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute(Name = "id")]Guid userId)
        {
            try
            {
                bool deleteResult = await _userService
                    .SoftDeleteUserAsync(userId);
                if (!deleteResult)
                {
                    TempData["Error"] = "User can't be deleted.";

                    return RedirectToAction("Index","Home");
                }
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                TempData["Error"] = "Role cannot be removed."; ;

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> DeletedAccounts()
        {
            string userId = GetAdminUserId()!;

            IEnumerable<AdminUserViewModel> users = await _userService.GetAllUsersAsync(userId,true);

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> RestoreAccount([FromRoute(Name = "id")] Guid userId)
        {
            try
            {
                bool isRestoreSuccessful = await _userService.RestoreUserAsync(userId);

                if (!isRestoreSuccessful)
                {
                    throw new InvalidOperationException();
                }
                return RedirectToAction(nameof(DeletedAccounts));
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                TempData["Error"] = "User cannot be restored."; ;

                return RedirectToAction(nameof(DeletedAccounts));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount([FromRoute(Name = "id")] Guid userId)
        {
            try
            {
                bool deleteResult = await _userService
                    .HardDeleteUserAsync(userId);
                if (!deleteResult)
                {
                    TempData["Error"] = "User can't be deleted.";

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                TempData["Error"] = "Role cannot be removed."; ;

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
