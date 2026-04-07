namespace QuizGame.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Admin.User;
    using static GCommon.OutputMessages.ErrorMessages;
    using static GCommon.OutputMessages.SuccessMessages;

    public class UserManagementController : BaseAdminController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(IUserService userService,
            ILogger<UserManagementController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {

                IEnumerable<AdminUserViewModel> adminUserViewModels = await _userService
                    .GetAllUsersAsync(false);

                return View(adminUserViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorLoad, nameof(User)));
                TempData["ErrorMessage"] = string.Format(ErrorLoad, nameof(User));
                return RedirectToAction(nameof(Index));
            }

        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorDisplayCreatePage, nameof(User)));
                TempData["ErrorMessage"] = string.Format(ErrorDisplayCreatePage, nameof(User));

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminCreateUserViewModel quizViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(quizViewModel);
            }

            try
            {
                await _userService.CreateUserAsync(quizViewModel);
                TempData["SuccessMessage"] = string.Format(SuccessCreate, nameof(User));

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorCreate, nameof(User)));
                TempData["ErrorMessage"] = string.Format(ErrorCreate, nameof(User));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorCreate, nameof(User)));
                TempData["ErrorMessage"] = string.Format(ErrorCreate, nameof(User));
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute(Name = "id")] Guid? userId)
        {
            if (userId == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(User));
                return NotFound();
            }

            try
            {
                AdminManageUserRolesViewModel? viewModel = await _userService.GetUserByIdAsync(userId);

                return View(viewModel);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorDisplayEditPage, nameof(User)));
                TempData["ErrorMessage"] = string.Format(ErrorDisplayEditPage, nameof(User));
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole([FromRoute(Name = "id")] Guid? userId, string role)
        {
            if (userId == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(User));
                return NotFound();
            }
            try
            {
                await _userService
                    .AssignRoleToUserAsync(userId, role);

                TempData["SuccessMessage"] = string.Format(SuccessAssignRole);

                return RedirectToAction(nameof(Index));

            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorAssignRole));
                TempData["ErrorMessage"] = string.Format(ErrorAssignRole);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorAssignRole));
                TempData["ErrorMessage"] = string.Format(ErrorAssignRole);

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole([FromRoute(Name = "id")] Guid? userId, string role)
        {
            if (userId == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(User));
                return NotFound();
            }
            try
            {
                await _userService
                    .RemoveRoleFromUserAsync(userId, role);

                TempData["SuccessMessage"] = string.Format(SuccessRemoveRole);

                return RedirectToAction(nameof(Index));

            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorRemoveRole));
                TempData["ErrorMessage"] = string.Format(ErrorRemoveRole);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorRemoveRole));
                TempData["ErrorMessage"] = string.Format(ErrorRemoveRole);

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute(Name = "id")] Guid? userId)
        {
            if (userId == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(User));
                return NotFound();
            }
            try
            {
                await _userService
                    .SoftDeleteUserAsync(userId);

                TempData["SuccessMessage"] = string.Format(SuccessSoftDelete, nameof(User));

                return RedirectToAction(nameof(Index));

            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorSoftDelete, nameof(User)));
                TempData["ErrorMessage"] = string.Format(ErrorSoftDelete, nameof(User));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorSoftDelete, nameof(User)));
                TempData["ErrorMessage"] = string.Format(ErrorSoftDelete, nameof(User));

                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeletedAccounts()
        {
            try
            {
                IEnumerable<AdminUserViewModel> users = await _userService.GetAllUsersAsync(true);

                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorLoadDeletedList, nameof(User)));
                TempData["ErrorMessage"] = string.Format(ErrorLoadDeletedList, nameof(User));

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RestoreAccount([FromRoute(Name = "id")] Guid? userId)
        {
            if (userId == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(User));
                return NotFound();
            }

            try
            {
                await _userService.RestoreUserAsync(userId);

                TempData["SuccessMessage"] = string.Format(SuccessRestore, nameof(User));
                return RedirectToAction(nameof(DeletedAccounts));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorRestore, nameof(User)));
                TempData["ErrorMessage"] = string.Format(ErrorUpdate, nameof(User));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorRestore, nameof(User)));
                TempData["ErrorMessage"] = string.Format(ErrorRestore, nameof(User));

                return RedirectToAction(nameof(DeletedAccounts));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount([FromRoute(Name = "id")] Guid? userId)
        {
            if (userId == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(User));
                return NotFound();
            }

            try
            {
                await _userService
                    .HardDeleteUserAsync(userId);

                TempData["SuccessMessage"] = string.Format(SuccessHardDelete, nameof(User));
                return RedirectToAction(nameof(DeletedAccounts));

            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorHardDelete, nameof(User)));
                TempData["ErrorMessage"] = string.Format(ErrorHardDelete, nameof(User));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorHardDelete, nameof(User)));
                TempData["ErrorMessage"] = string.Format(ErrorHardDelete, nameof(User));

                return RedirectToAction(nameof(DeletedAccounts));
            }
        }
    }
}
