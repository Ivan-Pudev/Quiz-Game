namespace QuizGame.Core
{
    using Microsoft.EntityFrameworkCore;
    using QuizGame.Core.Contracts;
    using QuizGame.Data;
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Game;
    using QuizGame.ViewModels.Leaderboards;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public class GameService : IGameService
    {
        private readonly QuizGameDbContext _dbContext;

        private readonly IQuizzesService _quizzesService;
        public GameService(QuizGameDbContext dbContext, IQuizzesService quizzesService)
        {
            _dbContext = dbContext;
            _quizzesService = quizzesService;
        }

        public async Task<int> StartAttemptAsync(int quizId, ClaimsPrincipal user)
        {
            string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("User not logged in");

            Quiz? quiz = await _quizzesService.GetQuizByIdAsync(quizId);

            if (quiz == null)
                throw new Exception("Quiz not found");

            var maxScore = quiz.Questions.Sum(q => q.Points);

            var attempt = new QuizAttempt
            {
                QuizId = quizId,
                UserId = userId,
                CurrentQuestionIndex = 0,
                Score = 0,
                MaxScore = maxScore,
                IsFinished = false
            };

            _dbContext.QuizAttempts.Add(attempt);
            await _dbContext.SaveChangesAsync();

            return attempt.Id;
        }

        public async Task<PlayQuestionViewModel?> GetCurrentQuestionAsync(int attemptId)
        {
            var attempt = await _dbContext.QuizAttempts
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(qn => qn.Answers)
                .FirstOrDefaultAsync(a => a.Id == attemptId);

            if (attempt == null) throw new Exception("Attempt not found");
            if (attempt.IsFinished) return null;

            var questions = attempt.Quiz.Questions
                .OrderBy(q => q.Id)
                .ToList();

            if (attempt.CurrentQuestionIndex >= questions.Count)
                return null;

            var qn = questions[attempt.CurrentQuestionIndex];

            return new PlayQuestionViewModel
            {
                AttemptId = attempt.Id,
                QuizId = attempt.QuizId,
                QuestionId = qn.Id,
                QuestionContent = qn.Content,
                Answers = qn.Answers.Select(a => new AnswerVm
                {
                    Id = a.Id,
                    Content = a.Content
                }).ToList(),
            };
        }

        public async Task SubmitAnswerAsync(int attemptId, int questionId, int selectedAnswerId)
        {
            var attempt = await _dbContext.QuizAttempts
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(qn => qn.Answers)
                .FirstOrDefaultAsync(a => a.Id == attemptId);

            if (attempt == null) throw new Exception("Attempt not found");
            if (attempt.IsFinished) return;

            var question = attempt.Quiz.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null) throw new Exception("Question not found in this quiz");

            var selected = question.Answers.FirstOrDefault(a => a.Id == selectedAnswerId);
            if (selected == null)
                throw new Exception($"Selected answer not found. questionId={questionId}, selectedAnswerId={selectedAnswerId}");

            var isCorrect = selected.IsCorrect;

            var earned = isCorrect ? question.Points : 0;

            attempt.Score += earned;

            _dbContext.AttemptAnswers.Add(new AttemptAnswer
            {
                QuizAttemptId = attempt.Id,
                QuestionId = questionId,
                SelectedAnswerId = selectedAnswerId,
                IsCorrect = isCorrect,
                EarnedPoints = earned
            });

            attempt.CurrentQuestionIndex += 1;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<GameSummaryViewModel> FinishAttemptAsync(int attemptId)
        {
            var attempt = await _dbContext.QuizAttempts
                .Include(a => a.Quiz)
                .Include(a => a.Answers)
                .FirstOrDefaultAsync(a => a.Id == attemptId)
                ?? throw new Exception("Attempt not found");

            if (!attempt.IsFinished)
            {
                attempt.IsFinished = true;
            }

            await _quizzesService.SubmitScoreAsync(attempt.QuizId, attempt.UserId, attempt.Score);
            await _dbContext.SaveChangesAsync();

            Leaderboard? leaderboard = await _quizzesService.GetLeaderboardByQuizIdAsync(attempt.QuizId);
            int leaderboardId = leaderboard.Id;

            return new GameSummaryViewModel
            {
                QuizId = attempt.QuizId,
                QuizTitle = attempt.Quiz.Title,
                Score = attempt.Score,
                MaxScore = attempt.MaxScore,
                CorrectAnswers = attempt.Answers.Count(a => a.IsCorrect),
                TotalQuestions = attempt.Answers.Count,
                LeaderboardId = leaderboardId
            };
        }
    }
}
