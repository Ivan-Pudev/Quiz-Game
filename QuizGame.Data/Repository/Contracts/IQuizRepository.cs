namespace QuizGame.Data.Repository.Contracts
{
    using QuizGame.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IQuizRepository
    {
        Task<IEnumerable<Answer>> GetAllAnswers();

        Task<IEnumerable<Question>> GetAllQuestionsOrderByContentAsync();

        Task<IEnumerable<Question>> GetQuestionsFromTheirIdsAsync(List<int> selectedQuestionsIds);

        Task<IEnumerable<Quiz>> GetAllQuizzesWithQuestionAnswersCategoriesAndLeaderboardAsync();

        Task<Quiz?> GetQuizWithQuestionsAnswersCategoriesAndLeaderboardByIdAsync(int? id);

        Task<Quiz?> GetQuizWithQuestionsByIdAsync(int id);

        Task<Quiz?> GetQuizByIdAsync(int id);

        Task<bool> AddQuizAsync(Quiz quiz);

        Task<bool> UpdateQuizAsync(Quiz quiz);

        Task<bool> HardDeleteQuizAsync(Quiz quiz);
    }
}
