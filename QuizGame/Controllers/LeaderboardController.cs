namespace QuizGame.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Admin.Leaderboard;
    using QuizGame.ViewModels.Leaderboards;
    using static GCommon.OutputMessages.ErrorMessages;
    using static GCommon.OutputMessages.SuccessMessages;

    [Authorize]
    public class LeaderboardController : BaseController
    {
        private readonly ILeaderboardService _leaderboardService;
        private readonly ILogger<LeaderboardController> _logger;

        public LeaderboardController(ILeaderboardService leaderboardService
            , ILogger<LeaderboardController> logger)
        {
            _leaderboardService = leaderboardService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<Leaderboard> leaderboards = await _leaderboardService.GetLeaderboardsAsync();

                return View(leaderboards);
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorLoad, nameof(Leaderboard)));
                TempData["ErrorMessage"] = string.Format(ErrorLoad, nameof(Leaderboard));
                return RedirectToAction(nameof(Index));
            }

        }

        [HttpGet]
        public async Task<IActionResult> Rankings(Guid? id)
        {
            if (id == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(Leaderboard));
                return NotFound();
            }
            try
            {
                IEnumerable<LeaderboardRowVm>? leaderboard = await _leaderboardService
                    .GetLeaderboardEntriesByQuizIdAsync(id);

                return View(leaderboard);
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorLoad, nameof(Rankings)));
                TempData["ErrorMessage"] = string.Format(ErrorLoad, nameof(Rankings));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorLoad, nameof(Rankings)));
                TempData["ErrorMessage"] = string.Format(ErrorLoad, nameof(Rankings));
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            try
            { 
                AdminLeaderboardPageViewModel leaderboardViewModels = await _leaderboardService.GetLeaderboardsToManageAsync();
                return View(leaderboardViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorLoadDetails, nameof(Leaderboard)));
                TempData["ErrorMessage"] = string.Format(ErrorLoadDetails, nameof(Leaderboard));
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageEntries(Guid? id)
        {
            if (id == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(Leaderboard));
                return NotFound();
            }

            try
            {
                AdminManageEntriesViewModel leaderboardViewModel = await _leaderboardService
                    .GetLeaderboardsEntriesToManageDetailsAsync(id);
                return View(leaderboardViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorDisplayEditPage, nameof(LeaderboardEntry)));
                TempData["ErrorMessage"] = string.Format(ErrorDisplayEditPage, nameof(LeaderboardEntry));
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEntry(Guid id, [FromRoute]int newScore)
        {
            if (id == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(LeaderboardEntry));
                return NotFound();
            }

            try
            {
                await _leaderboardService.UpdateEntryAsync(id, newScore);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorUpdate, nameof(LeaderboardEntry)));
                TempData["ErrorMessage"] = string.Format(ErrorUpdate, nameof(LeaderboardEntry));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorUpdate, nameof(LeaderboardEntry)));
                TempData["ErrorMessage"] = string.Format(ErrorUpdate, nameof(LeaderboardEntry));
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(LeaderboardEntry));
                return NotFound();
            }

            try
            {
                await _leaderboardService.SoftDeleteEntryAsync(id);

                TempData["SuccessMessage"] = string.Format(SuccessSoftDelete, nameof(LeaderboardEntry));
                return RedirectToAction(nameof(Details));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorSoftDelete,nameof(LeaderboardEntry)));
                TempData["ErrorMessage"] = string.Format(ErrorSoftDelete, nameof(LeaderboardEntry));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorSoftDelete, nameof(LeaderboardEntry)));
                TempData["ErrorMessage"] = string.Format(ErrorSoftDelete, nameof(LeaderboardEntry));
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GlobalLeaderboard()
        {
            try
            {
                AdminGlobalLeaderboardViewModel globalLeaderboard = await _leaderboardService
                    .GetGlobalLeaderboardAsync();
                return View(globalLeaderboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorLoad, nameof(Leaderboard)));
                TempData["ErrorMessage"] = string.Format(ErrorLoad, nameof(Leaderboard));
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeletedEntries()
        {
            try
            {
                IEnumerable<AdminLeaderboardEntryViewModel> leaderboardViewModels = await _leaderboardService
                    .GetLeaderboardsEntriesToManageAsync();
                return View(leaderboardViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorLoadDeletedList,nameof(LeaderboardEntry)));
                TempData["ErrorMessage"] = string.Format(ErrorLoadDeletedList, nameof(LeaderboardEntry));
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RestoreEntry(Guid? id)
        {
            if (id == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(LeaderboardEntry));
                return NotFound();
            }

            try
            {
                await _leaderboardService.RestoreEntryAsync(id);

                TempData["SuccessMessage"] = string.Format(SuccessRestore, nameof(LeaderboardEntry));
                return RedirectToAction(nameof(DeletedEntries));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorRestore,nameof(LeaderboardEntry)));
                TempData["ErrorMessage"] = string.Format(ErrorRestore, nameof(LeaderboardEntry));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorRestore, nameof(LeaderboardEntry)));
                TempData["ErrorMessage"] = string.Format(ErrorRestore, nameof(LeaderboardEntry));
                return RedirectToAction(nameof(DeletedEntries));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEntry(Guid? id)
        {
            if (id == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(LeaderboardEntry));
                return NotFound();
            }

            try
            {
                await _leaderboardService.HardDeleteEntryAsync(id);

                TempData["SuccessMessage"] = string.Format(SuccessHardDelete, nameof(LeaderboardEntry));
                return RedirectToAction(nameof(DeletedEntries));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorHardDelete,nameof(LeaderboardEntry)));
                TempData["ErrorMessage"] = string.Format(ErrorHardDelete, nameof(LeaderboardEntry));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorHardDelete, nameof(LeaderboardEntry)));
                TempData["ErrorMessage"] = string.Format(ErrorHardDelete, nameof(LeaderboardEntry));
                return RedirectToAction(nameof(DeletedEntries));
            }
        }
    }
}
