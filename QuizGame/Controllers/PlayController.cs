namespace QuizGame.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Game;
    using System.Collections.Generic;
    using static GCommon.OutputMessages.ErrorMessages;
    using static GCommon.OutputMessages.SuccessMessages;
    public class PlayController : BaseController
    {
        private readonly IGameService _gameService;
        private readonly IQuizService _quizService;
        private readonly ILogger<PlayController> _logger;

        public PlayController(IGameService gameService, IQuizService quizService,
            ILogger<PlayController> logger)
        {
            _gameService = gameService;
            _quizService = quizService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<Quiz> allQuizzes = await _quizService.GetAllQuizzesAsync();
                return View(allQuizzes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorLoad, nameof(Quiz)));
                TempData["ErrorMessage"] = string.Format(ErrorLoad, nameof(Quiz));
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Start(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(Quiz));
                return NotFound();
            }
            try
            {
                Guid attemptId = await _gameService.StartAttemptAsync(id, User);

                return RedirectToAction(nameof(Question), new { attemptId });
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorStart));
                TempData["ErrorMessage"] = string.Format(ErrorStart);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorStart));
                TempData["ErrorMessage"] = string.Format(ErrorStart);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Question(Guid attemptId)
        {
            if (attemptId == Guid.Empty)
            {
                TempData["ErrorMessage"] = string.Format(ErrorInvalidId, nameof(QuizAttempt));
                return NotFound();
            }
            try
            {
                PlayQuestionViewModel? vm = await _gameService.GetCurrentQuestionAsync(attemptId);

                if (vm == null)
                {
                    TempData["ErrorMessage"] = string.Format(ErrorLoadQuestion);
                    return NotFound();
                }
                return RedirectToAction(nameof(Finish), new { attemptId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorLoadQuestion);
                TempData["Error"] = "Question loaded incorrectly.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAnswer(Guid attemptId, Guid questionId, Guid selectedAnswerId)
        {
            try
            {
                if (selectedAnswerId == Guid.Empty)
                    return RedirectToAction(nameof(Question), new { attemptId });

                await _gameService.SubmitAnswerAsync(attemptId, questionId, selectedAnswerId);

                return RedirectToAction(nameof(Question), new { attemptId });
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorStart));
                TempData["ErrorMessage"] = string.Format(ErrorStart);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorStart));
                TempData["ErrorMessage"] = string.Format(ErrorStart);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Finish(Guid attemptId, Guid quizId, Guid userId)
        {
            try
            {
                GameSummaryViewModel summary = await _gameService.FinishAttemptAsync(attemptId);

                if (summary == null)
                {
                    TempData["Error"] = string.Format(ErrorGenerateSummary);
                    return RedirectToAction(nameof(Index));
                }

                return View(summary);
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, string.Format(ErrorGenerateSummary));
                TempData["ErrorMessage"] = string.Format(ErrorStart);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format(ErrorFinishQuiz));
                TempData["ErrorMessage"] = string.Format(ErrorFinishQuiz);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

