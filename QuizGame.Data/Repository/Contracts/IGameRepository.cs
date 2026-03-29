namespace QuizGame.Data.Repository.Contracts
{
    using QuizGame.Data.Models;
    public interface IGameRepository
    {
        Task<QuizAttempt?> GetQuizAttemptWithQuizAndAnswersByIdAsync(Guid attemptId);

        Task<QuizAttempt?> GetQuizAttemptWithQuizQuestionsAndAnswersByIdAsync(Guid attemptId);

        Task<bool> AddQuizAttemptAsync(QuizAttempt attempt);

        Task<bool> AddAttemptAnswerAsync(AttemptAnswer answer);

        Task<bool> UpdateAttempAnswersAsync(AttemptAnswer answer);
    }
}
