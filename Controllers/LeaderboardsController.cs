namespace QuizGame.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Leaderboards;

    public class LeaderboardsController : Controller
    {
        private readonly ILeaderboardsService _leaderboardsService;

        public LeaderboardsController(ILeaderboardsService leaderboardsService)
        {
            _leaderboardsService = leaderboardsService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                List<Leaderboard> leaderboards = await _leaderboardsService.GetLeaderboardsAsync();

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
                List<LeaderboardRowVm>? leaderboard = await _leaderboardsService
                    .GetLeaderboardsEntriesByIdAsync(id);

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
