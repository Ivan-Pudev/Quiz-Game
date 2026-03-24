using Microsoft.EntityFrameworkCore;
using QuizGame.Core.Contracts;
using QuizGame.Data;
using QuizGame.Data.Models;
using QuizGame.Data.Repository.Contracts;
using QuizGame.ViewModels.Leaderboards;

namespace QuizGame.Core
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ILeaderboardRepository _leaderboardRepository;

        public LeaderboardService(ILeaderboardRepository leaderboardRepository)
        {
            _leaderboardRepository = leaderboardRepository;
        }

        public Task<IEnumerable<Leaderboard>> GetLeaderboardsAsync()
        {
            return _leaderboardRepository.GetLeaderboardsWithQuizzesAsync();
        }

        public async Task<IEnumerable<LeaderboardRowVm>?> GetLeaderboardEntriesByQuizIdAsync(int quizId)
        {
            IEnumerable<LeaderboardEntry> entries = await _leaderboardRepository.GetLeaderboardEntriesByIdAsync(quizId);

            return entries
                .Select((e, index) => new LeaderboardRowVm
                {
                    Rank = index + 1,
                    UserName = e.User?.UserName ?? "(unknown)",
                    Score = e.Score
                })
                .ToList();
        }

        public async Task<Leaderboard?> GetLeaderboardByQuizIdAsync(int id)
        {
            return await _leaderboardRepository.GetLeaderboardsWithEntriesByQuizIdAsync(id);
        }
    }
}
