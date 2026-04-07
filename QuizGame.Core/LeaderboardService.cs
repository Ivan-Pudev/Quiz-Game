using QuizGame.Core.Contracts;
using QuizGame.Data.Models;
using QuizGame.Data.Repository;
using QuizGame.Data.Repository.Contracts;
using QuizGame.ViewModels.Admin.Leaderboard;
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

        public async Task<IEnumerable<LeaderboardRowVm>?> GetLeaderboardEntriesByQuizIdAsync(Guid? quizId)
        {
            IEnumerable<LeaderboardEntry> entries = await _leaderboardRepository.GetLeaderboardWithEntriesAndUserByIdAsync(quizId);

            return entries
                .Select((e, index) => new LeaderboardRowVm
                {
                    Rank = index + 1,
                    UserName = e.User?.UserName ?? "(unknown)",
                    Score = e.Score
                })
                .ToList();
        }

        public async Task<Leaderboard?> GetLeaderboardByQuizIdAsync(Guid? id)
        {
            return await _leaderboardRepository.GetLeaderboardsWithEntriesByQuizIdAsync(id);
        }

        public async Task<AdminLeaderboardPageViewModel> GetLeaderboardsToManageAsync()
        {
            IEnumerable<Leaderboard> leaderboards = await GetLeaderboardsAsync();

            IEnumerable<AdminLeaderboardViewModel> leaderboardsViewModels = leaderboards
                .Select(l => new AdminLeaderboardViewModel
                {
                    Id = l.Id,
                    QuizId = l.QuizId,
                    QuizTitle = l.Quiz.Title,
                    Description = l.Quiz.Description,
                    EntryCount = l.Entries.Count(),
                    LastUpdated = l.LastUpdated,
                    Title = l.Quiz.Title,
                })
                .ToList();

            AdminLeaderboardPageViewModel pageViewModel = new AdminLeaderboardPageViewModel
            {
                Leaderboards = leaderboardsViewModels,
                Total = leaderboardsViewModels.Count(),
                TotalEntries = leaderboardsViewModels.Sum(x => x.EntryCount),
                UpdatedToday = leaderboardsViewModels.Count(x => x.LastUpdated == DateOnly.FromDateTime(DateTime.UtcNow)),
                AvgEntries = leaderboardsViewModels.Any() ? leaderboardsViewModels.Average(x => x.EntryCount) : 0
            };

            return pageViewModel;
        }

        public async Task<IEnumerable<AdminLeaderboardEntryViewModel>> GetLeaderboardsEntriesToManageAsync()
        {
            IEnumerable <LeaderboardEntry> leaderboardEntries = await _leaderboardRepository
                .GetLeaderboardsWithEntriesAsync();

            var leaderboardsViewModels = new List<AdminLeaderboardEntryViewModel>();
            foreach (var entry in leaderboardEntries)
            {
                leaderboardsViewModels.Add(new AdminLeaderboardEntryViewModel
                {
                    Id = entry.Id,
                    LeaderboardId = entry.LeaderboardId,
                    LeaderboardTitle = entry.Leaderboard.Title,
                    Rank = entry.Rank,
                    Score = entry.Score,
                    UserId = entry.UserId,
                    UserName = entry.User!.UserName!,
                    IsDeleted = entry.IsDeleted
                });
            }
            
            return leaderboardsViewModels;
        }

        public async Task<AdminManageEntriesViewModel> GetLeaderboardsEntriesToManageDetailsAsync(Guid? id)
        {
            Leaderboard? leaderboard = await _leaderboardRepository
                .GetLeaderboardWithEntriesAndUserBydAsync(id);
            var filteredEntries = leaderboard.Entries.Where(e => e.IsDeleted == false).ToList();

            List<AdminLeaderboardEntryViewModel> entries = new List<AdminLeaderboardEntryViewModel>();
            List<UserSelectViewModel> users = new List<UserSelectViewModel>();
            foreach (var entry in filteredEntries)
            {
                entries.Add(new AdminLeaderboardEntryViewModel
                {
                    LeaderboardId = entry.LeaderboardId,
                    LeaderboardTitle = entry.Leaderboard.Title,
                    Id = entry.Id,
                    Rank = entry.Rank,
                    Score = entry.Score,
                    UserId= entry.UserId,
                    UserName = entry?.User!.UserName!,
                    IsDeleted = entry!.IsDeleted
                });

                users.Add(new UserSelectViewModel
                {
                    Id = entry!.UserId,
                    UserName = entry.User.UserName!
                });
            }

            return new AdminManageEntriesViewModel
            {
                LeaderboardId = leaderboard.Id,
                LeaderboardTitle = leaderboard.Title,
                LastUpdated = leaderboard.LastUpdated,
                Entries = entries,
                AvailableUsers = users,
            };
        }

        public async Task<AdminGlobalLeaderboardViewModel> GetGlobalLeaderboardAsync()
        {
            IEnumerable<LeaderboardEntry> entries = await _leaderboardRepository
                .GetLeaderboardsWithEntriesWithQuizAsync();
           
            AdminGlobalLeaderboardViewModel globalLeaderboardViewModel = new AdminGlobalLeaderboardViewModel
            {
                RankedEntries = entries.Select(e => new GlobalLeaderboardRowVm
                {
                    UserId = e.UserId,
                    UserName = e.User!.UserName!,
                    TotalScore = e.Score,
                    Attempts = 0,
                    BestScore = 0,
                    AverageScore = 0,
                }).ToList(),

                QuizBreakdown = entries.Select(e=>new QuizBreakdownViewModel
                {
                    QuizTitle = e.Leaderboard.Quiz.Title,
                    EntryCount = e.Leaderboard.Entries.Count()
                }).ToList()
            };

            return globalLeaderboardViewModel;
        }

        public async Task UpdateEntryAsync(Guid? entryId, int score)
        {
            LeaderboardEntry? entry = await _leaderboardRepository
                .GetLeaderboardEntryByIdAsync(entryId);

            if (entry == null)
            {
                throw new InvalidOperationException();
            }

            entry.Score = score;

            bool isUpdateSuccessful = await _leaderboardRepository
                .UpdateLeaderboardEntriesAsync(entry);

            if (!isUpdateSuccessful)
            {
                throw new InvalidOperationException();
            }
        }

        public async Task RestoreEntryAsync(Guid? id)
        {
           
            bool isRestoreSuccessful = await _leaderboardRepository
                .RestoreEntryAsync(id);

            if (!isRestoreSuccessful)
            {
                throw new InvalidOperationException();
            }
        }

        public async Task SoftDeleteEntryAsync(Guid? id)
        {
            
            bool isSoftDeleteSuccessful = await _leaderboardRepository
                .SoftDeleteEntryAsync(id);

            if (!isSoftDeleteSuccessful)
            {
                throw new InvalidOperationException();
            }
        }

        public async Task HardDeleteEntryAsync(Guid? id)
        {
            bool isSoftHardSuccessful = await _leaderboardRepository
                .HardDeleteEntryAsync(id);

            if (!isSoftHardSuccessful)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
