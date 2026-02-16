namespace QuizGame.Core.Contracts
{
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Game;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGameService
    {
        Task<int> StartAttemptAsync(int quizId, ClaimsPrincipal user);
        Task<PlayQuestionViewModel?> GetCurrentQuestionAsync(int attemptId);
        Task SubmitAnswerAsync(int quizId, int questionId, int score);
        Task<GameSummaryViewModel> FinishAttemptAsync(int attemptId);
    }
}
