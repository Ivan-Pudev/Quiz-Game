namespace QuizGame.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Game;
    using System.Collections.Generic;

    public class PlayController : BaseController 
    {
        private readonly IGameService _gameService;
        private readonly IQuizService _quizService;

        public PlayController(IGameService gameService, IQuizService quizService)
        {
            _gameService = gameService;
            _quizService = quizService;
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
            catch (Exception)
            {
                TempData["Error"] = "Unable to load quizzes to play.";
                return View(Enumerable.Empty<Quiz>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Start(Guid id) 
        {
            try
            {
                Guid attemptId = await _gameService.StartAttemptAsync(id, User);

                return RedirectToAction(nameof(Question), new { attemptId });
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to start quiz.";
                return RedirectToAction(nameof(Index));
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Question(Guid attemptId)
        {
            try
            {
                var vm = await _gameService.GetCurrentQuestionAsync(attemptId);

                if (vm == null)
                    return RedirectToAction(nameof(Finish), new { attemptId });

                
                return View(vm);
            }
            catch (Exception)
            {
                TempData["Error"] = "Question loaded incorrectly.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAnswer(Guid attemptId,Guid questionId,Guid selectedAnswerId)
        {
            try
            {
                if (selectedAnswerId == Guid.Empty)
                    return RedirectToAction(nameof(Question), new { attemptId });

                await _gameService.SubmitAnswerAsync(attemptId, questionId, selectedAnswerId);

                return RedirectToAction(nameof(Question), new { attemptId });
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Finish(Guid attemptId,Guid quizId,Guid userId)
        {
            try
            {
                GameSummaryViewModel summary = await _gameService.FinishAttemptAsync(attemptId);

                if (summary == null)
                {
                    TempData["Error"] = "Game summary could not be generated.";
                    return RedirectToAction(nameof(Index));
                }

                return View(summary);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to finish quiz.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

