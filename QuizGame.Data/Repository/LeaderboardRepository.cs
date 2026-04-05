namespace QuizGame.Data.Repository
{
    using Microsoft.EntityFrameworkCore;
    using QuizGame.Data.Models;
    using QuizGame.Data.Repository.Contracts;

    public class LeaderboardRepository : BaseRepository, ILeaderboardRepository
    {
        public LeaderboardRepository(QuizGameDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<Leaderboard?> GetLeaderboardWithEntriesAndUserBydAsync(Guid? leaderboardId)
        {
            return await DbContext.Leaderboards
                .AsNoTracking()
                .Include(l => l.Entries)
                    .ThenInclude(e => e.User)
                .AsSplitQuery()
                .FirstOrDefaultAsync(l => l.Id == leaderboardId);
        }

        public async Task<Leaderboard?> GetLeaderboardWithEntriesAndUserByQuizIdAsync(Guid? quizId)
        {
            return await DbContext.Leaderboards
                .AsNoTracking()
                .Include(l => l.Entries)
                    .ThenInclude(e => e.User)
                    .AsSplitQuery()
                .FirstOrDefaultAsync(l => l.QuizId == quizId);
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardWithEntriesAndUserByIdAsync(Guid? id)
        {
            return await DbContext.LeaderboardEntries
                .AsNoTracking()
                .Where(e => e.LeaderboardId == id)
                .Include(e => e.User)
                .OrderByDescending(e => e.Score)
                .ToListAsync();
        }

        public async Task<IEnumerable<Leaderboard>> GetLeaderboardsWithQuizzesAsync()
        {
            return await DbContext.Leaderboards
                .AsNoTracking()
                .Include(l => l.Quiz)
                .OrderByDescending(l => l.LastUpdated)
                .ToListAsync();
        }

        public async Task<Leaderboard?> GetLeaderboardsWithEntriesByQuizIdAsync(Guid? quizId)
        {
            return await DbContext.Leaderboards
                .Include(l => l.Entries)
                .FirstOrDefaultAsync(l => l.QuizId == quizId);
        }


        public async Task<List<LeaderboardEntry>> GetLeaderboardEntriesOrderedByScoreByLeaderboardIdAsync
            (Guid? leaderboardId)
        {
            return await DbContext.LeaderboardEntries
                .Where(e => e.LeaderboardId == leaderboardId)
                .OrderByDescending(e => e.Score)
                .ThenBy(e => e.Id)
                .ToListAsync();
        }

        public async Task<LeaderboardEntry?> GetLeaderboardEntryForUserByIdAsync(Guid? leaderboardId, Guid? userId)
        {
            return await DbContext.LeaderboardEntries
                .FirstOrDefaultAsync(e => e.Id == leaderboardId && e.UserId == userId);
        }

        public async Task<LeaderboardEntry?> GetLeaderboardEntryByIdAsync(Guid? entryId)
        {
            return await DbContext
                .LeaderboardEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == entryId);
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardsWithEntriesAsync()
        {
            return await DbContext.LeaderboardEntries
                .AsNoTracking()
                .Include(l => l.Leaderboard)
                .Include(l=>l.User)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardsWithEntriesWithQuizAsync()
        {
            return await DbContext.LeaderboardEntries
                .AsNoTracking()
                .Include(l => l.Leaderboard)
                .Include(l => l.User)
                .Include(l=>l.Leaderboard.Quiz)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<bool> AddLeaderboardAsync(Leaderboard leaderboard)
        {
            await DbContext.Leaderboards.AddAsync(leaderboard);
            int resultCount = await SaveChangesAsync();

            return resultCount > 0;
        }


        public async Task<bool> AddLeaderboardEntryAsync(LeaderboardEntry leaderboardEntry)
        {
            await DbContext.LeaderboardEntries.AddAsync(leaderboardEntry);
            int resultCount = await SaveChangesAsync();

            return resultCount > 0;
        }

        public async Task<bool> UpdateLeaderboardEntriesAsync(LeaderboardEntry leaderboardEntry)
        {
            DbContext.LeaderboardEntries.Update(leaderboardEntry);
            int resultCount = await SaveChangesAsync();

            return resultCount > 0;
        }

        public async Task<bool> RestoreEntryAsync(Guid? id)
        {
            LeaderboardEntry? entry = DbContext
                .LeaderboardEntries
                .FirstOrDefault(l => l.Id == id);

            if (entry == null)
            {
                return false;
            }

            entry.IsDeleted = false;
            int resultsCount = await SaveChangesAsync();

            return resultsCount > 0;
        }
        public async Task<bool> SoftDeleteEntryAsync(Guid? id)
        {
            LeaderboardEntry? entry = DbContext
                .LeaderboardEntries
                .FirstOrDefault(l => l.Id == id);

            if (entry == null)
            {
                return false;
            }

            entry.IsDeleted = true;
            int resultsCount = await SaveChangesAsync();

            return resultsCount > 0;
        }

        public async Task<bool> HardDeleteEntryAsync(Guid? id)
        {
            LeaderboardEntry? entry = DbContext
                .LeaderboardEntries
                .FirstOrDefault(l => l.Id == id);

            if (entry == null)
            {
                return false;
            }

            DbContext.Remove(entry);
            int resultsCount = await SaveChangesAsync();

            return resultsCount > 0;
        }

    }
}
