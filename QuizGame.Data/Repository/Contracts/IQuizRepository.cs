namespace QuizGame.Data.Repository.Contracts
{
    using QuizGame.Data.Models;
    using System;
    using System.Collections.Generic;

    public interface IQuizRepository
    {
        Task<IEnumerable<Question>> GetAllQuestionsOrderByContentAsync();

        Task<IEnumerable<Question>> GetQuestionsFromTheirIdsAsync(List<Guid> selectedIds);

        Task<IEnumerable<Quiz>> GetAllQuizzesWithQuestionAnswersCategoriesAndLeaderboardAsync();

        Task<Quiz?> GetQuizWithQuestionsAnswersCategoriesAndLeaderboardByIdAsync(Guid? id);

        Task<Quiz?> GetQuizWithQuestionsByIdAsync(Guid? id);

        Task<bool> AddQuizAsync(Quiz quiz);

        Task<bool> UpdateQuizAsync(Quiz quiz);

        Task<bool> HardDeleteQuizAsync(Quiz quiz);
    }
}
