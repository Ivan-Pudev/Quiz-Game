namespace QuizGame.Core.Contracts
{
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Leaderboards;
    using System.Collections.Generic;

    public interface ILeaderboardService
    {
        Task<IEnumerable<Leaderboard>> GetLeaderboardsAsync();

        Task<IEnumerable<LeaderboardRowVm>?> GetLeaderboardEntriesByQuizIdAsync(Guid? quizId);
        Task<Leaderboard?> GetLeaderboardByQuizIdAsync(Guid id);
    }
}
