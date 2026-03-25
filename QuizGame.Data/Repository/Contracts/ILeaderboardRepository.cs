namespace QuizGame.Data.Repository.Contracts
{
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Leaderboards;
    using System.Collections.Generic;

    public interface ILeaderboardRepository
    {
        Task<Leaderboard?> GetLeaderboardWithEntriesAndUserByQuizIdAsync(Guid quizId);

        Task<IEnumerable<Leaderboard>> GetLeaderboardsWithQuizzesAsync();

        Task<Leaderboard?> GetLeaderboardsWithEntriesByQuizIdAsync(Guid quizId);

        Task<IEnumerable<LeaderboardEntry>> GetLeaderboardEntriesByIdAsync(Guid? id);

        Task<List<LeaderboardEntry>> GetLeaderboardEntriesOrderedByScoreByLeaderboardIdAsync
            (Guid leaderboardId);

        Task<LeaderboardEntry?> GetLeaderboardEntryForUserByIdAsync
            (Guid leaderboardId, Guid userId);

        Task<bool> AddLeaderboardAsync(Leaderboard leaderboard);

        Task<bool> AddLeaderboardEntryAsync(LeaderboardEntry leaderboardEntry);

        Task<bool> UpdateLeaderboardEntriesAsync(LeaderboardEntry leaderboardEntry);
    }
}
