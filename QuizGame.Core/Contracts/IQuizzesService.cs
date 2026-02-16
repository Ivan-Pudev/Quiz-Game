namespace QuizGame.Core.Contracts
{
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Leaderboards;
    using QuizGame.ViewModels.Quizzes;
    using System.Collections.Generic;
    public interface IQuizzesService
    {
        Task<IEnumerable<Quiz>> GetAllQuizzesAsync();

        Task<Quiz?> GetQuizByIdAsync(int? id);

        Task<IEnumerable<Question>> GetAllQuestionsAsync();

        Task<List<Question>> GetQuestionsFromTheirIdsAsync(List<int> selectedQuestionsIds);

        Task<List<Answer>> GetAllAnswers();

        Task<List<LeaderboardRowVm>?> GetLeaderboardEntriesByQuizIdAsync(int id);

        Task<Leaderboard?> GetLeaderboardByQuizIdAsync(int quizId);

        Task<List<LeaderboardRowVm>?> GetLeaderboardEntriesByIdAsync(int id);

        Task<CreateQuizViewModel> CreateQuizFormAsync();

        Task CreateQuizAsync(CreateQuizViewModel viewModel);

        Task AddSelectedQuestions(Quiz selectedQuiz, List<int> selectedQuestionsIds);

        DetailsQuizViewModel ShowQuizDetails(Quiz quizModel);

        Task<EditQuizViewModel> EditQuizGetDataFromForm(Quiz quizModel);

        Task UpdateQuizAsync(EditQuizViewModel viewModel, List<int> selectedQuestionIds);

        Task DeleteQuizAsync(int id);

        Task<Leaderboard> CreateLeaderboardAsync(int quizId);

        Task SubmitScoreAsync(int quizId, string userId, int score);
        
    }
}
