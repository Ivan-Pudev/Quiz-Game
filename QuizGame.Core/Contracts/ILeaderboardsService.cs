namespace QuizGame.Core.Contracts
{
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Leaderboards;
    using System.Collections.Generic;

    public interface ILeaderboardsService
    {
        Task<List<Leaderboard>> GetLeaderboardsAsync();

        Task<List<LeaderboardRowVm>> GetLeaderboardsEntriesByIdAsync(int id);
    }
}
