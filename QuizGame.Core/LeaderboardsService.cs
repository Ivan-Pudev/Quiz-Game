using Microsoft.EntityFrameworkCore;
using QuizGame.Core.Contracts;
using QuizGame.Data;
using QuizGame.Data.Models;
using QuizGame.ViewModels.Leaderboards;

namespace QuizGame.Core
{
    public class LeaderboardsService : ILeaderboardsService
    {
        private readonly QuizGameDbContext _dbContext;

        public LeaderboardsService(QuizGameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Leaderboard>> GetLeaderboardsAsync()
        {
            return await _dbContext.Leaderboards
                .AsNoTracking()
                .Include(l => l.Quiz)
                .OrderByDescending(l => l.LastUpdated)
                .ToListAsync();
        }

        public async Task<List<LeaderboardRowVm>?> GetLeaderboardsEntriesByIdAsync(int id)
        {
            var entries = await _dbContext.LeaderboardEntries
                .AsNoTracking()
                .Include(e => e.User)
                .Where(e => e.LeaderboardId == id)
                .OrderByDescending(e => e.Score)
                .ThenBy(e => e.Id)
                .ToListAsync();

            return entries.Select((e, index) => new LeaderboardRowVm
            {
                Rank = index + 1,
                UserName = e.User?.UserName ?? "(unknown)",
                Score = e.Score
            }).ToList();
        }
    }
}
