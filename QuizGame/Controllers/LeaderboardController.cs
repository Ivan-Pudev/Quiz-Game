namespace QuizGame.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
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
        public async Task<IActionResult> Details(Guid? id)
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
    }
}
