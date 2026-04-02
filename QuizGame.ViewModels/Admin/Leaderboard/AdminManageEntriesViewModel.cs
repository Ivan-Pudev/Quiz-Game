namespace QuizGame.ViewModels.Admin.Leaderboard
{
    using System;
    using System.Collections.Generic;
    using System.Text;


    public class AdminManageEntriesViewModel
    {
        public Guid LeaderboardId { get; set; }
        public string LeaderboardTitle { get; set; } = null!;
        public DateOnly LastUpdated { get; set; }
        public List<AdminLeaderboardEntryViewModel> Entries { get; set; }
        = new List<AdminLeaderboardEntryViewModel>();
        public List<UserSelectViewModel> AvailableUsers { get; set; }
        = new List<UserSelectViewModel>();
        public AddEntryInputModel NewEntry { get; set; } = null!;
    }
}
