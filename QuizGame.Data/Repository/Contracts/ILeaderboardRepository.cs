namespace QuizGame.Data.Repository.Contracts
{
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Leaderboards;
    using System.Collections.Generic;

    public interface ILeaderboardRepository
    {
        Task<Leaderboard?> GetLeaderboardWithEntriesAndUserByQuizIdAsync(int quizId);

        Task<IEnumerable<Leaderboard>> GetLeaderboardsWithQuizzesAsync();

        Task<Leaderboard?> GetLeaderboardsWithEntriesByQuizIdAsync(int quizId);

        Task<IEnumerable<LeaderboardEntry>> GetLeaderboardEntriesByIdAsync(int id);

        Task<List<LeaderboardEntry>> GetLeaderboardEntriesOrderedByScoreByLeaderboardIdAsync(int leaderboardId);

        Task<LeaderboardEntry?> GetLeaderboardEntryForUserByIdAsync(int leaderboardId, string userId);

        Task<bool> AddLeaderboardAsync(Leaderboard leaderboard);

        Task<bool> AddLeaderboardEntryAsync(LeaderboardEntry leaderboardEntry);

        Task<bool> UpdateLeaderboardEntriesAsync(LeaderboardEntry leaderboardEntry);
    }
}
