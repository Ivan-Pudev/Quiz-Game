namespace QuizGame.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using QuizGame.Core;
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Admin.Leaderboard;
    using QuizGame.ViewModels.Leaderboards;

    [Authorize]
    public class LeaderboardController : BaseController
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<Leaderboard> leaderboards = await _leaderboardService.GetLeaderboardsAsync();

                return View(leaderboards);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load leaderboards.";
                return View(Enumerable.Empty<Leaderboard>());
            }

        }

        [HttpGet]
        public async Task<IActionResult> Rankings(Guid? id)
        {
            try
            {
                IEnumerable<LeaderboardRowVm>? leaderboard = await _leaderboardService
                    .GetLeaderboardEntriesByQuizIdAsync(id);

                return View(leaderboard);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load leaderboard details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            try
            {
                IEnumerable<AdminLeaderboardViewModel> leaderboardViewModels = await _leaderboardService.GetLeaderboardsToManageAsync();
                return View(leaderboardViewModels);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load leaderboard details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageEntries(Guid id)
        {
            try
            {
                AdminManageEntriesViewModel leaderboardViewModel = await _leaderboardService
                    .GetLeaderboardsEntriesToManageDetailsAsync(id);
                return View(leaderboardViewModel);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load leaderboard entries.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEntry(Guid id, int newScore)
        {
            try
            {
                bool isUpdateSuccessful = await _leaderboardService.UpdateEntryAsync(id,newScore);

                if (!isUpdateSuccessful)
                {
                    throw new InvalidOperationException();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                TempData["Error"] = "Entry cannot be edited."; ;

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["Error"] = "Invalid entry id.";
                    return RedirectToAction(nameof(Details));
                }

                await _leaderboardService.SoftDeleteEntryAsync(id);
                TempData["Success"] = "Entry deleted successfully.";
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "Entry not found.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to delete entry.";
            }

            return RedirectToAction(nameof(Details));
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
            catch (Exception)
            {
                TempData["Error"] = "Unable to load global leaderboard.";
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
            catch (Exception)
            {
                TempData["Error"] = "Unable to load leaderboard details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RestoreEntry([FromRoute(Name = "id")] Guid userId)
        {
            try
            {
                bool isRestoreSuccessful = await _leaderboardService.RestoreEntryAsync(userId);

                if (!isRestoreSuccessful)
                {
                    throw new InvalidOperationException();
                }
                return RedirectToAction(nameof(DeletedEntries));
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                TempData["Error"] = "Quiz cannot be restored."; ;

                return RedirectToAction(nameof(DeletedEntries));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEntry([FromRoute(Name = "id")] Guid userId)
        {
            try
            {
                bool deleteResult = await _leaderboardService
                    .HardDeleteEntryAsync(userId);
                if (!deleteResult)
                {
                    TempData["Error"] = "Quiz can't be deleted.";

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                TempData["Error"] = "Quiz cannot be removed."; ;

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
