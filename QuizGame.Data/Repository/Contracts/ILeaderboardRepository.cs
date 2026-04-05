namespace QuizGame.Data.Repository.Contracts
{
    using QuizGame.Data.Models;
    using System.Collections.Generic;

    public interface ILeaderboardRepository
    {
        Task<Leaderboard?> GetLeaderboardWithEntriesAndUserBydAsync(Guid leaderboardId);

        Task<Leaderboard?> GetLeaderboardWithEntriesAndUserByQuizIdAsync(Guid quizId);

        Task<IEnumerable<Leaderboard>> GetLeaderboardsWithQuizzesAsync();

        Task<Leaderboard?> GetLeaderboardsWithEntriesByQuizIdAsync(Guid quizId);

        Task<IEnumerable<LeaderboardEntry>> GetLeaderboardWithEntriesAndUserBydAsync(Guid? id);

        Task<List<LeaderboardEntry>> GetLeaderboardEntriesOrderedByScoreByLeaderboardIdAsync
            (Guid leaderboardId);

        Task<LeaderboardEntry?> GetLeaderboardEntryForUserByIdAsync
            (Guid leaderboardId, Guid userId);

        Task<LeaderboardEntry?> GetLeaderboardEntryByIdAsync(Guid? entryId);

        Task<IEnumerable<LeaderboardEntry>> GetLeaderboardsWithEntriesAsync();

        Task<IEnumerable<LeaderboardEntry>> GetLeaderboardsWithEntriesWithQuizAsync();

        Task<bool> AddLeaderboardAsync(Leaderboard leaderboard);

        Task<bool> AddLeaderboardEntryAsync(LeaderboardEntry leaderboardEntry);

        Task<bool> UpdateLeaderboardEntriesAsync(LeaderboardEntry leaderboardEntry);

        Task<bool> RestoreEntryAsync(Guid entryId);

        Task<bool> SoftDeleteEntryAsync(Guid entryId);

        Task<bool> HardDeleteEntryAsync(Guid entryId);

    }
}
