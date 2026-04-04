namespace QuizGame.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> RestoreEntry(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["Error"] = "Invalid quiz id.";
                    return RedirectToAction(nameof(Index));
                }

                //await _leaderboardService.DeleteQuizAsync(id);
                TempData["Success"] = "Quiz deleted successfully.";
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "Quiz not found.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to delete quiz.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEntry(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["Error"] = "Invalid entry id.";
                    return RedirectToAction(nameof(Index));
                }

                await _leaderboardService.HardDeleteEntryAsync(id);
                TempData["Success"] = "Quiz deleted successfully.";
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "Quiz not found.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to delete quiz.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
