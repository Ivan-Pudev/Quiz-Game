namespace QuizGame.Core.Contracts
{
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Leaderboards;
    using QuizGame.ViewModels.Quizzes;
    using System.Collections.Generic;
    public interface IQuizService
    {
        Task<Quiz?> GetQuizByIdAsync(int id);

        Task<IEnumerable<Quiz>> GetAllQuizzesAsync();

        Task<IEnumerable<Question>> GetAllQuestionsAsync();

        Task<CreateQuizViewModel> CreateQuizFormAsync();

        Task CreateQuizAsync(CreateQuizViewModel viewModel);

        Task AddSelectedQuestions(Quiz selectedQuiz, List<int> selectedQuestionsIds);

        DetailsQuizViewModel ShowQuizDetails(Quiz quizModel);

        Task<EditQuizViewModel> EditQuizGetDataFromForm(Quiz quizModel);

        Task EditQuizAsync(EditQuizViewModel viewModel, List<int> selectedQuestionIds);

        Task DeleteQuizAsync(int id);

        Task<Leaderboard> CreateLeaderboardAsync(int quizId);

        Task SubmitScoreAsync(int quizId, string userId, int score);
    }
}
