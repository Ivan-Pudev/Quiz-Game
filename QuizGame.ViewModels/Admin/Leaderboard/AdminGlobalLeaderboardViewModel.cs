using System;
using System.Collections.Generic;
using System.Text;

namespace QuizGame.ViewModels.Admin.Leaderboard
{
    public class AdminGlobalLeaderboardViewModel
    {
        public List<GlobalEntryViewModel> Entries { get; set; }
        public List<QuizBreakdownViewModel> QuizBreakdown { get; set; }
    }
}
