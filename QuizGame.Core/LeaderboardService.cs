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
            IEnumerable<LeaderboardEntry> entries = await _leaderboardRepository.GetLeaderboardWithEntriesAndUserBydAsync(quizId);

            return entries
                .Select((e, index) => new LeaderboardRowVm
                {
                    Rank = index + 1,
                    UserName = e.User?.UserName ?? "(unknown)",
                    Score = e.Score
                })
                .ToList();
        }

        public async Task<Leaderboard?> GetLeaderboardByQuizIdAsync(Guid id)
        {
            return await _leaderboardRepository.GetLeaderboardsWithEntriesByQuizIdAsync(id);
        }

        public async Task<IEnumerable<AdminLeaderboardViewModel>> GetLeaderboardsToManageAsync()
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
                });

            return leaderboardsViewModels;
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
                    UserName = entry.User!.UserName!
                });
            }
            
            return leaderboardsViewModels;
        }

        public async Task<AdminManageEntriesViewModel> GetLeaderboardsEntriesToManageDetailsAsync(Guid id)
        {
            Leaderboard? leaderboard = await _leaderboardRepository
                .GetLeaderboardWithEntriesAndUserBydAsync(id);

            return new AdminManageEntriesViewModel
            {
                LeaderboardId = leaderboard.Id,
                LeaderboardTitle = leaderboard.Title,
                LastUpdated = leaderboard.LastUpdated,
                
                Entries = leaderboard.Entries.Select(entry => new AdminLeaderboardEntryViewModel
                {
                    LeaderboardId = entry.LeaderboardId,
                    LeaderboardTitle = entry.Leaderboard.Title,
                    Id = entry.Id,
                    Rank = entry.Rank,
                    Score = entry.Score,
                    UserId = entry.UserId,
                    UserName = entry?.User!.UserName!
                }).ToList(),
                NewEntry = leaderboard.Entries.Select(entry => new AddEntryInputModel
                {
                    Rank = entry.Rank,
                    Score = entry.Score,
                    UserId = entry.UserId,
                }).First(),
                AvailableUsers = leaderboard.Entries.Select(entry => new UserSelectViewModel
                {
                    Id = entry!.UserId,
                    UserName = entry.User.UserName!
                }).ToList()
            };
        }

        public async Task<bool> RestoreEntryAsync(Guid entryId)
        {
            if (entryId == Guid.Empty)
            {
                throw new InvalidOperationException();
            }

            bool result = await _leaderboardRepository
                .RestoreEntryAsync(entryId);

            return result;
        }

        public async Task<bool> SoftDeleteEntryAsync(Guid entryId)
        {
            if (entryId == Guid.Empty)
            {
                throw new InvalidOperationException();
            }

            bool result = await _leaderboardRepository
                .SoftDeleteEntryAsync(entryId);

            return result;
        }

        public async Task<bool> HardDeleteEntryAsync(Guid entryId)
        {
            if (entryId == Guid.Empty)
            {
                throw new InvalidOperationException();
            }

            bool result = await _leaderboardRepository
                .HardDeleteEntryAsync(entryId);

            return result;
        }
    }
}
