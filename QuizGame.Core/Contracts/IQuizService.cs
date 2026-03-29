namespace QuizGame.Core.Contracts
{
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Quizzes;
    using System.Collections.Generic;
    public interface IQuizService
    {
        Task<Quiz?> GetQuizByIdAsync(Guid? id);

        Task<IEnumerable<Quiz>> GetAllQuizzesAsync();

        Task<IEnumerable<Question>> GetAllQuestionsAsync();

        Task<CreateQuizViewModel> CreateQuizFormAsync();

        Task CreateQuizAsync(CreateQuizViewModel viewModel);

        Task AddSelectedQuestions(Quiz selectedQuiz, List<Guid> selectedQuestionsIds);

        DetailsQuizViewModel ShowQuizDetails(Quiz quizModel);

        Task<EditQuizViewModel> EditQuizGetDataFromForm(Quiz quizModel);

        Task EditQuizAsync(EditQuizViewModel viewModel, List<Guid> selectedQuestionIds);

        Task DeleteQuizAsync(Guid id);

        Task<Leaderboard> CreateLeaderboardAsync(Guid quizId);

        Task SubmitScoreAsync(Guid quizId, Guid userId, int score);
    }
}
