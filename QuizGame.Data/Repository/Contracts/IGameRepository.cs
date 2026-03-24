namespace QuizGame.Data.Repository.Contracts
{
    using QuizGame.Data.Models;
    public interface IGameRepository
    {
        Task<QuizAttempt?> GetQuizAttemptWithQuizQuestionAndAnswersByIdAsync(int attemptId);

        Task<bool> AddQuizAttemptAsync(QuizAttempt attempt);

        Task<bool> AddAttemptAnswerAsync(AttemptAnswer answer);

        Task<bool> UpdateAttempAnswersAsync(AttemptAnswer answer);
    }
}
