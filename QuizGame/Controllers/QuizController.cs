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
        private readonly IQuizService _quizService;
        private readonly ILeaderboardService _leaderboardService;

        public QuizController(IQuizService quizService, ILeaderboardService leaderboardService)
        {
            _quizService = quizService;
            _leaderboardService = leaderboardService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<Quiz> quizzes = await _quizService.GetAllQuizzesAsync();
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
                CreateQuizViewModel quiz = await _quizService.CreateQuizFormAsync();

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
                await _quizService.CreateQuizAsync(quizViewModel);
                TempData["Success"] = "Created quiz successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to create quiz.";

                quizViewModel.Questions = (await _quizService.GetAllQuestionsAsync()).ToList();
                return View(quizViewModel);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            try
            {
                Quiz? currentQuiz = await _quizService.GetQuizByIdAsync(id);
                if (currentQuiz == null) return NotFound();

                Leaderboard? leaderboard = await _leaderboardService.GetLeaderboardByQuizIdAsync(currentQuiz.Id);
                if (leaderboard == null)
                    leaderboard = await _quizService.CreateLeaderboardAsync(currentQuiz.Id);

                DetailsQuizViewModel viewModel = _quizService.ShowQuizDetails(currentQuiz);
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
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            try
            {
                Quiz? currentQuiz = await _quizService.GetQuizByIdAsync(id);

                if (currentQuiz == null)
                {
                    return NotFound();
                }

                EditQuizViewModel viewModel = await _quizService.EditQuizGetDataFromForm(currentQuiz);

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
        public async Task<IActionResult> Edit(Guid id, EditQuizViewModel quizViewModel)
        {
            try
            {
                if (id != quizViewModel.Id) return NotFound();

                if (!ModelState.IsValid)
                {
                    Quiz? quiz = await _quizService.GetQuizByIdAsync(id);
                    if (quiz == null) return NotFound();

                    quizViewModel = await _quizService.EditQuizGetDataFromForm(quiz);
                    return View(quizViewModel);
                }

                List<Guid> selectedIds = quizViewModel.SelectedQuestions
                    .Where(q => q.IsSelected)
                    .Select(q => q.QuestionId)
                    .ToList();

                await _quizService.EditQuizAsync(quizViewModel, selectedIds);

                TempData["Success"] = "Updated quiz successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to update quiz.";

                var quiz = await _quizService.GetQuizByIdAsync(id);
                if (quiz != null)
                    quizViewModel = await _quizService.EditQuizGetDataFromForm(quiz);

                return View(quizViewModel);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["Error"] = "Invalid quiz id.";
                    return RedirectToAction(nameof(Index));
                }

                await _quizService.DeleteQuizAsync(id);
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
        public async Task<IActionResult> Leaderboard(Guid id)
        {
            try
            {
                IEnumerable<LeaderboardRowVm>? rows = await _leaderboardService.GetLeaderboardEntriesByQuizIdAsync(id);

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
