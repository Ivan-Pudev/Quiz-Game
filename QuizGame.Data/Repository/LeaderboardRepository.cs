namespace QuizGame.Data.Repository
{
    using Microsoft.EntityFrameworkCore;
    using QuizGame.Data.Models;
    using QuizGame.Data.Repository.Contracts;
    using QuizGame.ViewModels.Leaderboards;

    public class LeaderboardRepository : BaseRepository, ILeaderboardRepository
    {
        public LeaderboardRepository(QuizGameDbContext dbContext) 
            : base(dbContext)
        {
        }

        public async Task<Leaderboard?> GetLeaderboardWithEntriesAndUserByQuizIdAsync(int quizId)
        {
            return await DbContext.Leaderboards
                .AsNoTracking()
                .Include(l => l.Entries)
                    .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(l => l.QuizId == quizId);
        }

        public async Task<IEnumerable<Leaderboard>> GetLeaderboardsWithQuizzesAsync()
        {
            return await DbContext.Leaderboards
                .AsNoTracking()
                .Include(l => l.Quiz)
                .OrderByDescending(l => l.LastUpdated)
                .ToListAsync();
        }

        public async Task<Leaderboard?> GetLeaderboardsWithEntriesByQuizIdAsync(int quizId)
        {
            return await DbContext.Leaderboards
                .Include(l => l.Entries)
                .FirstOrDefaultAsync(l => l.QuizId == quizId);
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardEntriesByIdAsync(int id)
        {
            return await DbContext.LeaderboardEntries
                .AsNoTracking()
                .Where(e => e.LeaderboardId == id)
                .Include(e => e.User)
                .OrderByDescending(e => e.Score)
                .ToListAsync();
        }

        public async Task<List<LeaderboardEntry>> GetLeaderboardEntriesOrderedByScoreByLeaderboardIdAsync(int leaderboardId)
        {
            return await DbContext.LeaderboardEntries
                .Where(e => e.LeaderboardId == leaderboardId)
                .OrderByDescending(e => e.Score)
                .ThenBy(e => e.Id)
                .ToListAsync();
        }

        public async Task<bool> AddLeaderboardAsync(Leaderboard leaderboard)
        {
            await DbContext.Leaderboards.AddAsync(leaderboard);
            int resultCount = await SaveChangesAsync();

            return resultCount == 1;
        }

        public async Task<LeaderboardEntry?> GetLeaderboardEntryForUserByIdAsync(int leaderboardId, string userId)
        {
            return await DbContext.LeaderboardEntries
                .FirstOrDefaultAsync(e => e.Id == leaderboardId && e.UserId == userId);
        }

        public async Task<bool> AddLeaderboardEntryAsync(LeaderboardEntry leaderboardEntry)
        {
            await DbContext.LeaderboardEntries.AddAsync(leaderboardEntry);
            int resultCount = await SaveChangesAsync();

            return resultCount == 1;
        }

        public async Task<bool> UpdateLeaderboardEntriesAsync(LeaderboardEntry leaderboardEntry)
        {
            DbContext.LeaderboardEntries.Update(leaderboardEntry);
            int resultCount = await SaveChangesAsync();

            return resultCount == 1;
        }
    }
}
