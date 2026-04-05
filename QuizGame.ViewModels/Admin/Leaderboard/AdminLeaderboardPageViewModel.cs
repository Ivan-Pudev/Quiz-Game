namespace QuizGame.ViewModels.Admin.Leaderboard
{
    using System.Collections.Generic;

    public class AdminLeaderboardPageViewModel
    {
        public IEnumerable<AdminLeaderboardViewModel> Leaderboards { get; set; }
        =new List<AdminLeaderboardViewModel>();

        public int Total { get; set; }
        public int TotalEntries { get; set; }
        public int UpdatedToday { get; set; }
        public double AvgEntries { get; set; }
    }
}
