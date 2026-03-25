namespace QuizGame.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Leaderboards;
    using QuizGame.ViewModels.Quizzes;
    using System.Collections.Generic;

    public class QuizController : BaseController
    {
        private readonly IQuizService _quizzesService;
        private readonly ILeaderboardService _leaderboardsService;

        public QuizController(IQuizService quizzesService, ILeaderboardService leaderboardsService)
        {
            _quizzesService = quizzesService;
            _leaderboardsService = leaderboardsService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<Quiz> quizzes = await _quizzesService.GetAllQuizzesAsync();
                return View(quizzes);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load quizzes";
                return View(Enumerable.Empty<Quiz>());
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                CreateQuizViewModel quiz = await _quizzesService.CreateQuizFormAsync();

                return View(quiz);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to create quiz";
                return View(new CreateQuizViewModel());
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateQuizViewModel quizViewModel)
        {
            try
            {
                await _quizzesService.CreateQuizAsync(quizViewModel);
                TempData["Success"] = "Created quiz successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to create quiz.";

                quizViewModel.Questions = (await _quizzesService.GetAllQuestionsAsync()).ToList();
                return View(quizViewModel);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                Quiz? currentQuiz = await _quizzesService.GetQuizByIdAsync(id);
                if (currentQuiz == null) return NotFound();

                Leaderboard? leaderboard = await _leaderboardsService.GetLeaderboardByQuizIdAsync(currentQuiz.Id);
                if (leaderboard == null)
                    leaderboard = await _quizzesService.CreateLeaderboardAsync(currentQuiz.Id);

                DetailsQuizViewModel viewModel = _quizzesService.ShowQuizDetails(currentQuiz);
                viewModel.LeaderboardId = leaderboard.Id;

                return View(viewModel);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load page.";
                return RedirectToAction(nameof(Index));
            }
            
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            try
            {
                Quiz? currentQuiz = await _quizzesService.GetQuizByIdAsync(id);

                if (currentQuiz == null)
                {
                    return NotFound();
                }

                EditQuizViewModel viewModel = await _quizzesService.EditQuizGetDataFromForm(currentQuiz);

                return View(viewModel);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to open edit page.";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditQuizViewModel quizViewModel)
        {
            try
            {
                if (id != quizViewModel.Id) return NotFound();

                if (!ModelState.IsValid)
                {
                    Quiz? quiz = await _quizzesService.GetQuizByIdAsync(id);
                    if (quiz == null) return NotFound();

                    quizViewModel = await _quizzesService.EditQuizGetDataFromForm(quiz);
                    return View(quizViewModel);
                }

                List<int> selectedIds = quizViewModel.SelectedQuestions
                    .Where(q => q.IsSelected)
                    .Select(q => q.QuestionId)
                    .ToList();

                await _quizzesService.EditQuizAsync(quizViewModel, selectedIds);

                TempData["Success"] = "Updated quiz successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to update quiz.";

                var quiz = await _quizzesService.GetQuizByIdAsync(id);
                if (quiz != null)
                    quizViewModel = await _quizzesService.EditQuizGetDataFromForm(quiz);

                return View(quizViewModel);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Invalid quiz id.";
                    return RedirectToAction(nameof(Index));
                }

                await _quizzesService.DeleteQuizAsync(id);
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Leaderboard(int id)
        {
            try
            {
                IEnumerable<LeaderboardRowVm>? rows = await _leaderboardsService.GetLeaderboardEntriesByQuizIdAsync(id);

                ViewBag.QuizId = id;
                return View(rows);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load leaderboard for this quiz";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
