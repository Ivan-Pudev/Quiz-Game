namespace QuizGame.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Leaderboards;

    public class LeaderboardController : Controller
    {
        private readonly ILeaderboardService _leaderboardsService;

        public LeaderboardController(ILeaderboardService leaderboardsService)
        {
            _leaderboardsService = leaderboardsService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<Leaderboard> leaderboards = await _leaderboardsService.GetLeaderboardsAsync();

                return View(leaderboards);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load leaderboards.";
                return View(Enumerable.Empty<Leaderboard>());
            }
            
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                IEnumerable<LeaderboardRowVm>? leaderboard = await _leaderboardsService
                    .GetLeaderboardEntriesByQuizIdAsync(id);

                return View(leaderboard);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load leaderboard details.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
