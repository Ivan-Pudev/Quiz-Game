namespace QuizGame.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.ViewModels;
    using System.Collections.Generic;

    public class QuizzesController : Controller
    {
        private readonly IQuizzesService _quizzesService;

        public QuizzesController(IQuizzesService quizzesService)
        {
            _quizzesService = quizzesService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Quiz> quizzes = await _quizzesService.GetAllQuizzesAsync();

            return View(quizzes);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateQuizViewModel quiz = await _quizzesService.CreateQuizFormAsync();

            return View(quiz);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateQuizViewModel quizViewModel)
        {
            IEnumerable<Question> allQuestions = await _quizzesService.GetAllQuestionsAsync();

            if (!ModelState.IsValid)
            {
                return View(quizViewModel);
            }

            try
            {
                await _quizzesService.CreateQuizAsync(quizViewModel);
            }
            catch (Exception)
            {
                return View("Error");
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            Quiz? currentQuiz = await _quizzesService.GetQuizWithQuestionsByIdAsync(id);

            if (currentQuiz == null)
            {
                return NotFound();
            }

            DetailsQuizViewModel quizViewModel = _quizzesService.ShowQuizDetails(currentQuiz);

            return View(quizViewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            Quiz? currentQuiz = await _quizzesService.GetQuizWithQuestionsByIdAsync(id);

            if (currentQuiz == null)
            {
                return NotFound();
            }

            EditQuizViewModel inputModel = await _quizzesService.EditQuizGetDataFromForm(currentQuiz);

            return View(inputModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(int id,EditQuizViewModel quizViewModel)
        {
            if (id != quizViewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(quizViewModel);
            }

            Quiz? selectedQuiz = await _quizzesService.GetQuizWithQuestionsByIdAsync(quizViewModel.Id);

            if (selectedQuiz == null)
            {
                return NotFound();
            }

            try
            {
                await _quizzesService.UpdateQuizAsync(quizViewModel,selectedQuiz);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Quiz? quizToDelete = await _quizzesService.GetQuizWithQuestionsByIdAsync(id);

            if (quizToDelete != null)
            {
               await _quizzesService.DeleteQuizAsync(quizToDelete);
            }

            return RedirectToAction(nameof(Index));
        }

        //public async Task<IActionResult> Leaderboard()
        //{

        //}
    }
}
