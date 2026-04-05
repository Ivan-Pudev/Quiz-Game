namespace QuizGame.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Leaderboards;
    using QuizGame.ViewModels.Quizzes;
    using System.Collections.Generic;
    using static GCommon.OutputMessages.ErrorMessages;
    using static GCommon.OutputMessages.SuccessMessages;

    public class QuizController : BaseController
    {
        private readonly IQuizService _quizService;
        private readonly ILeaderboardService _leaderboardService;
        private readonly ILogger<QuizController> _logger;

        public QuizController(IQuizService quizService, ILeaderboardService leaderboardService,
            ILogger<QuizController> logger)
        {
            _quizService = quizService;
            _leaderboardService = leaderboardService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<Quiz> quizzes = await _quizService.GetAllQuizzesAsync();
                return View(quizzes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorLoad, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorLoad, nameof(Quiz));
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                CreateQuizViewModel quiz = await _quizService.CreateQuizFormAsync();

                return View(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorDisplayCreatePage, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorDisplayCreatePage, nameof(Quiz));
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateQuizViewModel quizViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(quizViewModel);
            }

            try
            {
                await _quizService.CreateQuizAsync(quizViewModel);
                TempData["SuccessMessage"] = string.Format(SuccessCreate, nameof(Quiz));
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorCreate, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorCreate, nameof(Quiz));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorCreate, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorCreate, nameof(Quiz));
                quizViewModel.Questions = (await _quizService.GetAllQuestionsAsync()).ToList();
                return View(quizViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(Quiz));
                return BadRequest();
            }

            try
            {
                Quiz? currentQuiz = await _quizService.GetQuizByIdAsync(id);

                if (currentQuiz == null)
                {
                    return NotFound();
                }

                DetailsQuizViewModel viewModel = _quizService.ShowQuizDetails(currentQuiz);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorLoadDetails, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorLoadDetails, nameof(Quiz));
                return RedirectToAction(nameof(Index));
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(Quiz));
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
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorDisplayEditPage, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorDisplayEditPage, nameof(Quiz));
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, EditQuizViewModel quizViewModel)
        {
            if (id != quizViewModel.Id)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(Quiz));
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(quizViewModel);
            }

            try
            {
                Quiz? quiz = await _quizService.GetQuizByIdAsync(id);

                if (quiz == null)
                {
                    return NotFound();
                }

                quizViewModel = await _quizService.EditQuizGetDataFromForm(quiz);

                List<Guid> selectedIds = quizViewModel.SelectedQuestions
                    .Where(q => q.IsSelected)
                    .Select(q => q.QuestionId)
                    .ToList();

                await _quizService.EditQuizAsync(quizViewModel, selectedIds);

                TempData["SuccessMessage"] = string.Format(SuccessUpdate,nameof(Quiz));
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorUpdate, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorUpdate,nameof(Quiz));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorUpdate, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorUpdate, nameof(Quiz));

                return View(quizViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId,nameof(Quiz));
                return BadRequest();
            }

            try
            {
                await _quizService.SoftDeleteQuizAsync(id);

                TempData["SuccessMessage"] = string.Format(SuccessSoftDelete,nameof(Quiz));
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorSoftDelete, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorSoftDelete, nameof(Quiz));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorSoftDelete, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorSoftDelete, nameof(Quiz));
                
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Leaderboard(Guid id)
        {
            try
            {
                IEnumerable<LeaderboardRowVm>? rows = await _leaderboardService.GetLeaderboardEntriesByQuizIdAsync(id);

                return View(rows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorLoadLeaderboard));
                TempData["ErrorMessage"] = string.Format(ErrorLoadLeaderboard);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeletedQuizzes()
        {
            try
            {
                IEnumerable<DetailsQuizViewModel> quizzes = await _quizService.GetAllDeletedQuizzesAsync();

                return View(quizzes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorLoadDeletedList, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorLoadDeletedList, nameof(Quiz));
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RestoreQuiz([FromRoute(Name = "id")] Guid userId)
        {
            try
            {
                await _quizService.RestoreQuizAsync(userId);

                return RedirectToAction(nameof(DeletedQuizzes));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorRestore, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorRestore, nameof(Quiz));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorRestore, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorRestore, nameof(Quiz));
                return RedirectToAction(nameof(DeletedQuizzes));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuiz([FromRoute(Name = "id")] Guid userId)
        {
            try
            {
                await _quizService
                    .HardDeleteQuizAsync(userId);

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorDelete, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorDelete, nameof(Quiz));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorDelete, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorDelete, nameof(Quiz));
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
