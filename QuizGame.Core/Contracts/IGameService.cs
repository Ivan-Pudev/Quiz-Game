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
        Task<Guid> StartAttemptAsync(Guid quizId, ClaimsPrincipal user);
        Task<PlayQuestionViewModel?> GetCurrentQuestionAsync(Guid attemptId);
        Task SubmitAnswerAsync(Guid quizId, Guid questionId, Guid selectedAnswerId);
        Task<GameSummaryViewModel> FinishAttemptAsync(Guid attemptId);
    }
}
