using System;
using System.Collections.Generic;
using System.Text;

namespace QuizGame.ViewModels.Admin.Leaderboard
{
    public class AdminGlobalLeaderboardViewModel
    {

        public List<GlobalLeaderboardRowVm> RankedEntries { get; set; }
        =new List<GlobalLeaderboardRowVm>();

        public List<EntryVm> Top3 { get; set; } = new List<EntryVm>();

        public List<QuizBreakdownViewModel> QuizBreakdown { get; set; }
        = new List<QuizBreakdownViewModel>();

        public int TotalPlayers { get; set; }
        public int TopScore { get; set; }
        public double AvgScore { get; set; }
        public int TotalAttempts { get; set; }

    }
}
