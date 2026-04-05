namespace QuizGame.Core.Contracts
{
    using QuizGame.Data.Models;
    using QuizGame.ViewModels.Admin.Leaderboard;
    using QuizGame.ViewModels.Leaderboards;
    using System.Collections.Generic;

    public interface ILeaderboardService
    {
        Task<IEnumerable<Leaderboard>> GetLeaderboardsAsync();

        Task<IEnumerable<LeaderboardRowVm>?> GetLeaderboardEntriesByQuizIdAsync(Guid? quizId);

        Task<Leaderboard?> GetLeaderboardByQuizIdAsync(Guid id);

        Task<IEnumerable<AdminLeaderboardViewModel>> GetLeaderboardsToManageAsync();

        Task<IEnumerable<AdminLeaderboardEntryViewModel>> GetLeaderboardsEntriesToManageAsync();

        Task<AdminManageEntriesViewModel> GetLeaderboardsEntriesToManageDetailsAsync(Guid id);

        Task<AdminGlobalLeaderboardViewModel> GetGlobalLeaderboardAsync();

        Task<bool> UpdateEntryAsync(Guid userId, int score);

        Task<bool> RestoreEntryAsync(Guid entryId);

        Task<bool> SoftDeleteEntryAsync(Guid entryId);

        Task<bool> HardDeleteEntryAsync(Guid entryId);
        
    }
}
